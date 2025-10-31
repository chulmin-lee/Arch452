using Common;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System;

namespace ServiceCommon.HospitalService
{
  public class ENDO_Loader : Grouping_LoaderBase<ENDO_PT_INFO, ENDO_TYPE>
  {
    public ENDO_Loader() : base(SERVICE_ID.ENDO) { }
    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<ENDO_REQ>();
      var msg = this.create_message(req.Type);
      if (msg != null) return msg;
      return new ENDO_RESP();
    }

    protected override void subscribe_session(IServerSession s)
    {
      var endo = s.PackageInfo?.Endoscope ?? throw new Exception($"[{this.ID}] Endoscope is null");
      var session = new GroupingDataSession<ENDO_PT_INFO, ENDO_TYPE>(s, endo.Type);
      this.Sessions.Add(session.ID, session);
      session.Send(this.create_message(session.Keys));
    }

    protected override ServiceMessage create_message(ENDO_PT_INFO item) => new ENDO_RESP(item);
    protected override ServiceMessage create_message(List<ENDO_PT_INFO> list) => new ENDO_RESP(list);
  }
}