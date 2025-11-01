using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  public class EXAM_PT_Loader : Grouping_LoaderBase<OPD_ROOM_INFO, string>
  {
    public EXAM_PT_Loader() : base(SERVICE_ID.EXAM_PT)
    {
    }
    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<EXAM_REQ>();
      var keys = req.RoomCodes.Select(x => $"{req.DeptCode}:{x}").ToList();
      var msg = this.create_message(keys);
      if (msg != null) return msg;

      var items = req.RoomCodes.Select(x => new OPD_ROOM_INFO
      {
        Room = new ROOM_INFO { DeptCode = req.DeptCode, RoomCode = x }
      }).ToList();
      return new EXAM_RESP(items);
    }

    protected override void subscribe_session(IServerSession s)
    {
      var opd = s.PackageInfo?.OpdRoom ?? throw new Exception($"[{this.ID }] opd is null");

      var exams = opd.DeptRooms.Where(x => x.RoomType == "B").ToList();
      var keys = new List<string>();
      exams.ForEach(x => keys.AddRange(x.GetKeys()));

      var session = new GroupingDataSession<OPD_ROOM_INFO,string>(s, keys);
      this.Sessions.Add(session.ID, session);

      var msg = this.create_message(session.Keys);
      session.Send(msg);
    }
    protected override ServiceMessage create_message(List<OPD_ROOM_INFO> items) => new EXAM_RESP(items);
    protected override ServiceMessage create_message(OPD_ROOM_INFO item) => new EXAM_RESP(item);
  }
}