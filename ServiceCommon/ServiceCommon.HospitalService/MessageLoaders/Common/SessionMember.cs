using Common;
using ServiceCommon.ServerServices;

namespace ServiceCommon.HospitalService
{
  public class SessionMember
  {
    protected IServerSession Session;
    public int ID => this.Session.Key;
    public SessionMember(IServerSession s)
    {
      this.Session = s;
    }
    public virtual void Send(ServiceMessage m)
    {
      if (m != null)
      {
        LOG.dc($"{m.ServiceId}");
        this.Session.Send(m);
      }
    }
  }
}