using Common;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Network
{
  class MemoryBlockMap
  {
    public int FreeCount { get; private set; }
    public int UsedCount => _blockCount - this.FreeCount;
    int _blockCount;
    byte[] _map;
    int _min_index = 0;
    int _max_index = 0;
    object _lock = new object();
    public MemoryBlockMap(int count)
    {
      if (count % 8 != 0)
      {
        throw new FrameworkException($"{count} is not 8's multiple");
      }

      _blockCount = count;
      _map = new byte[_blockCount / 8];

      _min_index = 0;
      _max_index = _blockCount - 1; // 0 부터 시작

      for (int i = _min_index; i <= _max_index; i++)
      {
        this.Set(i);
      }

      LOG.tc($"Map.MapCount: {_map.Length}, freesize: {FreeCount}");
    }
    public void Returned(int index)
    {
      lock (_lock)
      {
        int prev_free = FreeCount;
        this.Set(index);
        LOG.tc($"Returned ({index}): freesize: {prev_free} => {FreeCount}");

#if DEBUG
        this.check_freecount_test();
#endif
      }
    }
    public void Returned(int min_index, int max_index)
    {
      lock (_lock)
      {
        int prev_free = FreeCount;
        for (int i = min_index; i <= max_index; i++)
        {
          this.Set(i);
        }
        LOG.tc($"Returned ({min_index} ~ {max_index}): freesize: {prev_free} => {FreeCount}");

#if DEBUG
        this.check_freecount_test();
#endif
      }
    }
    void check_freecount_test()
    {
      if (FreeCount != this.GetFreeCount())
      {
        throw new FrameworkException($"count error: FreeCount = {this.FreeCount}, GetFreeCount() = {this.GetFreeCount()}");
      }
    }

    public int GetFreeCount()
    {
      // 남는 영역에 엉뚱한값이 있을수있다.
      int count = 0;
      foreach (var b in _map)
      {
        byte v = b;
        for (; v > 0; count++)
        {
          v &= (byte)(v - 1);
        }
      }
      return count;
    }
    public List<int> GetBlock(int count)
    {
      lock (_lock)
      {
        var list = new List<int>();

        if (count <= 0)
          return list;

        if (FreeCount < count)
        {
          LOG.tc($"------ not enough : request: {count}, free: {FreeCount}");
          return list;
        }

        if (count == 1)
        {
          list.Add(this.GetSingleBlockIndex());
        }
        else
        {
          list.AddRange(GetSequentialBlocks2(count));
        }

        var result = list.Where(x => x >=0).ToList();

        foreach (var index in result)
        {
          this.Unset(index);
        }

        LOG.tc($"req count: {count}, list:{string.Join(",", result)}, free: {FreeCount}");

        return list;
      }
    }

    /// <summary>
    /// 사용가능한 block index 검색
    /// - 파편화를 예방하기 위해서 앞에서부터 검색
    /// </summary>
    /// <returns></returns>
    int GetSingleBlockIndex()
    {
      if (FreeCount > 0)
      {
        for (int block_no = _map.Length - 1; block_no >= 0; block_no--)
        {
          if (_map[block_no] > 0)
          {
            for (int j = 7; j >= 0; j--)
            {
              if ((_map[block_no] & (1 << j)) > 0)
              {
                int index = block_no * 8 + j;
                return index;
              }
            }
          }
        }
      }
      return -1;
    }
    /// <summary>
    /// count 갯수 만큼 연속된 비트가 1인 시작 index
    /// - 파편화를 예방하기 위해서 뒤에서부터 검색
    /// - count >= 16 이면 최소한 1개의 byte는 0xFF 이다
    /// </summary>
    List<int> GetSequentialBlocks2(int count)
    {
      int remaind = count;
      int start_index = -1;
      int end_index = -1;

      for (int block_no = 0; block_no < _map.Length; block_no++)
      {
        var v = _map[block_no];
        if (v == 0)
        {
          remaind = count;
          start_index = -1;
          end_index = -1;
        }
        else if (v == 0xFF)
        {
          if (start_index == -1)
          {
            start_index = block_no * 8;
          }

          if (remaind >= 8)
          {
            remaind -= 8;

            if (remaind == 0)
            {
              end_index = block_no * 8 + 7;
            }
          }
          else
          {
            end_index = block_no * 8 + remaind - 1;
            remaind = 0;
          }
        }
        else
        {
          if (remaind >= 8)
          {
            remaind = count;
            start_index = -1;
            end_index = -1;
          }

          for (int j = 0; j < 8; j++)
          {
            if ((v & (1 << j)) > 0)
            {
              if (start_index == -1)
              {
                start_index = block_no * 8 + j;
              }
              remaind--;

              if (remaind == 0)
              {
                end_index = block_no * 8 + j;
                break;
              }
            }
            else
            {
              remaind = count;
              start_index = -1;
              end_index = -1;
            }
          }
        }

        if (remaind == 0)
          break;
      }

      var list = new List<int>();

      if (start_index < 0 || end_index < 0)
      {
        LOG.tc($"---- not enough sequence ------");
        return list;
      }
      for (int x = start_index; x <= end_index; x++)
      {
        list.Add(x);
      }
      return list;
    }
    int array_index(int index) => index / 8;
    int bit_position(int index) => index % 8;

    /// <summary>
    /// 사용 가능함을 나타낸다
    /// </summary>
    void Set(int index)
    {
      //LOG.ic($"Map Index = {index}");
      if (index <= _max_index)
      {
        FreeCount++;

        if (this.IsSet(index))
        {
          throw new FrameworkException("blockmap: already set");
        }
        _map[this.array_index(index)] |= (byte)(1 << this.bit_position(index));
      }
    }
    /// <summary>
    /// 사용할수없음 표시
    /// </summary>
    void Unset(int index)
    {
      if (index <= _max_index)
      {
        if (!this.IsSet(index))
        {
          throw new FrameworkException("blockmap: already unset");
        }

        FreeCount--;
        _map[this.array_index(index)] &= (byte)(~(1 << this.bit_position(index)));
      }
    }
    public bool IsSet(int index)
    {
      if (index <= _max_index)
      {
        return (_map[this.array_index(index)] & (byte)(1 << this.bit_position(index))) != 0;
      }
      return false;
    }
  }
}