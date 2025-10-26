using Common;
using Framework.Network;
using System.Threading;

namespace ServiceCommon.ClientServices
{
  public abstract class ClientSessionBase : NetworkSessionBase<ServiceMessage>, IClientSession, IConnectionOwner<ServiceMessage>
  {
    protected ServiceMessage ConnectMessage;
    IClientSessionOwner SessionOwner;
    public ClientSessionBase(IClientSessionOwner owner)
    {
      this.SessionOwner = owner;
    }

    public virtual void Connect(ServiceMessage connect_message)
    {
      this.ConnectMessage = connect_message;
      this.Connect();
    }

    public void Connect()
    {
      if (this.Connection != null)
      {
        this.Connection.Close();
        this.Connection = null;
      }
      this.Connection = this.CreateClientConnection();
      this.Connection.Connect();
    }
    protected abstract INetworkConnection<ServiceMessage> CreateClientConnection();

    public virtual void OnMessageReceived(ServiceMessage message)
    {
      this.SessionOwner.OnMessageReceived(message);
    }
    public virtual void OnStateChanged(NetworkState state)
    {
      LOG.ic($"{this.NetworkId}: state: {state}");
      if (this.Connection == null) return;

      var o = this.SessionOwner.Option;

      switch (state)
      {
        case NetworkState.ConnectionFailed:
          {
            if (o.RetryConnecting)
            {
              Thread.Sleep(o.ConnectingInterval * 1000);
              this.Connection.Connect();
            }
            else
            {
              this.SessionOwner.OnStateChanged(state);
            }
          }
          break;
        case NetworkState.ConnectionLost:
          {
            this.Connection.Close();
            this.Connection = null;

            if (o.RecoverConnectionLost)
            {
              Thread.Sleep(o.RecoverInterval * 1000);
              this.Connect();
            }
            else
            {
              this.SessionOwner.OnStateChanged(state);
            }
          }
          break;
        case NetworkState.Connecting: break;
        case NetworkState.Closing:
          break;
        case NetworkState.Offline:
          break;
        case NetworkState.Online:
          {
            if (this.ConnectMessage != null)
            {
              this.Send(this.ConnectMessage);
            }
            this.SessionOwner.OnStateChanged(state);
          }
          break;
      }
    }
  }
}