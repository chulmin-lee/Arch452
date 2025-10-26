using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;

namespace ServiceCommon.HospitalService
{
  public class ER_AREA_CONGEST_Loader : Grouping_LoaderBase<ER_AREA_CONGEST_GROUP, bool>
  {
    public ER_AREA_CONGEST_Loader() : base(SERVICE_ID.ER_AREA_CONGEST) { }
    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<ER_AREA_CONGEST_REQ>();
      var msg = this.create_message(req.IsChild);
      return msg != null ? msg : new ER_AREA_CONGEST_RESP();
    }
    protected override void subscribe_session(IServerSession s)
    {
      var er = s.PackageInfo?.Emergency ?? throw new Exception($"[{this.ID}] Emergency is null");

      var session = new GroupingDataSession<ER_AREA_CONGEST_GROUP, bool>(s, er.IsChild);
      this.Sessions.Add(session.ID, session);
      session.Send(this.create_message(session.Keys));
    }
    protected override ServiceMessage create_message(ER_AREA_CONGEST_GROUP item)
    {
      var list = item.AreaCongests;
      return new ER_AREA_CONGEST_RESP(list);
    }
    protected override ServiceMessage create_message(List<ER_AREA_CONGEST_GROUP> list)
    {
      var patients = new List<ER_AREA_CONGEST_INFO>();
      list.ForEach(x => patients.AddRange(x.AreaCongests));
      return new ER_AREA_CONGEST_RESP(patients);
    }
  }
}