using System.IO;
using System.Linq;

namespace Common
{
  internal abstract class FileComparer
  {
    protected FileInfo  First;
    protected FileInfo  Second;

    protected bool IsSameSize => First.Length == Second.Length;
    protected bool IsSamePath => First.FullName == Second.FullName;
    protected bool Exists => First.Exists && Second.Exists;

    public FileComparer(string file1, string file2)
    {
      First = new FileInfo(file1);
      Second = new FileInfo(file2);
    }

    public bool IsSame()
    {
      if (!Exists) return false;
      if (IsSamePath) return true;
      if (!IsSameSize) return false;

      return OnCompare();
    }

    protected abstract bool OnCompare();
  }

  internal class ByteFileComparer : FileComparer
  {
    public ByteFileComparer(string file1, string file2) : base(file1, file2)
    {
    }
    protected override bool OnCompare()
    {
      return File.ReadAllBytes(First.FullName).SequenceEqual(File.ReadAllBytes(Second.FullName));
    }
  }
}