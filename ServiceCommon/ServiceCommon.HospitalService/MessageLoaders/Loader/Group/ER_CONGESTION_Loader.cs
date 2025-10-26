using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  public class ER_CONGESTION_Loader : Grouping_LoaderBase<ER_CONGESTION_INFO, bool>
  {
    public ER_CONGESTION_Loader() : base(SERVICE_ID.ER_CONGESTION) { }

    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<ER_CONGESTION_REQ>();
      var msg = this.create_message(req.IsChild);

      return msg != null ? msg : new ER_CONGESTION_RESP(new ER_CONGESTION_INFO { IsChild = req.IsChild });
    }
    protected override void subscribe_session(IServerSession s)
    {
      var er = s.PackageInfo?.Emergency ?? throw new Exception($"[{this.ID}] Emergency is null");
      var session = new GroupingDataSession<ER_CONGESTION_INFO,bool>(s, er.IsChild);
      this.Sessions.Add(session.ID, session);
      session.Send(this.create_message(session.Keys));
    }
    protected override ServiceMessage create_message(ER_CONGESTION_INFO item) => new ER_CONGESTION_RESP(item);
    protected override ServiceMessage create_message(List<ER_CONGESTION_INFO> list) => new ER_CONGESTION_RESP(list.First());
  }
}