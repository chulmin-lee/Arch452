using Common;
using Framework.Network.TCP;
using System.Net.Sockets;

namespace ServiceCommon.ServerServices
{
  public class TcpServerSession : ServerSessionBase
  {
    public TcpServerSession(IServerSessionOwner owner, Socket socket, int key)
      : base(key, owner)
    {
      var con = new TcpServerConnection<ServiceMessage>(this, TcpMessageConverter.ServerConverter);
      this.Connection = con;

      // Connection이 할당되기 전에 receive가 시작될 수 있으므로 Connection 할당 후에 SocketConnected 호출
      con.SocketConnected(socket);
    }

    public override void OnMessageReceived(ServiceMessage m)
    {
      if (m.ServiceId == SERVICE_ID.CLIENT_REGISTER)
      {
        var reg = m.CastTo<CLIENT_REGISTER_REQ>();
        this.PackageInfo = reg.PackageInfo;
        this.ClientInfo = reg.ClientInfo;
      }
      base.OnMessageReceived(m);
    }
  }
}