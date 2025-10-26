using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  public class ER_ISOLATION_Loader : Grouping_LoaderBase<ER_ISOLATION_INFO, string>
  {
    public ER_ISOLATION_Loader() : base(SERVICE_ID.ER_ISOLATION)
    {
    }
    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<ER_ISOLATION_REQ>();
      var msg = this.create_message(req.BedCodes);
      if (msg != null) return msg;

      var temp = req.BedCodes.Select(x => new ER_ISOLATION_INFO { RoomCode = x }).ToList();
      return new ER_ISOLATION_RESP(temp);
    }
    protected override void subscribe_session(IServerSession s)
    {
      var er = s.PackageInfo?.EmergencyIsolation ?? throw new Exception($"[{this.ID}] EmergencyIsolation is null");

      var session = new GroupingDataSession<ER_ISOLATION_INFO, string>(s, er.BedCodes);
      this.Sessions.Add(session.ID, session);
      session.Send(this.create_message(session.Keys));
    }
    protected override ServiceMessage create_message(ER_ISOLATION_INFO item) => new ER_ISOLATION_RESP(item);
    protected override ServiceMessage create_message(List<ER_ISOLATION_INFO> list) => new ER_ISOLATION_RESP(list);
  }
}