using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  public class ER_PATIENT_Loader : Grouping_LoaderBase<ER_PATIENT_GROUP, bool>
  {
    public ER_PATIENT_Loader() : base(SERVICE_ID.ER_PATIENT) { }
    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<ER_PATIENT_REQ>();
      var msg = this.create_message(req.IsChild);
      return msg != null ? msg : new ER_PATIENT_RESP(req.IsChild);
    }

    protected override void subscribe_session(IServerSession s)
    {
      var er = s.PackageInfo?.Emergency ?? throw new Exception($"[{this.ID}] Emergency is null");
      var session = new GroupingDataSession<ER_PATIENT_GROUP,bool>(s, er.IsChild);
      this.Sessions.Add(session.ID, session);
      session.Send(this.create_message(session.Keys));
    }
    protected override ServiceMessage create_message(ER_PATIENT_GROUP item)
    {
      return new ER_PATIENT_RESP(item.Patients, item.IsChild);
    }

    protected override ServiceMessage create_message(List<ER_PATIENT_GROUP> list)
    {
      var patients = new List<EMERGENCY_INFO>();
      list.ForEach(x => patients.AddRange(x.Patients));
      return new ER_PATIENT_RESP(patients, list.First().IsChild);
    }
  }
}