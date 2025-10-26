using Framework.Network;

namespace ServiceCommon.ClientServices
{
  public interface IClientService
  {
    int ClientId { get; }
    ClientConnectionOption ConnectionOption { get; }
    void SendMessage(ServiceMessage message);
    void Restart();
    void Start();
    void Close();

    // Session handler callback
    void OnMessageReceived(ServiceMessage m);
    void OnStateChanged(NetworkState state);
  }
}