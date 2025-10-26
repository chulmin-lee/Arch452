using Framework.Network;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class ClientServiceImpl : ClientServiceBase
  {
    public ClientServiceImpl(IClientViewManager vm) : base(vm, true)
    {
    }
    protected override ServerConfig get_service_config()
    {
      return KeyXml.Load("key.xml");
    }
    protected override ILocation get_location(ServerConfig config)
    {
      return new Location(config, "ClientUpdater.exe");
    }
    protected override IPackageViewConfig create_package_view(PlaylistSchedule s)
    {
      return PackageViewConfigFactory.Create(s);
    }
    protected override IPackageViewConfig create_updater_package_view()
    {
      return new UpdaterViewConfig(this.ClientView.ClientVersion);
    }
    protected override IClientUpdater get_client_updater()
    {
      return new ClientUpdater(this.Location, extract_zip_file: true);
    }
    protected override IPlaylistUpdater get_playlist_updater()
    {
      return new PlaylistUpdater(this.Location);
    }
    protected override IClientSessionHandler create_client_session_handler(bool grpc)
    {
      return CLIENT_SESSION_HANDLER.Tcp(this, ServerConfig);
    }
    protected override ClientConnectionOption get_connection_option()
    {
      return new ClientConnectionOption
      {
        RetryConnecting = true,
        ConnectingInterval = 5,
        RecoverConnectionLost = true,
        RecoverInterval = 5
      };
    }
  }
}