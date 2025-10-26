using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Network
{
  /// <summary>
  /// 여러 MemroyBlock을 관리한다
  /// </summary>
  public class MemoryBlocks : IDisposable
  {
    List<MemoryBlock> _blocks = new List<MemoryBlock>();
    public int Count => _blocks.Sum(x => x.Count);
    public bool IsEmpty => this.Count == 0;

    public int Write(ArraySegment<byte> seg)
    {
      do
      {
        var block = this.GetLastBlock();
        int write = block.Write(seg);

        if (seg.Array == null)
          throw new ArgumentNullException("seg.Array == null");

        // .net framework에는 slice가 없다
        seg = new ArraySegment<byte>(seg.Array, seg.Offset + write, seg.Count - write);
      } while (seg.Count > 0);
      return this.Count;
    }
    public ArraySegment<byte> Read(int message_size)
    {
      if (this.Count < message_size)
        throw new FrameworkException($"message size");

      var block = MemoryPoolHelper.GetBlock(message_size);
      var first = _blocks.First();
      if (first.Count >= message_size)
      {
        var m = new ArraySegment<byte>(first.Array, first.Offset, message_size);
        block.Write(m);
        first.Skip(message_size);
        if (first.IsEmpty)
        {
          this.FreeBlock(first);
        }
        return block.ReadAll();
      }
      else
      {
        int index = 0;
        int remaind = message_size;
        foreach (var p in _blocks.ToArray())
        {
          var seg = p.Read(Math.Min(remaind, p.Count));
          block.Write(seg);
          index += seg.Count;
          remaind -= seg.Count;

          if (p.IsEmpty)
          {
            this.FreeBlock(p);
          }

          if (remaind == 0)
          {
            return block.ReadAll();
          }
        }
      }
      throw new FrameworkException($"message size");
    }
    public ArraySegment<byte> Peek(int count) => this.Peek(0, count);
    public ArraySegment<byte> Peek(int offset, int count)
    {
      int totalCount = this.Count;
      if (totalCount < (offset + count))
      {
        throw new IndexOutOfRangeException($"{totalCount} < {offset}+{count}");
      }

      int wanted = count;
      var block = MemoryPoolHelper.GetBlock(wanted);

      foreach (var p in _blocks)
      {
        var seg = p.Peek(offset, count);
        if (seg.Count == 0)
        {
          // offset 만큼 skip 해도 부족하다
          offset -= p.Count;
        }
        else
        {
          // 뭔가를 읽어왔다는것은 offset 이상 데이타가 있다는것
          offset = 0;
          count -= seg.Count;
          block.Write(seg);

          if (block.Count == wanted)
            return block.ReadAll();
        }
      }
      throw new FrameworkException($"what wrong?");
    }

    public byte[] Flush()
    {
      var dump = new byte[this.Count];

      int offset = 0;
      foreach (var block in _blocks.ToArray())
      {
        Buffer.BlockCopy(block.Array, block.Offset, dump, offset, block.Count);
        offset += block.Count;
      }
      this.Clear();
      return dump;
    }
    public void Clear()
    {
      if (_blocks.Count > 0)
      {
        _blocks.ForEach(block => block.ReturnBuffer());
        _blocks.Clear();
      }
    }

    public void Dispose()
    {
      this.Clear();
    }

    void FreeBlock(MemoryBlock block)
    {
      LOG.tc($"BlockIndex : {block.BlockIndex}");
      if (!_blocks.Remove(block))
      {
        LOG.ec($"FreeBlock() - block not found, BlockIndex: {block.BlockIndex}");
        return;
      }
      block.ReturnBuffer();
    }
    MemoryBlock GetLastBlock()
    {
      if (!_blocks.Any() || _blocks.Last().IsFull)
      {
        LOG.tc($"GetLastBlock() - need new block, count: {_blocks.Count}");
        _blocks.Add(MemoryPoolHelper.GetBlock());
      }
      return _blocks.Last();
    }
  }
}