using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Common
{
  public static partial class FileHelper
  {
    public static string Root => AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// Root 기준으로 경로 생성
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public static string RootRelativePath(params string[] paths)
    {
      string [] arr = new string[paths.Length+1];
      int index = 0;
      arr[index++] = Root;
      foreach (var p in paths)
      {
        arr[index++] = p;
      }
      return Path.Combine(arr);
    }
    /// <summary>
    /// 지정된 파일 경로에 해당하는 디렉토리를 생성한다.
    /// "" 또는 "."는 현재 디렉토리를 의미하며, 이 경우에는 생성하지 않는다. (true 리턴)
    /// 상대 경로인 경우 현재 위치를 기준으로 생성한다
    /// 주의
    /// - 디렉토리 경로를 넘기지 말것 (GetDirectoryName() 때문에 제대로 생성되지 않는다)
    /// </summary>
    /// <param name="path">파일 경로 (절대/상대 경로 모두 가능)</param>
    /// <returns></returns>
    public static bool CreateFileDirectory(this string path)
    {
      if (string.IsNullOrEmpty(path))
        return false;

      var dir = Path.GetDirectoryName(path);
      if (string.IsNullOrEmpty(dir) || dir == ".")
        return true;

      if (!Directory.Exists(dir))
      {
        try
        {
          Directory.CreateDirectory(dir);
          return true;
        }
        catch (Exception ex)
        {
          Debug.WriteLine($"Create directory failed: {ex.Message}");
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// 지정된 폴더를 비운다.
    /// </summary>
    public static void ClearDirectory(string src_dir)
    {
      var dir = new DirectoryInfo(src_dir);
      if (dir.Exists)
      {
        foreach (var sub_dir in dir.EnumerateDirectories())
        {
          RemoveDirectory(sub_dir.FullName);
        }
        foreach (FileInfo file in dir.GetFiles())
        {
          file.IsReadOnly = false; // readonly 속성 제거
          file.Delete();
        }
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

      Directory.CreateDirectory(dest_dir);

      bool is_backup = !string.IsNullOrEmpty(backup_dir);
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
    public static string CreateTempDir(string prefix)
    {
      var dir_name = $"{prefix}-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}";
      var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir_name);
      RemoveDirectory(path);
      Directory.CreateDirectory(path);
      return path;
    }
    /// <summary>
    /// 지정된 directory의 파일을 전달된 파일만 남기고 삭제
    /// 목록에 있는 파일이 없으면 리턴
    /// </summary>
    /// <param name="src_dir">동기화할 폴더</param>
    /// <param name="filenames">파일 목록</param>
    /// <returns>존재하지 않는 파일</returns>
    public static List<string> SyncFiles(string src_dir, List<string> filenames, params string[] excepts)
    {
      var dir = new DirectoryInfo(src_dir);
      if (!dir.Exists) return filenames;

      var deleted = new List<string>();
      foreach (FileInfo file in dir.GetFiles())
      {
        var name = file.Name; // aaa.txt

        if (excepts.Contains(name))
          continue;

        if (!filenames.Contains(name))
        {
          deleted.Add(name);
          file.Delete();
        }
      }
      return deleted;
    }
  }
}