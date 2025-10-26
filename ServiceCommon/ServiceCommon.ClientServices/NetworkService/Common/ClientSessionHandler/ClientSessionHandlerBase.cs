using Framework.Network;

namespace ServiceCommon.ClientServices
{
  /// <summary>
  /// 1개의 Session 관리.
  /// Session 생성은 Impl class 에서 수행
  /// </summary>
  public abstract class ClientSessionHandlerBase : IClientSessionHandler, IClientSessionOwner
  {
    public bool IsConnected => this.Session?.IsConnected ?? false;
    protected IClientSession Session { get; set; }
    protected IClientService ClientService { get; private set; }
    public ClientConnectionOption Option => ClientService.ConnectionOption;

    public ClientSessionHandlerBase(IClientService owner)
    {
      this.ClientService = owner;
    }

    public void OnMessageReceived(ServiceMessage message) => this.ClientService.OnMessageReceived(message);
    public virtual void OnStateChanged(NetworkState state) => this.ClientService.OnStateChanged(state);
    public virtual void Connect(ServiceMessage m) => this.Session?.Connect(m);
    public void Connect() => this.Session?.Connect();
    public void Send(ServiceMessage m) => this.Session?.Send(m);
    public void Close() => this.Session?.Close();
  }
}