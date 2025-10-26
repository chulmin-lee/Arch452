using Common;
using System;
using System.Linq;
using System.Windows;

namespace EUMC.Client
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    ClientViewManager _wm;

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      LOG.Initialize("client.txt");

      var restart = Environment.GetCommandLineArgs().Contains(ClientViewManager.RestartArgument);
      if (!restart && ProcessHelper.IsAlreadyRunning())
      {
        LOG.e("already running");
        Current.Shutdown();
      }
      else
      {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        _wm = new ClientViewManager();
        _wm.Start();
      }
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      LOG.except((Exception)e.ExceptionObject);
      _wm?.Restart();
    }
  }
}