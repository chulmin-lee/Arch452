using Framework.Network;

namespace ServiceCommon.ClientServices
{
  public interface IClientSessionOwner
  {
    ClientConnectionOption Option { get; }
    void OnMessageReceived(ServiceMessage message);
    void OnStateChanged(NetworkState state);
  }
}