using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  public class ICU_Loader : Grouping_LoaderBase<ICU_PT_INFO, string>
  {
    public ICU_Loader() : base(SERVICE_ID.ICU)
    {
    }
    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<ICU_REQ>();
      var msg = this.create_message(req.IcuCodes);
      if (msg != null) return msg;

      var temp = req.IcuCodes.Select(x => new ICU_PT_INFO { IcuCode = x }).ToList();
      return new ICU_RESP(temp);
    }

    protected override void subscribe_session(IServerSession s)
    {
      var icu = s.PackageInfo?.Icu ?? throw new Exception($"[{this.ID}] Icu is null");
      var session = new GroupingDataSession<ICU_PT_INFO, string>(s, icu.WardCodes);
      this.Sessions.Add(session.ID, session);
      session.Send(this.create_message(session.Keys));
    }
    protected override ServiceMessage create_message(List<ICU_PT_INFO> list) => new ICU_RESP(list);
    protected override ServiceMessage create_message(ICU_PT_INFO item) => new ICU_RESP(item);
  }
}