using Common;
using System;

namespace Framework.Network
{
  public abstract class NetworkSessionBase<M> : INetworkSession<M>, IDisposable
     where M : class, new()
  {
    protected INetworkConnection<M> Connection { get; set; }
    public string NetworkId => this.Connection?.NetworkId ?? "not connected";
    public NetworkState NetworkState => this.Connection?.State ?? NetworkState.None;

    public bool IsConnected => this.Connection?.IsOnline ?? false;

    public virtual void Send(M m)
    {
      if (this.Connection == null)
      {
        throw new FrameworkException($"connection is null");
      }
      this.Connection.Send(m);
    }

    public void Close()
    {
      LOG.dc("session close");
      this.Connection?.Close();
      this.Connection = null;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    bool _disposed;
    protected virtual void Dispose(bool disposing)
    {
      LOG.dc("session dispose");
      if (!_disposed && disposing)
      {
        this.Close();
      }
      _disposed = true;
    }
    public override string ToString()
    {
      return this.NetworkId;
    }
  }
}