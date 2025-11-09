using Common;
using EUMC.ServerServices;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UIControls;

namespace EUMC.Server
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    //MainViewModel M = null;
    ServerService SM;
    UserServiceHandler UserService;
    public MainWindow()
    {
      InitializeComponent();
      LOG.Initialize("server.txt");
      UIContextHelper.Initialize();

#if EUMC_SEOUL
      string hspCode = "01";
      this.Title = "EUMC Seoul Server";
#elif EUMC_MOKDONG
      string hspCode = "02";
      this.Title = "EUMC Mokdong Server";
#endif

      SM = new ServerService(hspCode);
      UserService = this.SM.UserService;

      this.Closing += (s, e) =>
      {
        this.SM.Stop();
      };

      try
      {
        this.SM.Start();
        //this.DataContext = M = new MainViewModel(sm);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        //this.ShortcutBinding();
      }
      catch (Exception e)
      {
        LOG.except(e);
        this.Close();
      }
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
      var btn = sender as Button;
      if (btn == null) return;

      switch (btn.Content)
      {
        case "Restart":
          this.UserService.SendAll(SERVER_CMD_NOTI.ClientRestart);
          break;
        case "Update":
          this.UserService.SendAll(SERVER_CMD_NOTI.ClientUpdate);
          break;
        case "Shutdown":
          this.UserService.SendAll(SERVER_CMD_NOTI.ClientShutdown);
          break;
        case "PlaylistUpdate":
          this.UserService.SendAll(SERVER_CMD_NOTI.ClientConfigUpdate);
          break;
        case "CallMessage":
          {
            var msg = new CALL_PATIENT_NOTI()
            {
              PatientName = "맹*이",
              PatientNameTTS = "맹꽁이",
              CallMessage = "폐기종 검사실 및 진료실 안으로 빨리 들어오십시요",
              Package = PACKAGE.NONE,
            };
            this.UserService.SendAll(msg);
          }
          break;
        case "OperationMessage":
          {
            var msg = new CALL_OPERATION_NOTI()
            {
              CallMessage1 = "홍*길동님 수술이 종료되어 중환자실로 이동합니다",
              CallMessage2 = "보호자분은 수술실 입구로 오시기 바랍니다"
            };
            this.UserService.SendAll(msg);
          }
          break;
        case "Reload":
          this.UserService.SendAll(SERVER_CMD_NOTI.ClientReloadData);
          break;
        case "Announce":
          {
            var msg = new CALL_ANNOUNCE_NOTI("홍길동님 진료실 1번으로 들어오십시요");
            this.UserService.SendAll(msg);
          }
          break;
        case "Bell":
          {
            var msg = new CALLL_BELL_NOTI("nono");
            this.UserService.SendAll(msg);
          }
          break;
        case "DisconnectAll":

          break;
      }
    }
    private void Package_Test(object sender, RoutedEventArgs e)
    {
      var btn = sender as Button;
      if (btn == null) return;

      var package = btn.Content.ToString();
      if (!string.IsNullOrEmpty(package))
      {
        //this.this.UserService.RunPackage(package);
      }
    }
    //void ShortcutBinding()
    //{
    //  this.InputBindings.Clear();

    //  var list = M.GetShortcutsCommand();
    //  foreach (var cmd in list)
    //  {
    //    if (cmd.Gesture != null)
    //    {
    //      this.InputBindings.Add(new KeyBinding(cmd, cmd.Gesture) { CommandParameter = cmd.FunctionId });
    //    }
    //    else
    //    {
    //      Debug.WriteLine($"{cmd.Header} is not shortcut command");
    //    }
    //  }
    //}

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      LOG.Error("CurrentDomain_UnhandledException " + ((Exception)e.ExceptionObject).Message + " Is Terminating: " + e.IsTerminating.ToString());
      LOG.Error(except((Exception)e.ExceptionObject));
    }
    static string except(Exception ex)
    {
      if (ex != null)
      {
        StringBuilder sb = new StringBuilder();
        exception(ex, sb);
        return sb.ToString();
      }
      return string.Empty;
    }
    static void exception(Exception ex, StringBuilder sb)
    {
      if (ex == null) return;

      sb.AppendLine($"Exception");
      sb.AppendLine($"{ex.GetType().FullName}");
      sb.AppendLine($"{ex.Message}");
      var src = ex.TargetSite == null || ex.TargetSite.DeclaringType == null ? ex.Source : string.Format("{0}.{1}", ex.TargetSite.DeclaringType.FullName, ex.TargetSite.Name);
      sb.AppendLine($"Source : {src}");

      if (ex.InnerException == null)
      {
        sb.AppendLine("StackTrace");
        sb.AppendLine(ex.StackTrace);
      }
      else
      {
        exception(ex.InnerException, sb);
      }
    }
  }
}