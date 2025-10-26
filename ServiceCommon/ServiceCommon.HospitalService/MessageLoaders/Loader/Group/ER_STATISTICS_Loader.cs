using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  public class ER_STATISTICS_Loader : Grouping_LoaderBase<ER_STATISTIC_INFO, bool>
  {
    public ER_STATISTICS_Loader() : base(SERVICE_ID.ER_STATISTICS) { }

    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<ER_STATISTICS_REQ>();
      var msg = this.create_message(req.IsChild);

      return msg != null ? msg : new ER_STATISTICS_RESP(new ER_STATISTIC_INFO { IsChild = req.IsChild });
    }
    protected override void subscribe_session(IServerSession s)
    {
      var er = s.PackageInfo?.Emergency ?? throw new Exception($"[{this.ID}] Emergency is null");
      var session = new GroupingDataSession<ER_STATISTIC_INFO,bool>(s, er.IsChild);
      this.Sessions.Add(session.ID, session);
      session.Send(this.create_message(session.Keys));
    }
    protected override ServiceMessage create_message(ER_STATISTIC_INFO item) => new ER_STATISTICS_RESP(item);
    protected override ServiceMessage create_message(List<ER_STATISTIC_INFO> list) => new ER_STATISTICS_RESP(list.First());
  }
}