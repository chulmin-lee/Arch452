using Common;
using Framework.Network;
using Framework.Network.HTTP;
using System.Threading.Tasks;

namespace ServiceCommon.ClientServices
{
  public abstract partial class ClientServiceBase : IClientService
  {
    public int ClientId => this.ServerConfig.ClientID;
    protected ServerConfig ServerConfig { get; set; }
    protected PackageConfigurations PackageConfig { get; set; }
    protected ILocation Location { get; private set; }

    public ClientConnectionOption ConnectionOption { get; private set; }

    protected IClientViewManager ClientView;
    protected IPlaylistUpdater PlaylistUpdater;

    protected IClientUpdater ClientUpdater;
    protected IClientSessionHandler SessionHandler;
    protected ScheduleBuiler<CLIENT_SCHEDULE> SCHEDULER;
    protected CLIENT_INFO ClientInfo;
    CLIENT_STATUS_REQ ClientStatus;
    protected WatchDogClient WatchDogClient;
    IPackageViewConfig CurrentPackage;

    public ClientServiceBase(IClientViewManager vm, bool grpc)
    {
      this.ClientView = vm;
      this.SCHEDULER = new ScheduleBuiler<CLIENT_SCHEDULE>();
      HttpDownloader.DownloadProgressReported += (s, e) => this.OnDownloadProgress(e);

      this.ServerConfig = this.get_service_config();
      this.Location = this.get_location(this.ServerConfig);
      this.WatchDogClient = new WatchDogClient(9999);
      this.ClientInfo = new CLIENT_INFO
      {
        ClientId = ServerConfig.ClientID,
        IPAddress = NetworkHelper.GetLocalIPAddress(),
        MacAddress = NetworkHelper.GetMacAddress(),
        Version = vm.ClientVersion,
        HospitalCode = "01"
      };
      this.ClientStatus = new CLIENT_STATUS_REQ
      {
        ClientID = ServerConfig.ClientID,
      };
      this.PlaylistUpdater = this.get_playlist_updater();
      this.ClientUpdater = this.get_client_updater();
      this.ConnectionOption = this.get_connection_option();
      this.SessionHandler = this.create_client_session_handler(grpc);
    }

    #region impl
    protected abstract ServerConfig get_service_config();
    protected abstract ILocation get_location(ServerConfig config);
    protected abstract IPlaylistUpdater get_playlist_updater();
    protected abstract IClientUpdater get_client_updater();
    protected abstract IPackageViewConfig create_package_view(PlaylistSchedule s);
    protected abstract IPackageViewConfig create_updater_package_view();
    protected abstract IClientSessionHandler create_client_session_handler(bool grpc);
    protected abstract ClientConnectionOption get_connection_option();
    #endregion impl
    void OnDownloadProgress(DownloadProgressReport e)
    {
      var m = new DownloadProgressMessage()
      {
        Index = e.Index,
        FileName = e.FileName,
        Percent = e.Percent,
        DownloadSpeed = e.DownloadSpeed,
        DownloadSize = e.DownloadSize,
        TimeLeft = e.TimeLeft
      };
      this.ClientView.ReceiveMessage(m);
    }

    public void Start()
    {
      this.WatchDogClient.Start();
      this.update_async(client: true, playlist: true);
    }

    protected virtual void update_async(bool client = false, bool playlist = false, bool forced = false)
    {
      if (!client && !playlist) return;

      Task.Run(async () =>
      {
        bool show_updater_view = false;
        // 스케쥴 끄기
        this.schedule_clear();

        var update_view = this.create_updater_package_view();
        // test: 항상 업데이트 뷰 출력
        //this.change_content_view(update_view);

        if (client)
        {
          var ver = await this.ClientUpdater.ClientVersionCheck();
          if (ver?.IsUpdated ?? false)
          {
            show_updater_view = true;
            this.change_content_view(update_view);
          }

          var patch = await this.ClientUpdater.ClientUpdate(forced);
          if (!string.IsNullOrEmpty(patch))
          {
            // 여기서 업데이트 명령 보내기
            //this.WatchDogClient.Update(patch);
            this.client_update(patch);
            return;
          }
        }

        if (playlist)
        {
          PackageConfig = await this.PlaylistUpdater.DownloadAsync();

          if (PackageConfig.Success)
          {
            if (PackageConfig.HasDownloadFiles)
            {
              if (!show_updater_view)
              {
                show_updater_view = true;
                this.change_content_view(update_view);
              }
              await PackageConfig.ContentDownloadAsync();
            }
          }
        }
        // 스케쥴 다시 초기화
        this.schedule_initialize();
        // 패키지 스케쥴
        this.schedule_package();
      });
    }

    public void Close()
    {
      this.WatchDogClient.Close();
      this.SCHEDULER.Close();
      this.SessionHandler.Close();
    }

    public virtual void OnMessageReceived(ServiceMessage m)
    {
      LOG.wc($"{m.ServiceId}");
      switch (m.ServiceId)
      {
        #region Server Command
        case SERVICE_ID.SVR_COMMAND:
          {
            var cmd = m.CastTo<SERVER_CMD_NOTI>().Command;
            LOG.dc($"server command: {cmd}");
            switch (cmd)
            {
              case SERVER_CMD_NOTI.SERVER_COMMAND.ConfigUpdated: this.update_async(playlist: true); ; break;
              case SERVER_CMD_NOTI.SERVER_COMMAND.ClientUpdated: this.update_async(client: true); ; break;
              case SERVER_CMD_NOTI.SERVER_COMMAND.ClientForcedUpdate: this.update_async(client: true, forced: true); ; break;
              case SERVER_CMD_NOTI.SERVER_COMMAND.Rollback:
              case SERVER_CMD_NOTI.SERVER_COMMAND.Reload: break;
              case SERVER_CMD_NOTI.SERVER_COMMAND.Restart: this.restart(); break;
              case SERVER_CMD_NOTI.SERVER_COMMAND.Reboot: this.reboot(); break;
              case SERVER_CMD_NOTI.SERVER_COMMAND.Shutdown: this.shutdown(); break;
            }
          }
          break;

        case SERVICE_ID.SVR_STATUS:
          {
            //var m = e.CastTo<SERVER_STATUS_RESP>();
            //LOG.dc($"code: {m.Response}");
            //if (m.Response == SERVER_STATUS_RESP.STATE.ERROR)
            //{
            //  LOG.ec($"server error - reconnect");
            //  _sessionHandler.Reconnect();
            //}
          }
          break;
        #endregion Server Command
        case SERVICE_ID.CLIENT_STATUS:
          LOG.dc(m.ServiceId);
          break;

        case SERVICE_ID.NONE:
          LOG.ic($"no response");
          break;

        default:
          {
            ClientView?.ReceiveMessage(m);
          }
          break;
      }
    }

    public virtual void OnStateChanged(NetworkState state)
    {
      LOG.dc($"network state: {state}");
    }
    public void SendMessage(ServiceMessage message)
    {
      this.SessionHandler.Send(message);
    }
    public void Restart()
    {
    }
  }
}