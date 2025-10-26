using Common;
using System.Windows;

namespace EUMC.Server
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      if (ProcessHelper.IsAlreadyRunning())
      {
        Current.Shutdown();
      }
    }
  }
}