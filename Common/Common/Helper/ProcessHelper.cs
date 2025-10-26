using System.Diagnostics;

namespace Common
{
  public static class ProcessHelper
  {
    public static string ProcessName => Process.GetCurrentProcess().ProcessName;
    public static bool IsAlreadyRunning() => Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1;

    public static void Shutdown()
    {
      var psi = new ProcessStartInfo
      {
        FileName = "shutdown.exe",
        Arguments = "/s /f /t 0",
        UseShellExecute = false,
        ErrorDialog = false,
      };
      Process.Start(psi);
    }
    public static void Reboot()
    {
      LOG.w("SystemReboot");
      var psi = new ProcessStartInfo
      {
        FileName = "shutdown.exe",
        Arguments = "/r /f /t 0",
        UseShellExecute = false,
        ErrorDialog = false,
      };
      Process.Start(psi);
    }
    public static void Kill()
    {
      Process.GetCurrentProcess().Kill();
    }
  }
}