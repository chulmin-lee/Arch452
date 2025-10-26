using Common;
using Framework.Network;

namespace ServiceCommon.ServerServices
{
  public abstract class ServerSessionBase : NetworkSessionBase<ServiceMessage>, IServerSession, IConnectionOwner<ServiceMessage>
  {
    public int SessionId { get; set; }

    public int Key { get; private set; }

    public PackageInfo PackageInfo { get; protected set; }
    public CLIENT_INFO ClientInfo { get; protected set; }
    public PACKAGE Package => this.PackageInfo?.Package ?? PACKAGE.NONE;

    public bool UseService => this.PackageInfo?.UseService ?? false;

    IServerSessionOwner SessionOwner;
    public ServerSessionBase(int key, IServerSessionOwner owner)
    {
      this.Key = key;
      this.SessionOwner = owner;
    }
    public override string ToString()
    {
      return $"{PackageInfo?.Package ?? PACKAGE.NONE}";
    }
    public virtual void OnMessageReceived(ServiceMessage message)
    {
      this.SessionOwner.OnMessageReceived(this, message);
    }

    public virtual void OnStateChanged(NetworkState state)
    {
      LOG.dc($"state = {state}");
      if (state == NetworkState.ConnectionLost)
      {
        this.SessionOwner.OnSessionDisconnected(this);
      }
    }
  }
}