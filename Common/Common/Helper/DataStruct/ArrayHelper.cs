using Newtonsoft.Json.Linq;
using System;

namespace Common
{
  public static class ArrayHelper
  {
    public static bool RangeCheck(byte[] data, int offset, int length)
    {
      if (data == null || offset > data.Length || length > (data.Length - offset))
      {
        return false;
      }
      return true;
    }
    public static ArraySegment<byte> ToArraySegment(byte[] data, int offset, int length)
    {
      if (ArrayHelper.RangeCheck(data, offset, length))
      {
        return new ArraySegment<byte>(data, offset, length);
      }
      throw new IndexOutOfRangeException($"{data.Length} < {offset}+{length}");
    }
    public static byte[] ToByteArray(this ArraySegment<byte> seg)
    {
      if (seg.Count == 0) return new byte[0];
      var array = new byte[seg.Count];
      System.Array.Copy(seg.Array, seg.Offset, array, 0, seg.Count);
      return array;
    }

    //public static byte[] GetBytes(this string s) => Encoding.Unicode.GetBytes(s);
    //public static string GetString(this byte[] s) => Encoding.Unicode.GetString(s);
    //public static string GetString(this ArraySegment<byte> s) => Encoding.Unicode.GetString(s.Array, s.Offset, s.Count);
  }
}