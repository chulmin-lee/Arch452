using Common;
using System;
using System.Threading;

namespace Framework.Network
{
  public enum MB_TYPE
  {
    SINGLE_BLOCK = 0, // 단일 블럭 (기본)
    BIG_BLOCK = 1,    // 여러 블럭을 합친 경우
    GC_BLOCK = 2,     // GC로 제거될 블럭
  }

  public class MemoryBlock : IDisposable
  {
    public MB_TYPE BlockType { get; private set; } = MB_TYPE.SINGLE_BLOCK; // 블럭 타입 (기본은 단일 블럭)
    public int UseBlockCount { get; private set; } = 1; // MemoryBlock을 구성하는 블럭갯수 (기본 1개)
    IMemoryBlockOwner Owner;
    public byte[] Array => Owner.Buffers;
    public int BlockSize { get; private set; } // => Owner.BlockSize; 여러 블럭을 묶는 경우가 있다
    public int BlockIndex { get; private set; }

    long _returned = 0;

    /// <summary>
    /// 원본 기준으로 블럭의 시작 위치
    /// </summary>
    readonly int BlockOffset;

    int Front = 0; // 읽을 위치
    int Rear = 0;  // 쓸 위치

    /// <summary>
    /// 읽을 위치
    /// </summary>
    public int Offset => BlockOffset + Front;
    /// <summary>
    /// 기록할 위치
    /// </summary>
    public int Position => BlockOffset + Rear;
    /// <summary>
    /// 저장된 데이타 크기
    /// </summary>
    public int Count => Rear - Front;
    /// <summary>
    /// 여유 공간
    /// </summary>
    public int FreeSpace => BlockSize - Rear;
    public bool IsEmpty => Front == Rear;
    public bool IsFull => BlockSize == Rear;
    /// <summary>
    /// 이 블럭이 사용중인가?
    /// </summary>
    public bool IsUse { get; private set; } = false;
    internal MemoryBlock(IMemoryBlockOwner owner, int index)
    {
      this.BlockType = MB_TYPE.SINGLE_BLOCK; // 기본은 단일 블럭
      this.UseBlockCount = 1; // 단일 블럭이므로 블럭갯수는 1개
      this.Owner = owner;
      this.BlockIndex = index;
      this.BlockOffset = this.BlockIndex * owner.BlockSize;
      this.BlockSize = owner.BlockSize;
    }

    /// <summary>
    /// 일정 범위의 연속되는 인덱스들을 하나의 MemoryBlock으로 만든다
    /// 반환시 범위의 모든 블럭을 반환하기 위해서 max/min index로 BlockIndex를 생성한다
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="min_index">범위 시작 인덱스</param>
    /// <param name="max_index">범위 마지막 인덱스</param>
    internal MemoryBlock(IMemoryBlockOwner owner, int min_index, int max_index)
    {
      this.BlockType = MB_TYPE.BIG_BLOCK; // 여러 블럭을 합친 경우
      this.UseBlockCount = max_index - min_index + 1; // 블럭갯수는 범위에 있는 블럭들의 갯수
      this.Owner = owner;

      // 이전 index 범위를 복구하기 위해서 min_index와 max_index를 조합하여 BlockIndex를 설정한다
      // 주의: MemoryPool의 블럭갯수는 65536개 (0~65535) 보다는 작아야 한다
      this.BlockIndex = max_index << 16 | min_index;

      // 블럭의 시작 위치는 min_index * BlockSize로 설정한다
      this.BlockOffset = min_index * owner.BlockSize;
      // 블럭 크기는 범위에 있는 블럭들의 크기를 합친다
      this.BlockSize = owner.BlockSize * this.UseBlockCount;
    }

    /// <summary>
    /// 버퍼 부족시 직접 생성해서 제공하는 경우
    /// BlockIndex는 -1로 설정된다
    /// </summary>
    internal MemoryBlock(IMemoryBlockOwner gcowner)
    {
      this.BlockType = MB_TYPE.GC_BLOCK; // GC로 제거될 블럭
      this.UseBlockCount = 0; // GC로 제거될 블럭은 블럭갯수가 없다
      this.Owner = gcowner;
      this.BlockIndex = -1;
      this.BlockOffset = 0;
      this.BlockSize = gcowner.BlockSize;
    }

    /// <summary>
    /// 현재 블럭을 사용중으로 표시한다.
    /// </summary>
    /// <param name="id"></param>
    public void UseBuffer()
    {
      _returned = 0;
      this.Clear();
      this.IsUse = true; // 사용중 표시를 한다
    }
    public void UnuseBuffer()
    {
      this.Clear();
      this.IsUse = false; // 사용중 표시를 해제한다
    }
    /// <summary>
    /// 현재 블럭을 반환한다
    /// </summary>
    public void ReturnBuffer()
    {
      // 중복 반환 방지
      if (Interlocked.Exchange(ref _returned, 1) == 0)
      {
        Owner.ReturnBuffer(this);
      }
      else
      {
        LOG.ec($"MemoryBlock returned locked BlockIndex: {this.BlockIndex}");
      }
    }
    void Clear()
    {
      this.Front = 0;
      this.Rear = 0;
    }
    /// <summary>
    /// 현재 기록된 데이타를 ArraySegment로 반환한다.
    /// - Offset : 읽을 데이타 위치 (BlockOffset + Front)
    /// - Count : 현재 기록된 데이타 크기 (Rear - Front)
    /// </summary>
    /// <returns></returns>
    public ArraySegment<byte> ToArraySegment()
    {
      return new ArraySegment<byte>(Array, this.Offset, this.Count);
    }

    /// <summary>
    /// 데이타를 추가한다.
    /// </summary>
    /// <returns>추가한 데이타 크기</returns>
    public int Write(byte[] data) => this.Write(new ArraySegment<byte>(data));
    public int Write(byte[] data, int offset, int count) => this.Write(ArrayHelper.ToArraySegment(data, offset, count));
    /// <summary>
    /// 데이타를 추가한다.
    /// </summary>
    /// <param name="seg"></param>
    /// <returns>추가한 데이타 갯수</returns>
    public int Write(ArraySegment<byte> seg)
    {
      if (seg.Array == null)
        throw new ArgumentNullException(nameof(seg));

      if (seg.Count == 0) return 0;

      int count = this.FreeSpace > seg.Count ? seg.Count : this.FreeSpace;
      Buffer.BlockCopy(seg.Array, seg.Offset, this.Array, this.Position, count);
      this.Rear += count;
      return count;
    }

    public ArraySegment<byte> ReadAll() => this.Read(0, this.Count);
    public ArraySegment<byte> Read(int count) => this.Read(0, count);
    public ArraySegment<byte> Read(int offset, int count)
    {
      var data = this.Peek(offset, count);
      this.Front += data.Count;
      return data;
    }
    public ArraySegment<byte> Peek(int count) => this.Peek(0, count);
    public ArraySegment<byte> Peek(int offset, int count)
    {
      if (count == 0 || offset >= this.Count)
        return new ArraySegment<byte>();

      count = Math.Min(count, this.Count);
      return new ArraySegment<byte>(this.Array, this.Offset, count);
    }
    public byte[] ToArray()
    {
      var d = new byte[this.Count];
      Buffer.BlockCopy(this.Array, this.Offset, d, 0, this.Count);
      return d;
    }
    public int Skip(int count)
    {
      if (this.Count < count)
      {
        this.Front = 0;
        this.Rear = 0;
      }
      else
      {
        this.Front += count;
      }
      return this.Count;
    }
    public byte this[int index]
    {
      get
      {
        if ((uint)index >= (uint)this.Count)
        {
          throw new ArgumentOutOfRangeException("index");
        }

        return this.Array[this.Offset + index];
      }
      set
      {
        if ((uint)index >= (uint)this.Count)
        {
          throw new ArgumentOutOfRangeException("index");
        }
        this.Array[this.Offset + index] = value;
      }
    }
    public ushort ToUInt16(int index) => BitConverter.ToUInt16(this.Array, this.Offset + index);
    public short ToInt16(int index) => BitConverter.ToInt16(this.Array, this.Offset + index);
    public uint ToUInt32(int index) => BitConverter.ToUInt32(this.Array, this.Offset + index);
    public int ToInt32(int index) => BitConverter.ToInt32(this.Array, this.Offset + index);
    public ulong ToUInt64(int index) => BitConverter.ToUInt64(this.Array, this.Offset + index);
    public long ToInt64(int index) => BitConverter.ToInt64(this.Array, this.Offset + index);

    bool _disposed = false;
    public void Dispose()
    {
      // 중복방지
      if (!_disposed)
      {
        if (Interlocked.Read(ref _returned) == 0)
        {
          this.ReturnBuffer();
        }
        _disposed = true;
      }
    }
  }
}