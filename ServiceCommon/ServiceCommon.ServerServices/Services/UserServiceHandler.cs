using Common;
using System.Collections.Generic;

namespace ServiceCommon.ServerServices
{
  public class UserServiceHandler : IMessageServiceHandler
  {
    Dictionary<int, IServerSession> Sessions = new Dictionary<int, IServerSession>();
    public List<SERVICE_ID> SupportMessages { get; private set; } = new List<SERVICE_ID>();

    public List<PACKAGE> SupportPackages => new List<PACKAGE>();

    object _lock = new object();
    IServerSessionHandler SessionHandler;

    public UserServiceHandler(IServerSessionHandler manager)
    {
      SupportMessages.Add(SERVICE_ID.CLIENT_STATUS);
      SupportMessages.Add(SERVICE_ID.CLIENT_MEDIA_NOTI);
      this.SessionHandler = manager;
    }
    public bool Subscribe(IServerSession session)
    {
      lock (_lock)
      {
        if (!this.Sessions.ContainsKey(session.Key))
        {
          this.Sessions.Add(session.Key, session);
          return true;
        }
        LOG.ec($"key: {session.Key} already exsit");
        return false;
      }
    }
    public bool Unsubscribe(IServerSession session)
    {
      lock (_lock)
      {
        if (this.Sessions.ContainsKey(session.Key))
        {
          this.Sessions.Remove(session.Key);
          return true;
        }
        LOG.ec($"key :{session.Key} not exist");
        return false;
      }
    }
    public void Send(int key, ServiceMessage m)
    {
      LOG.wc($"Key: {key}, {m.ServiceId}");
      this.SessionHandler.Send(key, m);
    }
    public void SendAll(ServiceMessage m)
    {
      LOG.wc($"{m.ServiceId}");
      this.SessionHandler.SendAll(m);
    }
    public void Start()
    {
    }

    public void Stop()
    {
      lock (_lock)
      {
        this.Sessions.Clear();
      }
    }
    public ServiceMessage RequestService(ServiceMessage m)
    {
      LOG.dc($"{m.ServiceId}");
      return new CLIENT_STATUS_RESP(true); ;
    }

    public void RequestService(IServerSession session, ServiceMessage m)
    {
      var status = m.CastTo<CLIENT_STATUS_REQ>();
    }
  }
}