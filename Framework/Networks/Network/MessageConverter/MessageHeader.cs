using System.Reflection;
using System.Runtime.InteropServices;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Network
{
  public class MSG_HEADER
  {
    //public const byte Version = 3;
    public const ushort SD = 0xCAFE;
    public const ushort ED = 0xBABE;
    public static readonly int SD_SIZE = Marshal.SizeOf(MSG_HEADER.SD);
    public static readonly int ED_SIZE = Marshal.SizeOf(MSG_HEADER.ED);

    public static readonly int HeaderSize = Marshal.SizeOf(typeof(MessageHeader));
    public static readonly int SizeFieldOffset = GetSizeOffset();
    public static readonly int SizeFieldLength = GetSizeFieldLength();

    static int GetSizeOffset()
    {
      Type type = typeof(MessageHeader);

      if (type != null)
      {
        var fi = type.GetField(nameof(MessageHeader.message_size));
        if (fi != null)
        {
          var attr = fi.GetCustomAttribute<FieldOffsetAttribute>();
          if (attr != null)
          {
            return attr.Value;
          }
        }
      }
      throw new Exception("Could not find the message_size field or its FieldOffsetAttribute.");
    }
    static int GetSizeFieldLength()
    {
      Type type = typeof(MessageHeader);

      if (type != null)
      {
        var fi = type.GetField(nameof(MessageHeader.message_size));
        if (fi != null)
        {
          //var t = fi.FieldType;
          //switch (t)
          //{
          //  case Type _ when t == typeof(byte): return 1;
          //  case Type _ when t == typeof(sbyte): return 1;
          //  case Type _ when t == typeof(short): return 2;
          //  case Type _ when t == typeof(ushort): return 2;
          //  case Type _ when t == typeof(int): return 4;
          //  case Type _ when t == typeof(uint): return 4;
          //  case Type _ when t == typeof(long): return 8;
          //  case Type _ when t == typeof(ulong): return 8;
          //}

          switch (Type.GetTypeCode(fi.FieldType))
          {
            case TypeCode.Byte: return 1;
            case TypeCode.SByte: return 1;
            case TypeCode.Int16: return 2;
            case TypeCode.UInt16: return 2;
            case TypeCode.Int32: return 4;
            case TypeCode.UInt32: return 4;
            case TypeCode.Int64: return 8;
            case TypeCode.UInt64: return 8;
          }
        }
      }
      throw new Exception("Could not find the message_size field or its FieldOffsetAttribute.");
    }
  }

  [StructLayout(LayoutKind.Explicit, Pack = 1)]
  public struct MessageHeader
  {
    [FieldOffset(0)] public ushort sd;        // 0xCAFE
    [FieldOffset(2)] public int message_size; // header + PDU + ED
    [FieldOffset(6)] public int message_id;   // message id

    public MessageHeader(int message_id)
    {
      this.sd = MSG_HEADER.SD;
      this.message_id = message_id;
      this.message_size = 0;
    }
    public MessageHeader(int message_id, int pdu_size)
    {
      this.sd = MSG_HEADER.SD;
      this.message_id = message_id;
      this.message_size = MSG_HEADER.HeaderSize + pdu_size + MSG_HEADER.ED_SIZE;
    }
    public int PduSize => this.message_size - (MSG_HEADER.HeaderSize + MSG_HEADER.ED_SIZE);
    public byte[] ToArray() => ToArray(this);

    //==========================================
    // static methods
    //==========================================
    public static int HeaderSize => MSG_HEADER.HeaderSize;

    public static int GetPacketSize(ArraySegment<byte> seq)
    {
      return ToHeader(seq).message_size;
    }

    public static byte[] ToArray(MessageHeader header)
    {
      int headerSize = MSG_HEADER.HeaderSize;

      IntPtr ptr = Marshal.AllocHGlobal(headerSize);
      Marshal.StructureToPtr(header, ptr, true);
      byte[] arr = new byte[headerSize];
      Marshal.Copy(ptr, arr, 0, headerSize);
      Marshal.FreeHGlobal(ptr);
      return arr;
    }
    public static MessageHeader ToHeader(ArraySegment<byte> seg)
    {
      if (seg.Array != null && IsValid(seg))
      {
        IntPtr ptr = Marshal.AllocHGlobal(MSG_HEADER.HeaderSize);
        Marshal.Copy(seg.Array, seg.Offset, ptr, MSG_HEADER.HeaderSize);
        var header = Marshal.PtrToStructure<MessageHeader>(ptr);
        Marshal.FreeHGlobal(ptr);
        return header;
      }
      return new MessageHeader();
    }
    public static bool IsValid(ArraySegment<byte> data)
    {
      if (data.Array != null && data.Count >= MSG_HEADER.HeaderSize)
      {
        if (MSG_HEADER.SD == BitConverter.ToUInt16(data.Array, data.Offset))
        {
          return true;
        }
      }
      return false;
    }
  }
}