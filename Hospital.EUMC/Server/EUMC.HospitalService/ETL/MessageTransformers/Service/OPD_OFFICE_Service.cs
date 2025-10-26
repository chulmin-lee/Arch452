using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class OPD_OFFICE_Service : MessageTransformer
  {
    public OPD_OFFICE_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.OFFICE_ROOM)
    {
      this.subscribe(DATA_ID.OFFICE_ROOM);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.OFFICE_ROOM: return office_update(o.Data<OFFICE_POCO, DATA_ID>());
      }
      return null;
    }
    INotifyMessage office_update(UpdateData<OFFICE_POCO> updated)
    {
      // 변경이 발생한 deptCode:roomCode 에 해당하는 OPD_ROOM_INFO 생성
      // 생성
      // - All에서 dept:room으로 조회해서 만든다
      // - 만약 All에 해당하는 데이타가 없으면 환자가없는 빈 OPD_ROOM_INFO를 만든다

      var rooms = new List<OPD_ROOM_INFO>();
      foreach (var group in updated.ChangedAll.GroupBy(x => new { x.DeptCode, x.RoomCode }))
      {
        var deptCode = group.Key.DeptCode;
        var roomCode = group.Key.RoomCode;

        var patients = updated.All.Where(x => x.DeptCode == deptCode && x.RoomCode == roomCode);
        if (patients.Any())
        {
          var room_patient = patients.Where(x => x.InRoom).FirstOrDefault();
          if (room_patient != null)
          {
            patients = patients.Where(x => !x.InRoom).ToList();
          }
          rooms.Add(new OPD_ROOM_INFO()
          {
            Room = Mapper.Map<OFFICE_POCO, ROOM_INFO>(patients.First()),
            Doctor = Mapper.Map<OFFICE_POCO, DOCTOR_INFO>(patients.First()),
            RoomPatient = room_patient != null ? Mapper.Map<OFFICE_POCO, PATIENT_INFO>(room_patient) : null,
            // 대기순으로 정렬
            WaitPatients = Mapper.Map<OFFICE_POCO[], List<PATIENT_INFO>>(patients.ToArray()).OrderBy(x => x.WaitNo).ToList()
          });
        }
        else
        {
          // All에서 조회되는 환자가 없으면 환자없는 OPD_ROOM_INFO 생성

          var find = updated.Deleted.Where(x => x.DeptCode == deptCode && x.RoomCode == roomCode).FirstOrDefault();
          if (find == null)
          {
            throw new ServiceException($"{deptCode}:{roomCode} not found");
          }
          rooms.Add(new OPD_ROOM_INFO
          {
            Room = Mapper.Map<OFFICE_POCO, ROOM_INFO>(find),
            Doctor = Mapper.Map<OFFICE_POCO, DOCTOR_INFO>(find),
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