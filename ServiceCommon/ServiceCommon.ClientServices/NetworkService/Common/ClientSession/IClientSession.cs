using Framework.Network;

namespace ServiceCommon.ClientServices
{
  public interface IClientSession : INetworkSession<ServiceMessage>
  {
    void Connect(ServiceMessage connect_message);
    void Connect();
  }
}