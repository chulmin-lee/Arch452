using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.HospitalService;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class DR_PHOTO_Service : MessageTransformer
  {
    Dictionary<string, PHOTO_POCO> _photos = new Dictionary<string, PHOTO_POCO>(); // dr_no, PHOTO
    Dictionary<string, string> _doctor = new Dictionary<string, string>(); // doctor_no, group_key

    public DR_PHOTO_Service(IHospitalMemberOwner owner, Config config) : base(owner, SERVICE_ID.DR_PHOTO)
    {
      this.subscribe(DATA_ID.DR_PHOTO, DATA_ID.OFFICE_ROOM);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.DR_PHOTO: return photo_update(o.Data<PHOTO_POCO, DATA_ID>());
        case DATA_ID.OFFICE_ROOM: return office_update(o.Data<OFFICE_POCO, DATA_ID>());
      }
      return null;
    }
    INotifyMessage photo_update(UpdateData<PHOTO_POCO> d)
    {
      var updated = new List<PhotoUpdateInfo>();
      foreach (var p in d.Deleted)
      {
        if (_doctor.ContainsKey(p.DoctorNo))
        {
          updated.Add(new PhotoUpdateInfo { DoctorNo = p.DoctorNo, RoomKey = _doctor[p.DoctorNo] });
        }
      }

      foreach (var p in d.AddedAndUpdated)
      {
        if (_doctor.ContainsKey(p.DoctorNo))
        {
          updated.Add(new PhotoUpdateInfo
          {
            DoctorNo = p.DoctorNo,
            FilePath = p.FilePath,
            RoomKey = _doctor[p.DoctorNo]
          });
        }
      }
      return new NotifyMessage<PhotoUpdateInfo> { ID = this.ID, Updated = updated };
    }

    INotifyMessage office_update(UpdateData<OFFICE_POCO> updated)
    {
      // 의사가 바뀌면 client가 요청하므로, 이 서비스에서는 런타임중 의사 사진이 바뀐 경우에만 통지하는 역할을 수행한다
      // 그러므로 현재 의사가 할당된 진료실만 업데이트 한다.
      _doctor.Clear();
      foreach (var group in updated.All.GroupBy(x => new { x.DoctorNo, x.DeptCode, x.RoomCode }))
      {
        var dr_no = group.Key.DoctorNo;
        var group_key = $"{group.Key.DeptCode}:{group.Key.RoomCode}";
        _doctor.Add(dr_no, group_key);
      }
      return null;
    }

    internal class EventData : INotifyMessage
    {
      public string PhotoDir;
      public SERVICE_ID ID { get; set; } = SERVICE_ID.DR_PHOTO;
      public UpdateData<PHOTO_POCO> Datas;
      public EventData(string dir, UpdateData<PHOTO_POCO> d)
      {
        PhotoDir = dir;
        Datas = d;
      }
    }

    internal class Config : ServiceConfig
    {
      public List<string> ExceptDoctors = new List<string>();
      public Config() : base(SERVICE_ID.DR_PHOTO) { }
    }
  }
}