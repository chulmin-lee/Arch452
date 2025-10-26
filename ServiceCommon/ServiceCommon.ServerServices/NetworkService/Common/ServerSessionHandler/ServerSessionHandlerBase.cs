using Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace ServiceCommon.ServerServices
{
  public class ServerSessionHandlerBase : IServerSessionHandler, IServerSessionOwner
  {
    Dictionary<int, IServerSession> Sessions = new Dictionary<int, IServerSession>();
    int _session_key = 1;
    int _session_count = 0;
    protected IServerService ServerService { get; private set; }
    protected int NextKey => Interlocked.Increment(ref _session_key);

    public ServerSessionHandlerBase(IServerService manager)
    {
      this.ServerService = manager;
    }

    public virtual void OnMessageReceived(IServerSession session, ServiceMessage message)
    {
      this.ServerService.RequestService(session, message);
    }

    public virtual void OnSessionDisconnected(IServerSession session)
    {
      lock (this.Sessions)
      {
        int count = Interlocked.Decrement(ref _session_count);
        this.Sessions.Remove(session.Key);

        LOG.e($"UNREG: ID:{session.PackageInfo?.Package ?? PACKAGE.NONE}, count={count}, Sessions.Count={this.Sessions.Count}");

        if (count != this.Sessions.Count)
        {
          LOG.ec($"count != this.Sessions.Count");
        }
        this.ServerService.SessionDisconnected(session);
        session.Dispose();
      }
    }

    protected void OnSessionConnected(IServerSession session)
    {
      lock (this.Sessions)
      {
        try
        {
          this.Sessions.Add(session.Key, session);
          int count = Interlocked.Increment(ref _session_count);
          LOG.w($"REG: ID:{session.PackageInfo?.Package ?? PACKAGE.NONE}, count={count}, Sessions.Count={this.Sessions.Count}");
          this.ServerService.SessionConnected(session);
          if (count != this.Sessions.Count)
          {
            LOG.ec($"count != this.Sessions.Count");
          }
        }
        catch (Exception ex)
        {
          LOG.ec(ex.Message);
        }
      }
    }
    public virtual void Start() { }
    public virtual void Stop()
    {
      LOG.wc($"stop - start : Session count: {this.Sessions.Count}");

      lock (this.Sessions)
      {
        foreach (var session in this.all_sessions())
        {
          session.Close();
        }
        this.Sessions.Clear();
      }
      LOG.wc("stop - end");
    }
    public void Send(int key, ServiceMessage m)
    {
      var session = this.find_session(key);
      session?.Send(m);
    }
    public void SendAll(ServiceMessage m)
    {
      foreach (var session in this.all_sessions())
        session.Send(m);
    }

    protected IServerSession find_session(int key)
    {
      lock (this.Sessions)
      {
        return Sessions[key];
      }
    }
    protected List<IServerSession> all_sessions()
    {
      lock (this.Sessions)
      {
        return Sessions.Values.ToList();
      }
    }
  }
}