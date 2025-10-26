using Common;
using System;
using System.Net;
using System.Net.Sockets;

namespace Framework.Network.TCP
{
  public class TcpAcceptor : IClientAcceptor
  {
    public event EventHandler<Socket> ClientAccepted;

    Socket _listenSocket;
    bool _isRunning = false;
    public TcpAcceptor(int port)
    {
      _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      _listenSocket.Bind(new IPEndPoint(IPAddress.Any, port));
      _listenSocket.Listen(100);
    }

    public void Start()
    {
      var arg = new SocketAsyncEventArgs();
      arg.Completed += OnAcceptCompleted;
      _isRunning = true;
      this.StartAccept(arg);
    }
    public void Stop()
    {
      _isRunning = false;
      try { _listenSocket?.Close(); }
      catch (Exception) { }
    }
    void StartAccept(SocketAsyncEventArgs e)
    {
      if (!_isRunning) return;

      try
      {
        e.AcceptSocket = null;
        if (!_listenSocket.AcceptAsync(e))
        {
          OnAcceptCompleted(null, e);
        }
      }
      catch (ObjectDisposedException ex)
      {
        LOG.dc(ex.Message);
      }
    }
    void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
    {
      if (e.SocketError == SocketError.Success)
      {
        var conn = e.AcceptSocket; // 복사해놔야 한다.

        if (conn != null)
        {
          LOG.tc($"client connected. Port = {conn.LocalEndPoint}");
          this.SocketConnected(conn);
        }
      }
      else
      {
        if (_isRunning)
        {
          LOG.ec($"Accept Error: {e.SocketError}");
        }
      }
      this.StartAccept(e);
    }
    void SocketConnected(Socket socket)
    {
#if DEBUG
      if (this.ClientAccepted == null)
        throw new FrameworkException("ClientConnected is null");
#endif
      this.ClientAccepted?.Invoke(this, socket);
    }
  }
}