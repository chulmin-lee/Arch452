using System.IO;

namespace ClientUpdater
{
  public static class FileHelper
  {
    /// <summary>
    /// 지정된 폴더를 비운다. 없는 경우 생성한다.
    /// </summary>
    public static void ClearDirectory(string dir)
    {
      try
      {
        RemoveDirectory(dir);
      }
      finally
      {
        // 없는 경우 만든다
        Directory.CreateDirectory(dir);
      }
    }

    public static void RemoveDirectory(string src_dir)
    {
      var dir = new DirectoryInfo(src_dir);
      if (!dir.Exists)
        return;

      foreach (var sub_dir in dir.EnumerateDirectories())
      {
        RemoveDirectory(sub_dir.FullName);
      }

      foreach (FileInfo file in dir.GetFiles())
      {
        file.IsReadOnly = false;
        file.Delete();
      }
      dir.Delete(true);
    }
    /// <summary>
    /// src에 있는 파일을 dest로 복사한다
    /// backup 폴더가 설정된 경우 backup 한다.
    /// </summary>
    public static void CopyDirectory(string src_dir, string dest_dir, string backup_dir = "")
    {
      var dir = new DirectoryInfo(src_dir);
      if (!dir.Exists)
        throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

      bool is_backup = !string.IsNullOrEmpty(backup_dir);

      Directory.CreateDirectory(dest_dir);
      if (is_backup)
      {
        Directory.CreateDirectory(backup_dir);
      }

      foreach (FileInfo file in dir.GetFiles())
      {
        var name = file.Name; // aaa.txt
        var dest_file = Path.Combine(dest_dir, name);
        if (is_backup && File.Exists(dest_file))
        {
          File.Move(dest_file, Path.Combine(backup_dir, name));
        }
        // update
        file.CopyTo(dest_file, true);
      }

      foreach (DirectoryInfo sub_dir in dir.GetDirectories())
      {
        var name = sub_dir.Name;
        var target_sub = Path.Combine(dest_dir, name);
        var backup_sub = is_backup ? Path.Combine(backup_dir, name) : "";
        CopyDirectory(sub_dir.FullName, target_sub, backup_sub);
      }
    }
  }
}