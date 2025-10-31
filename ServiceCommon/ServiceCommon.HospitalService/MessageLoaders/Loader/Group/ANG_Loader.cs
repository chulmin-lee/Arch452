using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;

namespace ServiceCommon.HospitalService
{
  public class ANG_Loader : Grouping_LoaderBase<ANG_PT_INFO, ANG_TYPE>
  {
    public ANG_Loader() : base(SERVICE_ID.ANG) {
    }

    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<ANG_REQ>();
      var msg = this.create_message(req.Type);
      if (msg != null) return msg;
      return new ANG_RESP();
    }

    protected override void subscribe_session(IServerSession s)
    {
      var ang = s.PackageInfo?.Angiography ?? throw new Exception($"[{this.ID}] Angiography is null");
      var session = new GroupingDataSession<ANG_PT_INFO, ANG_TYPE>(s, ang.Type);
      this.Sessions.Add(session.ID, session);
      session.Send(this.create_message(session.Keys));
    }

    protected override ServiceMessage create_message(ANG_PT_INFO item) => new ANG_RESP(item);
    protected override ServiceMessage create_message(List<ANG_PT_INFO> list) => new ANG_RESP(list);
  }
}
