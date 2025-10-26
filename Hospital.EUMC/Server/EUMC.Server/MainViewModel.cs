using UIControls;
using System.Reflection;
using EUMC.ServerServices;

namespace EUMC.Server
{
  public partial class MainViewModel : ViewModelBase
  {
    public ServerService Service { get; private set; }
    public string Version { get; set; } = string.Empty;
    public MainViewModel(ServerService service)
    {
      this.Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
      Service = service;
      //this.Service.SessionChanged += (s, e) => { this.ClientCount = e; };
      this.MainMenuInitialize();
    }

    public void PlaylistUpdate()
    {
      //Service.PlaylistUpdate();
    }

    int _connections;
    public int Connections { get { return _connections; } set { Set(ref _connections, value); } }

    int _clientCount;
    public int ClientCount { get { return _clientCount; } set { Set(ref _clientCount, value); } }
  }
}