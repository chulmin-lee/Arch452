using Common;
using EUMC.ClientServices;
using ServiceCommon;
using ServiceCommon.ClientServices;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using UIControls;

namespace EUMC.Client
{
  public class ClientViewManager : IClientViewManager //, IWindowHelper
  {
    public string ClientPath { get; private set; }
    public string ClientVersion { get; private set; }
    public string ClientRestartArg => ClientViewManager.RestartArgument;
    public static string RestartArgument = "--restart";

#if DEBUG
    public static int RotationInterval = 2;
#else
  public static int RotationInterval = 10;
#endif
    IWindowViewModel WindowVM;

    Window MainWindow
    {
      get => Application.Current.MainWindow;
      set => Application.Current.MainWindow = value;
    }

    public ClientViewManager(string hspCode)
    {
      this.ClientVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString() ?? "0.0.0.0";
      this.ClientPath = Process.GetCurrentProcess().MainModule?.FileName ?? throw new Exception("no main module");

      CLIENT_SERVICE.Initialize(hspCode, this);

      this.WindowVM = new CustomWindowViewModel();
      this.MainWindow = new CustomWindow(WindowVM);
      this.MainWindow.Show();
    }
    void ShowCustomWindow()
    {
      if (!this.WindowVM.IsCustomWindow)
      {
        var prev = this.MainWindow;
        this.WindowVM = new CustomWindowViewModel();
        this.MainWindow = new CustomWindow(this.WindowVM);
        this.MainWindow.Show();
        prev.Close();
      }
    }
    void ShowContentWindow()
    {
      if (this.WindowVM.IsCustomWindow)
      {
        var prev = this.MainWindow;
        this.WindowVM = new ContentWindowViewModel();
        this.MainWindow = new ContentWindow(this.WindowVM);
        this.MainWindow.Closing += (s, e) => CLIENT_SERVICE.Close();
        this.MainWindow.Show();
        prev.Close();
      }
    }

    public void ConfigChanged(IPackageViewConfig o)
    {
      UIContextHelper.CheckInvokeOnUI(() =>
      {
        switch (o.WindowType)
        {
          case WindowScreenType.Custom:
            this.ShowCustomWindow();
            break;
          default:
            this.ShowContentWindow();
            break;
        }
        this.WindowVM.ConfigChanged(o);
      });
    }
    public void ReceiveMessage(ServiceMessage m)
    {
      LOG.ic($"{m.ServiceId}");
      UIContextHelper.CheckBeginInvokeOnUI(() =>
      {
        this.WindowVM.ReceiveMessage(m);
      });
    }
    public void Exit()
    {
      LOG.ic("Close WindowManager");

      UIContextHelper.CheckInvokeOnUI(() =>
      {
        this.WindowVM.Close();
        this.MainWindow.Close();
      });
    }

    public void Start()
    {
      CLIENT_SERVICE.Start();
    }
    public void Restart()
    {
      LOG.ic("Restart");
      CLIENT_SERVICE.Restart();
    }
    public void Close()
    {
      LOG.ic("Close");
      //this.WindowVM.Close();
      //this.MainWindow.Close();
      //CLIENT_SERVICE.Close();
    }
  }
}