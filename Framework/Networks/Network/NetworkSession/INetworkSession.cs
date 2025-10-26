using System;

namespace Framework.Network
{
  public interface INetworkSession<M> : IDisposable
    where M : class, new()
  {
    string NetworkId { get; }
    NetworkState NetworkState { get; }
    bool IsConnected { get; }
    void Close();
    void Send(M message);
  }
}