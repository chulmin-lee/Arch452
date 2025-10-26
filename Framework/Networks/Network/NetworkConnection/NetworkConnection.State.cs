namespace Framework.Network
{
  /// <summary>
  /// 메시지 변환을 위해서 M,S,R 3가지 파라메터가 필요한다
  /// </summary>
  /// <typeparam name="M">사용자 메시지</typeparam>
  public abstract partial class NetworkConnection<M> : INetworkConnection<M>
  {
    //protected Dictionary<NetworkState, ConnectionStateBase> States = new();

    //protected class ConnectionStateBase
    //{
    //  public NetworkState State;
    //  public ConnectionStateBase(NetworkState state)
    //  {
    //    State = state;
    //  }
    //  public virtual void Send(M message)
    //  {
    //    LOG.e($"[{this.State}] Send() not supported");
    //  }
    //  public virtual void OnReceive(M message)
    //  {
    //    LOG.e($"[{this.State}] OnReceive() not supported");
    //  }
    //  public virtual void Connect()
    //  {
    //    LOG.e($"[{this.State}] Connect() not supported");
    //  }
    //  public virtual void OnConnectionLost(string s)
    //  {
    //    LOG.e($"[{this.State}] OnConnectionLost() not supported");
    //  }
    //  public virtual void Close()
    //  {
    //    LOG.e($"[{this.State}] Close() not supported");
    //  }
    //}
  }
}