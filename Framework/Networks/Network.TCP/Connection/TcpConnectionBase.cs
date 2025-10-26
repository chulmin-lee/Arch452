using Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Framework.Network.TCP
{
  /// <summary>
  /// 모든 TCP 연결은 이 클래스를 인스턴스화하여 사용한다.
  /// - 다양한 메시지 포멧은 전달되는 MessageConverter가 처리한다.
  /// TCP의 W/R은 항상 byte array 이다
  /// </summary>
  /// <typeparam name="M"></typeparam>
  public abstract class TcpConnectionBase<M> : NetworkConnection<M>
    where M : class, new()
  {
    protected Socket Socket;
    protected IPEndPoint Remote;
    SocketAsyncEventArgs SAEA;
    IMessageConverter<M, ArraySegment<byte>, ArraySegment<byte>> Converter;

    protected TcpConnectionBase(IConnectionOwner<M> owner,
                                IMessageConverter<M, ArraySegment<byte>, ArraySegment<byte>> converter,
                                NetworkState init_state)
      : base(owner, init_state)
    {
      this.Converter = converter;
    }
    public void SocketConnected(Socket socket)
    {
      this.Socket = socket;
      this.Remote = socket.RemoteEndPoint as IPEndPoint;
      this.NetworkId = $"{this.Remote?.Address}:{this.Remote?.Port}";

      this.SAEA = SaeaPoolHelper.GetSaea();
      this.SAEA.UserToken = socket;
      this.SAEA.Completed += OnReceiveCompleted;

      if (this.ChangeState(NetworkState.Online))
      {
        this.ConnectionOwner.OnStateChanged(this.State);
      }
      this.start_receive();
    }

    #region Send
    protected override async Task send_impl(M m)
    {
      if (this.IsOnline && this.Socket != null)
      {
        var s = this.Converter.SendMessage(m);

        // 예외가 발생하면 위에서 받는다
        //this.Socket.SendAsync(s);
        await Task.Run(() => this.Socket.Send(s.Array, s.Offset, s.Count, SocketFlags.None));
      }
    }
    #endregion Send

    #region receive
    void start_receive()
    {
      if (this.IsOnline && Socket != null && SAEA != null)
      {
        try
        {
          if (!Socket.ReceiveAsync(SAEA))
          {
            OnReceiveCompleted(this, SAEA);
          }
        }
        catch (Exception ex)
        {
          this.OnConnectionLost(ex.Message);
        }
      }
    }
    void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
    {
      // 여기서 SocketAsyncEventArgs 예외가 발생할 수 있다.
      // SAEA 과 socket이 맞지 않는 경우
      // - An asynchronous socket operation is already in progress using this SocketAsyncEventArgs instance.
      // 해결책
      // - 다시 연결할때마다 Connection 재생성밖에 답이 없네

      if (e.SocketError == SocketError.Success && e.BytesTransferred > 0 && e.Buffer != null)
      {
        var raw = new ArraySegment<byte>(e.Buffer, e.Offset, e.BytesTransferred);

        var msg = this.Converter.ReadMessage(raw);
        while (msg != null)
        {
          this.OnReceive(msg);
          // 중첩 메시지 검사
          msg = this.Converter.ReadMessage();
        }
        this.start_receive();
      }
      else
      {
        this.OnConnectionLost($"OnReceiveCompleted() : code={e.SocketError},read={e.BytesTransferred}");
      }
    }
    #endregion receive

    protected override void close_impl()
    {
      if (this.Socket != null)
      {
        LOG.dc($"TcpConnection close - {this.NetworkId}");
        try
        {
          this.Socket.Shutdown(SocketShutdown.Both);
        }
        catch (SocketException ex)
        {
          LOG.ec($"Error shutting down socket: {ex.Message}");
        }
        finally
        {
          this.Socket.Close();
          this.Socket = null;

          if (this.SAEA != null)
          {
            this.SAEA.UserToken = null;
            this.SAEA.Completed -= OnReceiveCompleted;
            SaeaPoolHelper.ReturnSaea(this.SAEA);
            this.SAEA = null;
          }
        }
      }
    }
  }
}