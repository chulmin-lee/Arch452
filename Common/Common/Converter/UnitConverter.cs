using System;

namespace Common
{
  public static class UnitConverter
  {
    public enum SizeUnits
    {
      Byte, KB, MB, GB, TB, PB, EB, ZB, YB
    }

    public static int ToUnitSize(this long value, SizeUnits unit)
    {
      return (int)(value / (double)Math.Pow(1024, (Int64)unit));
    }

    public static string ToByteSize(this int bytes) => ToByteSize((double)bytes);
    public static string ToByteSize(this long bytes) => ToByteSize((double)bytes);
    public static string ToByteSize(this double bytes)
    {
      string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
      int index = 0;
      while (bytes >= 1024 && index < suffixes.Length - 1)
      {
        bytes /= 1024;
        index++;
      }

      return string.Format("{0:0.##} {1}", bytes, suffixes[index]);
    }
  }
}