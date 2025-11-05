using Common;
using System.Diagnostics;

namespace ServiceCommon.ClientServices
{
  public abstract partial class ClientServiceBase : IClientService
  {
    protected virtual void restart()
    {
      // 현재 프로그램을 재실행
      LOG.ic("restarting client");
      this.exit(this.ClientView.ClientPath, this.ClientView.ClientRestartArg);
    }
    protected virtual void reboot()
    {
#if !DEBUG
      if (this.CurrentPackage != null && this.CurrentPackage.CanReboot)
      {
        LOG.ic("rebooting client");
        this.exit("shutdown", "/r /f /t 0");
      }
#endif
    }
    protected virtual void shutdown()
    {
#if !DEBUG
      if (this.CurrentPackage != null && this.CurrentPackage.CanReboot)
      {
        LOG.ic("shutting down client");
        this.exit("shutdown", "/s /f /t 0");
      }
#endif
    }
    void client_update(string path)
    {
      var arg = $"{path} {this.ClientView.ClientPath}";
      this.exit(this.Location.UpdaterPath, arg);
    }

    void exit(string exe, string args)
    {
      LOG.ic($"exiting client: {exe} {args}");

      var info = new ProcessStartInfo
      {
        FileName = exe,
        Arguments = args,
        UseShellExecute = true,
        ErrorDialog = false,
      };

      Process.Start(info);

      this.ClientView.Exit();
      this.Close();
    }
  }
}