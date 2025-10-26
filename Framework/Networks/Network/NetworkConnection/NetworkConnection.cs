using Common;
using System;
using System.Threading.Tasks;

namespace Framework.Network
{
  /// <summary>
  /// 메시지 변환을 위해서 M,S,R 3가지 파라메터가 필요한다
  /// </summary>
  /// <typeparam name="M">사용자 메시지</typeparam>
  /// <typeparam name="S">전송 Network Data type</typeparam>
  /// <typeparam name="R">수신 Network Data type</typeparam>
  public abstract partial class NetworkConnection<M> : INetworkConnection<M>
    where M : class, new()
  {
    public virtual string NetworkId { get; protected set; } = string.Empty;
    public NetworkState State => this.StateLock.State;
    public virtual bool IsOnline => this.StateLock.IsSet(NetworkState.Online);

    protected EnumInterlocked<NetworkState> StateLock;
    protected IConnectionOwner<M> ConnectionOwner;
    IProducerConsumer<M> SendChannel;

    public NetworkConnection(IConnectionOwner<M> owner, NetworkState init_state = NetworkState.Offline)
    {
      this.ConnectionOwner = owner;
      this.StateLock = new EnumInterlocked<NetworkState>(init_state);
      this.SendChannel = new ProducerMonitor<M>(send_proc);
    }
    protected bool ChangeState(NetworkState new_state, bool notify = false)
    {
      if (this.StateLock.Set(new_state))
      {
        if (notify)
        {
          this.ConnectionOwner.OnStateChanged(new_state);
        }
        return true;
      }
      return false;
    }
    protected bool IsState(NetworkState new_state) => this.StateLock.IsSet(new_state);
    //================================
    // 메시지 전송
    // - 실제 구현은 send_impl
    //================================
    #region Send
    public void Send(M m)
    {
      if (this.IsOnline)
      {
        this.SendChannel.Write(m);
      }
    }
    /// <summary>
    /// SendChannel에 전송할 메시지가 있을때 SendChannel에 의해서 호출된다
    /// online 상태인 경우 메시지를 네트웍 데이타로 변환하여 전송한다
    /// 그렇지 않다면 처리하지 않고 버린다
    /// </summary>
    async Task send_proc(M m)
    {
      try
      {
        await this.send_impl(m);
      }
      catch (Exception ex)
      {
        this.OnConnectionLost(ex.Message);
      }
    }
    /// <summary>
    /// 실제 네트웍으로 전송.
    /// GRPC ServerStreaming의 client 처럼, send 기능이 없는 경우 재정의하지 않아도 되도록 기본 메소드 정의
    /// </summary>
    protected virtual Task send_impl(M message) => Task.FromResult<object>(null);
    #endregion Send

    //================================
    // 메시지 수신
    // - 실제 메시지 수신은 impl class 에서 처리 후 OnReceive() 호출
    //================================
    #region Receive
    /// <summary>
    /// 네트웍 데이타를 메시지로 변환해서 구독자(=Session)에게 전달한다
    /// </summary>
    protected virtual void OnReceive(M m)
    {
      if (this.IsOnline)
      {
        this.ConnectionOwner.OnMessageReceived(m);
      }
    }
    #endregion Receive

    //================================
    // 연결 및 연결 상태
    //================================
    #region Connect
    public virtual void Connect() { }
    #endregion Connect

    //================================
    // 종료
    // - 실제 구현은 close_impl
    //================================
    #region Close

    /// <summary>
    /// Online 상태에서만 동작
    /// </summary>
    /// <param name="message"></param>
    protected void OnConnectionLost(string message)
    {
      if (this.IsOnline)
      {
        if (this.ChangeState(NetworkState.ConnectionLost))
        {
          LOG.ec(message);
          this.clean_up();
          this.ConnectionOwner?.OnStateChanged(this.State);
        }
      }
    }

    /// <summary>
    /// local에서 remote 연결을 끊는다.
    /// </summary>
    public void Close()
    {
      if (this.ChangeState(NetworkState.Closing))
      {
        LOG.dc("close");
        this.clean_up();
      }
    }
    void clean_up()
    {
      LOG.dc("clean up");
      this.close_impl();
      this.SendChannel.Clear();
    }
    protected abstract void close_impl();

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    bool _disposed;
    protected virtual void Dispose(bool disposing)
    {
      if (!_disposed && disposing)
      {
        LOG.ic($"{this.NetworkId}:");

        this.Close();
      }
      _disposed = true;
    }
    #endregion Close
  }
}