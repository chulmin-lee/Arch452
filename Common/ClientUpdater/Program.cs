using System;
using System.Diagnostics;
using System.IO;

namespace ClientUpdater
{
  internal class Program
  {
    static void Main(string[] args)
    {
      log.Initialize("updater.txt");

      if (args.Length < 2)
      {
        return;
      }

      var src_dir = args[0];   // 업데이트 파일이 압축해제되서 저장된 폴더
      var run_app = args[1];       // 업데이트 완료 후 실행할 파일
      var proc_name = Path.GetFileNameWithoutExtension(run_app);
      if (!WaitClientExit(proc_name))
      {
        return;
      }

      log.d($"src: {src_dir}");
      log.d($"run: {run_app}");

      if (Directory.Exists(src_dir))
      {
        var dest_dir = AppDomain.CurrentDomain.BaseDirectory;
        FileHelper.CopyDirectory(src_dir, dest_dir);
        FileHelper.RemoveDirectory(src_dir);
      }

      var processStartInfo = new ProcessStartInfo
      {
        FileName = run_app,
        UseShellExecute = true
      };
      Process.Start(processStartInfo);
    }

    /// <summary>
    /// 확장자없이 이름만..
    /// </summary>
    /// <param name="proc_name"></param>
    /// <returns></returns>
    static bool WaitClientExit(string proc_name)
    {
      foreach (Process process in Process.GetProcessesByName(proc_name))
      {
        try
        {
          log.dc("Waiting for application to exit...");
          if (!process.WaitForExit(2000))
          {
            process.Kill();
            break;
          }
        }
        catch (Exception exception)
        {
          log.except(exception);
          return false;
        }
      }
      return true;
    }
  }
}