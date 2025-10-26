using System;
using System.Diagnostics;
using System.IO;

namespace Common
{
  public static class SystemHelper
  {
    /// <summary>
    /// 지정된 드라이브의 사용 가능 용량을 리턴
    /// </summary>
    /// <param name="drivename">C:\ </param>
    /// <returns></returns>
    public static long GetHddFreespace(string drivename = @"C:\")
    {
      foreach (DriveInfo drive in DriveInfo.GetDrives())
      {
        if (drive.IsReady && drive.Name == drivename)
        {
          return drive.AvailableFreeSpace;
        }
      }
      return -1;
    }
    public static void RunAndExit(string path)
    {
      try
      {
        Process.Start(path);
      }
      catch (Exception ex)
      {
        LOG.except(ex);
      }
      Environment.Exit(0);
    }
    public static void RunAndExit(ProcessStartInfo info)
    {
      try
      {
        Process.Start(info);
      }
      catch (Exception ex)
      {
        LOG.except(ex);
      }
      Environment.Exit(0); // 이거 없으면 빨리 안죽는다
                           //Process.GetCurrentProcess().WaitForExit(1000);
    }
    public static ProcessStartInfo CreateStartInfo(string path, string arg = null, bool hide = false)
    {
      //UseShellExecute = true,  NET.Framework: true, NET.Core: false

      var info = new ProcessStartInfo()
      {
        FileName = path,
        Arguments = arg,
        UseShellExecute = false
      };

      if (hide)
      {
        info.WindowStyle = ProcessWindowStyle.Hidden;
        info.CreateNoWindow = true;
      }

      return info;
    }
    public static ProcessStartInfo ShutdownStartInfo => new ProcessStartInfo
    {
      FileName = "shutdown.exe",
      Arguments = "/s /f /t 0",
      UseShellExecute = false,
      ErrorDialog = false,
    };
    public static ProcessStartInfo RebootStartInfo => new ProcessStartInfo
    {
      FileName = "shutdown.exe",
      Arguments = "/r /f /t 0",
      UseShellExecute = false,
      ErrorDialog = false,
    };
  }
}