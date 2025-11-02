using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class OFFICE_PT_Service : MessageTransformer
  {
    Dictionary<string, OPD_ROOM_INFO> ROOMS = new Dictionary<string, OPD_ROOM_INFO>();
    Config CONFIG;
    public OFFICE_PT_Service(IHospitalMemberOwner owner, Config config) : base(owner, SERVICE_ID.OFFICE_PT)
    {
      this.CONFIG = config;
      this.subscribe(DATA_ID.DEPT_MASTER, DATA_ID.OFFICE_ROOM, DATA_ID.OFFICE_PT, DATA_ID.DR_PHOTO);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.DEPT_MASTER: return dept_master_updated(o.Data<DEPT_MASTER_POCO>());
        case DATA_ID.OFFICE_ROOM: return office_room_updated(o.Data<OFFICE_ROOM_POCO>());
        case DATA_ID.OFFICE_PT: return office_pt_updated(o.Data<OFFICE_PT_POCO>());
        case DATA_ID.DR_PHOTO: return dr_photo_updated(o.Data<DR_PHOTO_POCO>());
      }
      return null;
    }
    INotifyMessage dept_master_updated(UpdateData<DEPT_MASTER_POCO> dept)
    {
      //========================
      // data backup
      //========================
      _dept_names.Clear();
      dept.All.ForEach(x => _dept_names.Add(x.DeptCode, x.DeptName));
      if (!this.IsReady) return null;
      //========================
      // check first ready
      //========================
      var now = this.IsJustReady();
      if(now != null) return now;
      //========================
      // check updated
      //========================
      var updated = new List<OPD_ROOM_INFO>();
      dept.AddedAndUpdated.ForEach(p =>
      {
        foreach (var room in ROOMS.Values.Where(x => x.DeptCode == p.DeptCode))
        {
          this.check_room_info(room.Room);
          updated.Add(room);
        }
      });
      //---------------------------------------
      // result
      //---------------------------------------
      return updated.Any() ? new NotifyMessage<OPD_ROOM_INFO> { ID = this.ID, Updated = updated } : null;
    }
    INotifyMessage office_room_updated(UpdateData<OFFICE_ROOM_POCO> rooms)
    {
      //========================
      // data backup
      //========================
      _office_rooms.Clear();
      rooms.All.ForEach(x => _office_rooms.Add(x.GroupKey, x));
      if (!this.IsReady) return null;
      //========================
      // check first ready
      //========================
      var now = this.IsJustReady();
      if (now != null) return now;
      //========================
      // check updated
      //========================
      var updated = new List<OPD_ROOM_INFO>();
      rooms.AddedAndUpdated.ForEach(room =>
      {
        var key = room.GroupKey;
        if (!this.ROOMS.TryGetValue(key, out var opd_room))
        {
          opd_room = create_opd_room_info(room);
          this.ROOMS.Add(key, opd_room);
        }
        else
        {
          opd_room.Room = this.check_room_info(Mapper.Map<OFFICE_ROOM_POCO, ROOM_INFO>(room));
          opd_room.Doctor = this.check_doctor_info(Mapper.Map<OFFICE_ROOM_POCO, DOCTOR_INFO>(room));
        }
        updated.Add(opd_room);
      });
      rooms.Deleted.ForEach(room =>
      {
        if (this.ROOMS.TryGetValue(room.GroupKey, out var opd_room))
        {
          opd_room.Clear();
          updated.Add(opd_room);
        }
      });
      //---------------------------------------
      // result
      //---------------------------------------
      return updated.Any() ? new NotifyMessage<OPD_ROOM_INFO> { ID = this.ID, Updated = updated } : null;
    }
    INotifyMessage office_pt_updated(UpdateData<OFFICE_PT_POCO> patients)
    {
      //========================
      // data backup
      //========================
      _office_patients.Clear();
      patients.All.GroupBy(x => new { x.DeptCode, x.RoomCode }).ToList().ForEach(p =>
      {
        var key = $"{p.Key.DeptCode}:{p.Key.RoomCode}";
        var room_pt = Mapper.Map<OFFICE_PT_POCO[], List<PATIENT_INFO>>(p.ToArray()).OrderBy(x => x.WaitNo).ToList();
        room_pt.ForEach(x => x.PatientName = x.PatientName.MaskedName());
        _office_patients.Add(key, room_pt);
      });
      if (!this.IsReady) return null;
      //========================
      // check first ready
      //========================
      var now = this.IsJustReady();
      if (now != null) return now;
      //========================
      // check updated
      //========================
      var updated = new List<OPD_ROOM_INFO>();
      patients.ChangedAll.GroupBy(x => new { x.DeptCode, x.RoomCode }).ToList().ForEach(group =>
      {
        var key = $"{group.Key.DeptCode}:{group.Key.RoomCode}";
        if (this.ROOMS.TryGetValue(key, out var opd_room))
        {
          this.update_room_patients(opd_room, key);
          updated.Add(opd_room);
        }
      });
       //---------------------------------------
      // result
      //---------------------------------------
      return updated.Any() ? new NotifyMessage<OPD_ROOM_INFO> { ID = this.ID, Updated = updated } : null;
    }
    INotifyMessage dr_photo_updated(UpdateData<DR_PHOTO_POCO> photo)
    {
      //========================
      // data backup
      //========================
      _dr_photos.Clear();
      photo.All.ForEach(x => _dr_photos.Add(x.DoctorNo, x));
      if (!this.IsReady) return null;
      //========================
      // check first ready
      //========================
      var now = this.IsJustReady();
      if (now != null) return now;
      //========================
      // check updated
      //========================
      var updated = new List<OPD_ROOM_INFO>();
      photo.ChangedAll.ForEach(dr =>
      {
        if (this.CONFIG.ShowPhoto(dr.DoctorNo))
        {
          var opd_room = ROOMS.Values.Where(x => x.DoctorNo == dr.DoctorNo).FirstOrDefault();
          if (opd_room != null)
          {
            _dr_photos.TryGetValue(dr.DoctorNo, out var find);
            opd_room.Doctor.PhotoUrl = find?.PhotoUrl ?? string.Empty;
            updated.Add(opd_room);
          }
        }
      });
      //---------------------------------------
      // result
      //---------------------------------------
      return updated.Any() ? new NotifyMessage<OPD_ROOM_INFO> { ID = this.ID, Updated = updated } : null;
    }


    #region internal method
    protected override INotifyMessage IsJustReady()
    {
      if (this.IsReady && this.ROOMS.Count == 0)
      {
        var all = new List<OPD_ROOM_INFO>();
        foreach (var room in _office_rooms.Values)
        {
          all.Add(this.create_opd_room_info(room));
        }
        all.ForEach(x => this.ROOMS.Add(x.GroupKey, x));
        return new NotifyMessage<OPD_ROOM_INFO> { ID = this.ID, Updated = all };
      }
      return null;
    }
    OPD_ROOM_INFO create_opd_room_info(OFFICE_ROOM_POCO room)
    {
      var key = room.GroupKey;
      var opd_room = new OPD_ROOM_INFO
      {
        Room = this.check_room_info(Mapper.Map<OFFICE_ROOM_POCO, ROOM_INFO>(room)),
        Doctor = this.check_doctor_info(Mapper.Map<OFFICE_ROOM_POCO, DOCTOR_INFO>(room))
      };
      this.update_room_patients(opd_room, key);
      return opd_room;
    }
    void update_room_patients(OPD_ROOM_INFO opd_room, string key)
    {
      if (!_office_patients.TryGetValue(key, out var list))
      {
        list = new List<PATIENT_INFO>();
      }
      opd_room.RoomPatient = null;
      opd_room.WaitPatients.Clear();

      if (list.Any())
      {
        opd_room.RoomPatient = list.First();
        opd_room.WaitPatients = list.Skip(1).ToList();
      }
    }

    /// <summary>
    /// 의사이름에 부서명이 들어있는 경우 처리
    /// 예) "심장혈관흉부외과※성숙환"
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    DOCTOR_INFO check_doctor_info(DOCTOR_INFO o)
    {
      if (!string.IsNullOrWhiteSpace(o.DoctorName))
      {
        int index = o.DoctorName.IndexOf('※');
        if (index > 0)
        {
          var arr = o.DoctorName.Split('※');
          if (arr.Length > 1)
          {
            o.DoctorDeptName = arr[0];
            o.DoctorName = arr[1];
          }
        }
        else
        {
          index = o.DoctorName.IndexOf(')');
          if (index > 0)
          {
            var arr = o.DoctorName.Split('(', ')');
            if(arr.Length > 2)
            {
              o.DoctorName = arr[1];
            }
          }
        }

        if (this.CONFIG.ShowPhoto(o.DoctorNo))
        {
          _dr_photos.TryGetValue(o.DoctorNo, out var photo);
          o.PhotoUrl = photo?.PhotoUrl ?? string.Empty;
        }
      }
      return o;
    }
    /// <summary>
    /// 부서명 할당
    /// - Room 정보에는 부서명이 없다
    ///   dept_rooms에서 찾아서 할당하고, 조건에 따라 다른 이름을 사용해야할 수 있다.
    /// 방이름
    /// - Room 정보에 방이름은 있지만, 조건에 따라 변경해야 한다.
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    ROOM_INFO check_room_info(ROOM_INFO o)
    {
      var names = this.CONFIG.Find(o.DeptCode, o.RoomCode);
      if (names != null)
      {
        o.DeptName = names.NewDeptName;
        if (names.NewRoomNames.ContainsKey(o.RoomCode))
        {
          o.RoomName = names.NewRoomNames[o.RoomCode];
        }
      }
      else
      {
        if (_dept_names.TryGetValue(o.DeptCode, out string name))
        {
          o.DeptName = name;
        }
      }
      return o;
    }
    #endregion

    #region data collection
    Dictionary<string, string> _dept_names = new Dictionary<string, string>();
    // 방정보와 의사정보가 같이 들어있어서 POCO 로 관리
    Dictionary<string, OFFICE_ROOM_POCO> _office_rooms = new Dictionary<string, OFFICE_ROOM_POCO>();
    Dictionary<string, List<PATIENT_INFO>> _office_patients = new Dictionary<string, List<PATIENT_INFO>>();
    Dictionary<string, DR_PHOTO_POCO> _dr_photos = new Dictionary<string, DR_PHOTO_POCO>();
    #endregion

    internal class OfficeNameConfig
    {
      public string DeptCode { get; set; }
      public List<string> Conditions { get; set; } = new List<string>();
      public string NewDeptName { get; set; }
      public Dictionary<string, string> NewRoomNames { get; set; } = new Dictionary<string, string>();
    }


    internal class Config : ServiceConfig
    {
      public List<string> NoPhotoDoctors { get; set; } = new List<string>();
      public List<OfficeNameConfig> NameConfigs { get; set; } = new List<OfficeNameConfig>();
      public Config() : base(SERVICE_ID.OFFICE_PT)
      {
      }
      public OfficeNameConfig Find(string deptCode, string roomCode)
      {
        return NameConfigs.Where(x => x.DeptCode == deptCode && x.Conditions.Contains(roomCode)).FirstOrDefault();
      }
      public bool ShowPhoto(string dr_no)
      {
        return !this.NoPhotoDoctors.Contains(dr_no);
      }
    }
  }
}