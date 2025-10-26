using UIControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace EUMC.Server
{
  public partial class MainViewModel : ViewModelBase
  {
    public event EventHandler OnRequestClose;
    public MainMenuManager MAINMENU { get; set; }
    //public ContextMenuManager CONTEXTMENU { get; set; }
    FunctionCollection<GROUP_ID, FUNC_ID> Functions;
    MenuSyncManager SYNC = new MenuSyncManager();
    List<FunctionCommand<GROUP_ID, FUNC_ID>> COMMANDS;
    List<FunctionCommand<GROUP_ID, FUNC_ID>> SHORTCUT_COMMANDS;

    void MainMenuInitialize()
    {
      CommandHelper.Initialize();
      Functions = FunctionCollectionCreator.CreateFunctionCollection<GROUP_ID, FUNC_ID>();

      var layout = CreateMainMenuLayout();
      MAINMENU = new MainMenuManager(Functions, layout);
      MAINMENU.SetObserver(SYNC);

      //var context = CreateContextMenuLayout();
      //CONTEXTMENU = new ContextMenuManager(Functions, context);

      this.CommandInitialize();
    }
    //private ContextMenuLayout CreateContextMenuLayout()
    //{
    //  var layout = new ContextMenuLayout();

    //  var bandId = CONTEXT_BAND_ID.CLIENT_MANAGE;
    //  var manage = layout.CreateBandItem(bandId, CommandHelper.BandName(bandId));
    //  {
    //    manage.Add(layout.MenuItem(ClientUpdateCommand));
    //    manage.Add(layout.MenuItem(ClientOnCommand));
    //    manage.Add(layout.MenuItem(ClientOffCommand));
    //    layout.AddBand(manage);
    //  }
    //  return layout;
    //}
    void CommandInitialize()
    {
      COMMANDS = new List<FunctionCommand<GROUP_ID, FUNC_ID>>();

      #region Commands
      COMMANDS.Add(ExitCommand);
      COMMANDS.Add(UpdateAllClientCommand);
      COMMANDS.Add(ClientVersionMatchCommand);
      COMMANDS.Add(LogSettingCommand);
      #endregion Commands

      SHORTCUT_COMMANDS = new List<FunctionCommand<GROUP_ID, FUNC_ID>>();
      foreach (var cmd in COMMANDS)
      {
        if (cmd.Gesture != null)
          SHORTCUT_COMMANDS.Add(cmd);
      }
    }

    public List<FunctionCommand<GROUP_ID, FUNC_ID>> GetShortcutsCommand()
    {
      return SHORTCUT_COMMANDS;
    }

    void ExecuteCommand(FUNC_ID funcId, FunctionItemBase<GROUP_ID, FUNC_ID> item)
    {
      string header = item != null ? item.Header : "call by hotkey";
      Debug.WriteLine($"ID:{funcId}, Header={header}");

      //if (item == null)
      //{
      //  var info = this.Functions.GetFunction(funcId);
      //  SYNC.ExternalAction(funcId);
      //}

      switch (funcId)
      {
        case FUNC_ID.EXIT:
          this.action_exit();
          break;
        case FUNC_ID.CLIENT_UPDATE_ALL:
          {
            if (MessageBoxResult.Yes == MessageBox.Show("모든 클라이언트들을 업데이트 하시겠습니까?", "클라이언트 프로그램 업데이트", MessageBoxButton.YesNoCancel, MessageBoxImage.Information))
            {
              //Service.AllClientUpdate();
            }
          }
          break;
        case FUNC_ID.SETTINGS:
          {
            var win = new SettingsView() { Owner = UIContextHelper.WindowHandle };
            win.Show();
            break;
          }
        case FUNC_ID.CLIENT_VERSION_MATCH:
          {
            if (MessageBoxResult.Yes == MessageBox.Show("클라이언트 버전 일치 작업을 진행 하시겠습니까?", "클라이언트 프로그램 버전 업데이트", MessageBoxButton.YesNoCancel, MessageBoxImage.Information))
            {
              //Service.AllClientVersionMatch();
            }
          }
          break;
      }
    }

    public bool action_exit()
    {
      if (MessageBoxResult.Yes == MessageBox.Show("서비스 매니저를 종료 하시겠습니까?", "프로그램 종료", MessageBoxButton.YesNoCancel, MessageBoxImage.Information))
      {
        //Task.Run(() => Service.Stop());
        //Service.Stop();
        OnRequestClose?.Invoke(this, EventArgs.Empty);
        return true;
      }
      return false;
    }

    MainMenuLayout CreateMainMenuLayout()
    {
      var dic = new Dictionary<MAINMENU_BAND_ID, List<FunctionCommand<GROUP_ID, FUNC_ID >>>();
      {
        // files
        {
          var commands = new List<FunctionCommand<GROUP_ID, FUNC_ID>>()
          {
            ExitCommand,
          };
          dic.Add(MAINMENU_BAND_ID.FILES, commands);
        }
        // 관리
        {
          var commands = new List<FunctionCommand<GROUP_ID, FUNC_ID>>()
          {
            UpdateAllClientCommand,
            ClientVersionMatchCommand,
            LogSettingCommand
          };
          dic.Add(MAINMENU_BAND_ID.MANAGEMENT, commands);
        }
      }

      //string uri = null;
      //{
      //  var asm = Assembly.GetExecutingAssembly().GetName().Name;
      //  uri = CommandHelper.GetURIName(asm, @"ToolBar/Icons");
      //}
      //var icons = new MainMenuIconFactory(uri);
      var layout = new MainMenuLayout();

      foreach (var item in dic)
      {
        var bandId = item.Key;
        var commands = item.Value;

        var band = layout.CreateBandItem(bandId, CommandHelper.BandName(bandId));

        foreach (var cmd in commands)
        {
          if (cmd != null)
          {
            band.Add(layout.MenuItem(cmd));
          }
          else
          {
            band.Add(layout.Seperator());
          }
        }
        layout.AddBand(band);
      }

      return layout;
    }

    #region Menu Command
    FunctionCommand<GROUP_ID, FUNC_ID> _exitCommand;
    FunctionCommand<GROUP_ID, FUNC_ID> _clientUpdateAllCommand;
    FunctionCommand<GROUP_ID, FUNC_ID> _clientVersionMatchCommand;
    FunctionCommand<GROUP_ID, FUNC_ID> _logSettingCommand;

    public FunctionCommand<GROUP_ID, FUNC_ID> ExitCommand => _exitCommand ?? (_exitCommand
                    = CommandHelper.Create(FUNC_ID.EXIT, (id, item) => ExecuteCommand(id, item)));

    public FunctionCommand<GROUP_ID, FUNC_ID> UpdateAllClientCommand => _clientUpdateAllCommand ?? (_clientUpdateAllCommand
                    = CommandHelper.Create(FUNC_ID.CLIENT_UPDATE_ALL, (id, item) => ExecuteCommand(id, item)));

    public FunctionCommand<GROUP_ID, FUNC_ID> ClientVersionMatchCommand => _clientVersionMatchCommand ?? (_clientVersionMatchCommand
                = CommandHelper.Create(FUNC_ID.CLIENT_VERSION_MATCH, (id, item) => ExecuteCommand(id, item)));

    public FunctionCommand<GROUP_ID, FUNC_ID> LogSettingCommand => _logSettingCommand ?? (_logSettingCommand
                = CommandHelper.Create(FUNC_ID.SETTINGS, (id, item) => ExecuteCommand(id, item)));
    #endregion Menu Command

    #region Context Menu (Group)
    public RelayCommand GroupUpdateCommand => CommandActivator.When(o => GroupUpdateCommandCan(o))
                                                   .Exec(o => GroupUpdateCommandAction(o), "Group Update");
    bool GroupUpdateCommandCan(object o)
    {
      return false; // has_children(o as PortGroup);
    }
    //bool has_children(PortGroup o)
    //{
    //  return o != null ? o.Sessions.Count > 0 : false;
    //}
    void GroupUpdateCommandAction(object o)
    {
      //var group = o as PortGroup;

      //if (group != null)
      //{
      //  if (MessageBoxResult.Yes == MessageBox.Show("현재 그룹의 모든 클라이언트들을 업데이트 하시겠습니까?", "클라이언트 그룹 프로그램 업데이트", MessageBoxButton.YesNoCancel, MessageBoxImage.Information))
      //  {
      //    Service.GroupClientUpdate(group.ServicePort);
      //  }
      //}
    }

    public RelayCommand GroupScreenOnCommand => CommandActivator.When(o => GroupScreenOnCommandCan(o))
                                                   .Exec(o => GroupScreenOnCommandAction(o), "Group Screen On");
    bool GroupScreenOnCommandCan(object o)
    {
      return false;// has_children(o as PortGroup);
    }
    void GroupScreenOnCommandAction(object o)
    {
      //var group = o as PortGroup;
      //if (group != null)
      //{
      //  Service.ScreenGroupCommand(true, group.ServicePort);
      //}
    }

    public RelayCommand GroupScreenOffCommand => CommandActivator.When(o => GroupScreenOffCommandCan(o))
                                                   .Exec(o => GroupScreenOffCommandAction(o), "Group Screen Off");
    bool GroupScreenOffCommandCan(object o)
    {
      return false; // has_children(o as PortGroup);
    }
    void GroupScreenOffCommandAction(object o)
    {
      //var group = o as PortGroup;

      //if (group != null)
      //{
      //  Service.ScreenGroupCommand(false, group.ServicePort);
      //}
    }
    #endregion Context Menu (Group)

    #region Context Menu (Client)
    public RelayCommand ClientUpdateCommand => CommandActivator.Exec(o => ClientUpdateCommandAction(o), "Update");
    void ClientUpdateCommandAction(object o)
    {
      //var session = o as EumcHostSession;
      //if (session != null)
      //{
      //  Service.ClientUpdate(session);
      //}
    }
    public RelayCommand ClientUpdateForcedCommand => CommandActivator.Exec(o => ClientUpdateForcedCommandAction(o), "Update (Forced)");
    void ClientUpdateForcedCommandAction(object o)
    {
      //var session = o as EumcHostSession;
      //if (session != null)
      //{
      //  Service.ClientUpdateForced(session);
      //}
    }
    public RelayCommand ConnectVncCommand => CommandActivator.Exec(o => ConnectVncCommandAction(o), "Connect VNC");
    void ConnectVncCommandAction(object o)
    {
      //var session = o as EumcHostSession;
      //if(session != null)
      //{
      //  var vnc = ServerConfigHelper.Settings.VNC;

      //  if(File.Exists(vnc.PATH))
      //  {
      //    var ip = session.RemoteIP;

      //    string param = $"-password {vnc.PASSWORD} {ip}";
      //    Process.Start(vnc.PATH, param);
      //  }
      //}
    }
    //public RelayCommand PhotoUpdateCommand => CommandActivator.Exec(o => PhotoUpdateCommandAction(o), "Photo Update");
    //void PhotoUpdateCommandAction(object o)
    //{
    //  var session = o as EumcHostSession;
    //  if (session != null)
    //  {
    //    Service.PhotoUpdate(session);
    //  }
    //}
    public RelayCommand RestartCommand => CommandActivator.Exec(o => RestartCommandAction(o), "Restart");
    void RestartCommandAction(object o)
    {
      //var session = o as EumcHostSession;
      //if (session != null)
      //{
      //  Service.RestartClient(session);
      //}
    }
    public RelayCommand RestartAllCommand => CommandActivator.Exec(o => RestartAllCommandAction(o), "Restart All (same package)");
    void RestartAllCommandAction(object o)
    {
      //var session = o as EumcHostSession;
      //if (session != null)
      //{
      //  Service.RestartAll(session);
      //}
    }
    public RelayCommand RebootCommand => CommandActivator.Exec(o => RebootCommandAction(o), "Reboot");
    void RebootCommandAction(object o)
    {
      //var session = o as EumcHostSession;
      //if (session != null)
      //{
      //  Service.RebootClient(session);
      //}
    }
    public RelayCommand ScreenOnCommand => CommandActivator.Exec(o => ScreenOnCommandAction(o), "ScreenOn");
    void ScreenOnCommandAction(object o)
    {
      //var session = o as EumcHostSession;
      //if (session != null)
      //{
      //  Service.ScreenCommand(session, true);
      //}
    }
    public RelayCommand ScreenOffCommand => CommandActivator.Exec(o => ScreenOffCommandAction(o), "ScreenOff");
    void ScreenOffCommandAction(object o)
    {
      //var session = o as EumcHostSession;
      //if (session != null)
      //{
      //  Service.ScreenCommand(session, false);
      //}
    }
    public RelayCommand ShutdownCommand => CommandActivator.Exec(o => ShutdownCommandAction(o), "Shutdown");
    void ShutdownCommandAction(object o)
    {
      //var session = o as EumcHostSession;
      //if (session != null)
      //{
      //  Service.ShutdownClient(session);
      //}
    }
    public RelayCommand UpdatePlaylistCommand => CommandActivator.Exec(o => UpdatePlaylistCommandAction(o), "UpdatePlaylist");
    void UpdatePlaylistCommandAction(object o)
    {
      //var session = o as EumcHostSession;
      //if (session != null)
      //{
      //  Service.UpdatePlaylistClient(session);
      //}
    }

    #endregion Context Menu (Client)
  }
}