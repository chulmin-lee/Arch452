using Common;
using ServiceCommon.ServerServices;
//using UIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace ServiceCommon.HospitalService
{
  public class ER_CPR_Loader : MessageLoaderBase
  {
    Dictionary<int, SessionMember> Sessions = new Dictionary<int, SessionMember>();
    bool _cpr = false;
    public ER_CPR_Loader() : base(SERVICE_ID.ER_CPR) { }

    protected override void message_notified(INotifyMessage m)
    {
      var data = m as NotifyMessage<bool>;
      if (data == null) throw new Exception($"{m.ID} not supported");

      var cpr = data.Updated.First();
      if (cpr != _cpr)
      {
        _cpr = cpr;
        var msg = this.create_message();
        foreach (var session in this.Sessions.Values)
        {
          session.Send(msg);
        }
      }
    }
    protected override ServiceMessage request_service(ServiceMessage m) => this.create_message();

    ER_CPR_RESP create_message() => new ER_CPR_RESP(_cpr);
    protected override void subscribe_session(IServerSession s)
    {
       var session = new SessionMember(s);
      this.Sessions.Add(session.ID, session);
      if(_cpr)
      {
        session.Send(this.create_message());
      }
    }

    protected override void unsubscribe_session(IServerSession s)
    {
      this.Sessions.Remove(s.Key);
    }
  }


}