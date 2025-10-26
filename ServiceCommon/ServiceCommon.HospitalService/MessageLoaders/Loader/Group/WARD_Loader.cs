using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  public class WARD_Loader : Grouping_LoaderBase<AREA_WARD_INFO, string>
  {
    public WARD_Loader() : base(SERVICE_ID.WARD_ROOMS)
    {
    }
    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<WARD_ROOM_REQ>();
      var msg = this.create_message(req.GroupKey);
      if (msg != null) return msg;

      var temp = new AREA_WARD_INFO
      {
        Floor = req.Floor,
        AreaCode = req.AreaCode
      };
      return new WARD_ROOM_RESP(temp);
    }
    protected override void subscribe_session(IServerSession s)
    {
      var ward = s.PackageInfo?.WardRoom ?? throw new Exception($"[{this.ID }] ward is null");
      var session = new GroupingDataSession<AREA_WARD_INFO, string>(s, ward.GetKey());
      this.Sessions.Add(session.ID, session);
      var msg = this.create_message(session.Keys);
      session.Send(msg);
    }
    protected override ServiceMessage create_message(AREA_WARD_INFO item) => new WARD_ROOM_RESP(item);
    protected override ServiceMessage create_message(List<AREA_WARD_INFO> list) => new WARD_ROOM_RESP(list.First());
  }
}