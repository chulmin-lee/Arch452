using System;
using System.Text;

namespace Common
{
  public static class LittleEndianConverter
  {
    /// <summary>
    /// Unicode( = UTF16, 2byte)를 사용해서 변환할것
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static byte[] GetBytes(string s) => Encoding.Unicode.GetBytes(s);
    public static string GetString(byte[] s) => Encoding.Unicode.GetString(s);
    public static string GetString(this ArraySegment<byte> s) => s.Array != null ? Encoding.Unicode.GetString(s.Array, s.Offset, s.Count) : string.Empty;
    public static ushort ToUInt16(this ArraySegment<byte> d) => d.Array != null ? BitConverter.ToUInt16(d.Array, d.Offset) : (ushort)0;
    public static short ToInt16(this ArraySegment<byte> d) => d.Array != null ? BitConverter.ToInt16(d.Array, d.Offset) : (short)0;
    public static uint ToUInt32(this ArraySegment<byte> d) => d.Array != null ? BitConverter.ToUInt32(d.Array, d.Offset) : 0u;
    public static int ToInt32(this ArraySegment<byte> d) => d.Array != null ? BitConverter.ToInt32(d.Array, d.Offset) : 0;
    public static ulong ToUInt64(this ArraySegment<byte> d) => d.Array != null ? BitConverter.ToUInt64(d.Array, d.Offset) : 0ul;
    public static long ToInt64(this ArraySegment<byte> d) => d.Array != null ? BitConverter.ToInt64(d.Array, d.Offset) : 0L;
  }
}