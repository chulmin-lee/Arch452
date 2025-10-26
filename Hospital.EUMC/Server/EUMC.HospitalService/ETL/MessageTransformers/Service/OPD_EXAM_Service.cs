using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class OPD_EXAM_Service : MessageTransformer
  {
    public OPD_EXAM_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.EXAM_ROOM)
    {
      this.subscribe(DATA_ID.EXAM_ROOM);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.EXAM_ROOM: return exam_update(o.Data<EXAM_POCO, DATA_ID>());
      }
      return null;
    }
    INotifyMessage exam_update(UpdateData<EXAM_POCO> updated)
    {
      // 데이터가 변경된 부서와 병실 목록
      var rooms = new List<OPD_ROOM_INFO>();
      foreach (var group in updated.ChangedAll.GroupBy(x => new { x.DeptCode, x.RoomCode }))
      {
        var deptCode = group.Key.DeptCode;
        var roomCode = group.Key.RoomCode;

        var room_patients = updated.All.Where(x => x.DeptCode == deptCode && x.RoomCode == roomCode);
        if (room_patients.Any())
        {
          var room_patient = room_patients.Where(x => x.InRoom).FirstOrDefault();
          if (room_patient != null)
          {
            room_patients = room_patients.Where(x => !x.InRoom).ToList();
          }

          rooms.Add(new OPD_ROOM_INFO
          {
            Room = Mapper.Map<EXAM_POCO, ROOM_INFO>(room_patients.First()),
            RoomPatient = room_patient != null ? Mapper.Map<EXAM_POCO, PATIENT_INFO>(room_patient) : null,
            WaitPatients = Mapper.Map<EXAM_POCO[], List<PATIENT_INFO>>(room_patients.ToArray()).OrderBy(x => x.WaitNo).ToList()
          });
        }
        else
        {
          var find = updated.Deleted.Where(x => x.DeptCode == deptCode && x.RoomCode == roomCode).FirstOrDefault();
          if (find == null)
          {
            throw new ServiceException($"{deptCode}:{roomCode} not found");
          }
          rooms.Add(new OPD_ROOM_INFO
          {
            Room = Mapper.Map<EXAM_POCO, ROOM_INFO>(find)
          });
        }
      }

      //---------------------------------------
      // result
      //---------------------------------------
      return new NotifyMessage<OPD_ROOM_INFO> { ID = this.ID, Updated = rooms };
    }
  }
}