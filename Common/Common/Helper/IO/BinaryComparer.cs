using System.IO;
using System.Linq;

namespace Common
{
  public static class BinaryComparer
  {
    public static bool IsSame(MemoryStream ms, string path)
    {
      return IsSame(ms.GetBuffer(), path);
    }
    public static bool IsSame(byte[] stream, string path)
    {
      if (File.Exists(path))
      {
        return IsSame(stream, File.ReadAllBytes(path));
      }
      return false;
    }
    public static bool IsSame(byte[] arr1, byte[] arr2)
    {
      if (arr1.Length == arr2.Length)
      {
        return arr1.SequenceEqual(arr2);
      }
      return false;
    }
  }
}