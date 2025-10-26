using System.IO;
using System.Threading;

namespace Common
{
  public static partial class FileHelper
  {
    /// <summary>
    /// File에 접근할 수 있는지 확인한다.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="retry"></param>
    /// <param name="waitMs"></param>
    /// <returns></returns>
    public static bool CanFileAccess(string filePath, int retry = 10, int waitMs = 10)
    {
      int attempts = 0;
      while (true)
      {
        try
        {
          using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
          {
            return true;
          }
        }
        catch (IOException)
        {
          if (++attempts > retry)
          {
            return false;
          }
          else
          {
            Thread.Sleep(waitMs);
          }
        }
      }
    }
    /// <summary>
    /// 유효한 경로 검사
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsValidPath(string path)
    {
      if (string.IsNullOrWhiteSpace(path) || path.IndexOfAny(Path.GetInvalidPathChars()) != -1 || !Path.IsPathRooted(path))
        return false;

      var pathRoot = Path.GetPathRoot(path);
      if (string.IsNullOrEmpty(pathRoot)) return false;

      if (pathRoot.Length <= 2 && pathRoot != "/") // Accepts X:\ and \\UNC\PATH, rejects empty string, \ and X:, but accepts / to support Linux
        return false;

      if (pathRoot[0] != '\\' || pathRoot[1] != '\\')
        return true; // Rooted and not a UNC path

      return pathRoot.Trim('\\').IndexOf('\\') != -1; // A UNC server name without a share name (e.g "\\NAME" or "\\NAME\") is invalid
    }

    public static bool IsValidFilename(string name)
    {
      if (!string.IsNullOrWhiteSpace(name))
      {
        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
          return true;
      }
      return false;
    }

    /// <summary>
    /// file move시 목적 위치에 파일이 존재하면 예외가 발생하므로 해당 처리 추가
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public static bool FileMove(string src, string dest)
    {
      if (File.Exists(src))
      {
        if (File.Exists(dest))
        {
          File.Delete(dest);
        }
        File.Move(src, dest);
        return File.Exists(dest);
      }
      return false;
    }
    /// <summary>
    /// 파일이 동일한지 검사한다.
    /// </summary>
    /// <param name="file1"></param>
    /// <param name="file2"></param>
    /// <returns></returns>
    public static bool IsSameFile(string file1, string file2)
    {
      var comp = new ByteFileComparer(file1, file2);
      return comp.IsSame();
    }
  }
}