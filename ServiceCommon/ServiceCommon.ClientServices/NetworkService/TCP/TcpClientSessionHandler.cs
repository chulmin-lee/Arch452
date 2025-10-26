using System.Net;

namespace ServiceCommon.ClientServices
{
  public class TcpClientSessionHandler : ClientSessionHandlerBase, IClientSessionHandler
  {
    public TcpClientSessionHandler(IClientService owner, string ip, int port)
      : base(owner)
    {
      var remote = new IPEndPoint(IPAddress.Parse(ip), port);
      this.Session = new TcpClientSession(this, remote);
    }
  }
}