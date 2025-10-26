using Common;
using System;
using System.Net;
using System.Net.Sockets;

namespace Framework.Network.TCP
{
  public class TcpClientConnection<M> : TcpConnectionBase<M>
    where M : class, new()
  {
    public TcpClientConnection(IConnectionOwner<M> owner, IMessageConverter<M, ArraySegment<byte>, ArraySegment<byte>> converter, IPEndPoint ep)
      : base(owner, converter, NetworkState.Offline)
    {
      this.Remote = ep ?? throw new ArgumentNullException(nameof(ep));
    }

    #region Connect
    public override void Connect()
    {
      if (this.Remote == null)
      {
        throw new InvalidOperationException("Remote endpoint is not set.");
      }
      LOG.ic($"Connect to {this.Remote.Address}:{this.Remote.Port}");

      var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      var conn_saea = new SocketAsyncEventArgs();
      conn_saea.Completed += on_connect_completed;
      conn_saea.RemoteEndPoint = this.Remote;

      if (!socket.ConnectAsync(conn_saea))
      {
        on_connect_completed(null, conn_saea);
      }
    }
    void on_connect_completed(object sender, SocketAsyncEventArgs e)
    {
      try
      {
        if (e.SocketError == SocketError.Success && e.ConnectSocket != null)
        {
          this.SocketConnected(e.ConnectSocket);
        }
        else
        {
          this.ConnectionOwner.OnStateChanged(NetworkState.ConnectionFailed);
        }
      }
      finally
      {
        // 연결에 사용한 SAEA는 더 이상 사용하지 않으므로 제거한다.
        e.Completed -= on_connect_completed;
        e.Dispose();
      }
    }
    #endregion Connect

    #region State test
    /*
    protected void StateInitialize()
    {
      // connecting
      {
        var s = new ConnectingState();
        this.States.Add(s.State, s);
      }
    }

    class OfflineState : ConnectionStateBase
    {
      public OfflineState() : base(NetworkState.Offline)
      {
      }

      public override void Connect()
      {
        // 결과에 따라, Online, ConnectionFailed 상태로 이동
      }
    }

    class ConnectingState : ConnectionStateBase
    {
      public ConnectingState() : base(NetworkState.Connecting)
      {
      }
    }
    */
    #endregion State test
  }
}