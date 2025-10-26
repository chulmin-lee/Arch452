using Common;
using Framework.Network.TCP;
using System.Net.Sockets;

namespace ServiceCommon.ServerServices
{
  public class TcpServerSessionHandler : ServerSessionHandlerBase
  {
    TcpAcceptor _acceptor;
    public TcpServerSessionHandler(IServerService manager, int accept_port) : base(manager)
    {
      _acceptor = new TcpAcceptor(accept_port);
      _acceptor.ClientAccepted += (s, e) => socket_connected(e);
    }

    void socket_connected(Socket socket)
    {
      LOG.ic("client socket connected");
      var session = new TcpServerSession(this, socket, this.NextKey);
    }
    public override void OnMessageReceived(IServerSession session, ServiceMessage message)
    {
      if (message.ServiceId == SERVICE_ID.CLIENT_REGISTER)
      {
        this.OnSessionConnected(session);
      }
      else
      {
        base.OnMessageReceived(session, message);
      }
    }

    public override void Start()
    {
      LOG.ic("start session handler");
      _acceptor.Start();
      base.Start();
    }

    public override void Stop()
    {
      LOG.ic("stop session handler");
      _acceptor?.Stop();
      base.Stop();
    }
  }
}