using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Network
{
  public static class MemoryPoolHelper
  {
    static readonly int _mapGroupSize = 8; // 8바이트 단위로 묶어서 관리
    static MemoryPool _instance;
    public static int BlockSize { get; private set; }
    public static void Initialize(int blockSize, int blockCount)
    {
      BlockSize = blockSize;

      if (_instance != null)
      {
        LOG.ec($"already initialized: size -{_instance.BlockSize}, count - {_instance.BlockCount}");
        return;
      }

      int groupCount = blockCount.CalcPageCount(_mapGroupSize);
      blockCount = groupCount * _mapGroupSize;
      if (blockCount > 0xFFFF)
      {
        throw new ArgumentOutOfRangeException($"blockCount {blockCount} is too large. max is 65536");
      }
      _instance = new MemoryPool(blockSize, blockCount);
    }
    /// <summary>
    /// memory block 요청. size가 지정된 경우 최소 크기는 size이며 더 클 수 있다
    /// </summary>
    /// <param name="size">0 이면 기본 블럭, 아니면 기본 블럭보다 크면 메모리 할당</param>
    /// <returns></returns>
    public static MemoryBlock GetBlock(int size = 0)
    {
      if (_instance == null) throw new FrameworkException("pool not created");
      return _instance.GetMemoryBlock(size);
    }
    public static string GetStatistics()
    {
      if (_instance == null) throw new FrameworkException("pool not created");
      return _instance.Statistics();
    }

    #region memory pool
    class MemoryPool : IMemoryBlockOwner, IDisposable
    {
      //ConcurrentQueue<MemoryBlock> _pool = new ConcurrentQueue<MemoryBlock>();
      MemoryBlockMap _map;
      Dictionary<int, MemoryBlock> _pool = new Dictionary<int, MemoryBlock>();
      public byte[] Buffers { get; private set; }
      public int BlockSize { get; private set; }
      public int BlockCount { get; private set; }
      public int UsedCount => _pool.Values.Where(x => x.IsUse).Count();

      object _lock = new object();
      long _totalGcBlockCount = 0;

      internal MemoryPool(int blockSize, int blockCount)
      {
        BlockSize = blockSize;
        BlockCount = blockCount;
        Buffers = new byte[blockSize * blockCount];

        for (int i = 0; i < blockCount; i++)
        {
          _pool.Add(i, new MemoryBlock(this, i));
        }
        _map = new MemoryBlockMap(this.BlockCount);
      }

      #region RENT MemoryBlock

      public MemoryBlock GetMemoryBlock() => this.GetMemoryBlock(this.BlockSize);
      public MemoryBlock GetMemoryBlock(int size)
      {
        lock (_lock)
        {
          if (size == 0)
          {
            size = this.BlockSize;
          }

          int count = size.CalcPageCount(this.BlockSize);
          var list = _map.GetBlock(count);

          LOG.tc($"GetMemoryBlock() size: {size}, MB Count: {count}, list: {string.Join(",", list)}");

          MemoryBlock block;
          if (list.Count == 0)
          {
            block = this.rent_gc_block(size);
          }
          else if (list.Count == 1)
          {
            block = this.rent_single_block(list[0]);
          }
          else
          {
            block = this.rent_big_block(list);
          }

#if DEBUG
          this.check_used_test("rent");
#endif
          return block;
        }
      }

      MemoryBlock rent_single_block(int index)
      {
        _pool[index].UseBuffer();
        return _pool[index];
      }
      MemoryBlock rent_big_block(List<int> list)
      {
        list.ForEach(index => _pool[index].UseBuffer());

        int min_index = list.Min();
        int max_index = list.Max();
        var block = new MemoryBlock(this, min_index, max_index);

        LOG.tc($"BigBlock >> new_index: {block.BlockIndex}, min_index: {min_index}, max_index: {max_index}");
        return block;
      }
      MemoryBlock rent_gc_block(int size)
      {
        _totalGcBlockCount++;
        return new MemoryBlock(new GcMemoryBlockOwner(size));
      }

      #endregion RENT MemoryBlock

      #region RETURN MemoryBlock

      /// <summary>
      /// 여러번 반환하는 경우를 예방하기 위해서 MemoryBlock에서 반환처리
      /// </summary>
      /// <param name="b"></param>
      public void ReturnBuffer(MemoryBlock b)
      {
        LOG.tc($"ReturnBuffer() Index: {b.BlockIndex}");

        lock (_lock)
        {
          switch (b.BlockType)
          {
            case MB_TYPE.GC_BLOCK: return_gc_block(b); break;
            case MB_TYPE.SINGLE_BLOCK: return_single_block(b); break;
            case MB_TYPE.BIG_BLOCK: return_big_block(b); break;
          }

#if DEBUG
          this.check_used_test("return");
#endif
        }
      }

      void return_single_block(MemoryBlock b)
      {
        b.UnuseBuffer(); // 똑같다 _pool[b.BlockIndex].UnusedBuffer();
        _map.Returned(b.BlockIndex);
      }
      void return_big_block(MemoryBlock b)
      {
        int min_index = b.BlockIndex & 0xFFFF;
        int max_index = b.BlockIndex >> 16;

        LOG.dc($"BigBlock: {b.BlockIndex}, min_index: {min_index}, max_index: {max_index}");

        for (int i = min_index; i <= max_index; i++)
        {
          _pool[i].UnuseBuffer();
        }
        _map.Returned(min_index, max_index);
      }
      void return_gc_block(MemoryBlock b)
      {
        _totalGcBlockCount--;
      }
      #endregion RETURN MemoryBlock

      void check_used_test(string mode)
      {
        if (this.UsedCount != _map.UsedCount)
        {
          throw new Exception($"count error: [{mode}] used {this.UsedCount} != {_map.UsedCount}");
        }
        if (this.BlockCount - _map.FreeCount != this.UsedCount)
        {
          throw new Exception($"freecount error: {this.BlockCount - this.UsedCount} != {_map.FreeCount}");
        }
      }

      public string Statistics()
      {
        lock (_lock)
        {
          return $"TotalBlock: {this.BlockCount}, Used:{_map.UsedCount}, Free:{_map.FreeCount}, GC:{_totalGcBlockCount}";
        }
      }

      public void Dispose()
      {
        _pool.Clear();
      }
    }
    #endregion memory pool

    #region dummy
    /// <summary>
    /// 버퍼 용량 부족시 메모리 생성해서 제공하는 경우의 owner
    /// </summary>
    class GcMemoryBlockOwner : IMemoryBlockOwner
    {
      public void ReturnBuffer(MemoryBlock b) { }
      public int BlockSize { get; private set; }
      public byte[] Buffers { get; private set; }

      public GcMemoryBlockOwner(int blockSize)
      {
        this.BlockSize = blockSize;
        this.Buffers = new byte[blockSize];
      }
    }
    #endregion dummy
  }

  public interface IMemoryBlockOwner
  {
    void ReturnBuffer(MemoryBlock b);
    int BlockSize { get; }
    byte[] Buffers { get; }
  }
}