using Common;
using System.Collections.Generic;
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
      else
      {
        //MaskedNameHelper.Initialize(MaskPosition.Right, 0.5, new List<string> { "아기" });
      }
    }
  }
}