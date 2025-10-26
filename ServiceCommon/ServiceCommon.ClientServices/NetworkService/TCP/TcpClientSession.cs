using Framework.Network;
using Framework.Network.TCP;
using System.Net;

namespace ServiceCommon.ClientServices
{
  public class TcpClientSession : ClientSessionBase, IClientSession
  {
    IPEndPoint Remote;
    public TcpClientSession(IClientSessionOwner owner, IPEndPoint ep) : base(owner)
    {
      this.Remote = ep;
    }

    protected override INetworkConnection<ServiceMessage> CreateClientConnection()
    {
      return new TcpClientConnection<ServiceMessage>(this, TcpMessageConverter.ClientConverter, this.Remote);
    }
  }
}