using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class EXAM_PT_Service : MessageTransformer
  {
    Dictionary<string, OPD_ROOM_INFO> ROOMS = new Dictionary<string, OPD_ROOM_INFO>(); // deptCode:RoomCode
    public EXAM_PT_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.EXAM_PT)
    {
      this.subscribe(DATA_ID.DEPT_MASTER, DATA_ID.EXAM_ROOM, DATA_ID.EXAM_STAFF, DATA_ID.EXAM_PT);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.DEPT_MASTER: return dept_master_updated(o.Data<DEPT_MASTER_POCO>());
        case DATA_ID.EXAM_ROOM: return exam_room_updated(o.Data<EXAM_ROOM_POCO>());
        case DATA_ID.EXAM_STAFF: return exam_staff_updated(o.Data<EXAM_STAFF_POCO>());
        case DATA_ID.EXAM_PT: return exam_pt_update(o.Data<EXAM_PT_POCO, DATA_ID>());
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
      if (now != null) return now;
      //========================
      // check updated
      //========================
      var updated = new List<OPD_ROOM_INFO>();
      dept.AddedAndUpdated.ForEach(p =>
      {
        foreach (var room in ROOMS.Values.Where(x => x.DeptCode == p.DeptCode))
        {
          room.Room.DeptName = p.DeptName;
          updated.Add(room);
        }
      });
      //---------------------------------------
      // result
      //---------------------------------------
      return updated.Any() ? new NotifyMessage<OPD_ROOM_INFO> { ID = this.ID, Updated = updated } : null;
    }
    INotifyMessage exam_room_updated(UpdateData<EXAM_ROOM_POCO> rooms)
    {
      //========================
      // data backup
      //========================
      _exam_rooms.Clear();
      Mapper.Map<EXAM_ROOM_POCO[], List<ROOM_INFO>>(rooms.All.ToArray())
            .ForEach(x => _exam_rooms.Add(x.GroupKey, x));
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
      rooms.AddedAndUpdated.ForEach(p =>
      {
        var key = p.GroupKey;
        var room = _exam_rooms[key];
        if (!this.ROOMS.TryGetValue(key, out var opd_room))
        {
          opd_room = create_opd_room_info(room);
          this.ROOMS.Add(key, opd_room);
        }
        else
        {
          opd_room.Room = room;
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
    INotifyMessage exam_staff_updated(UpdateData<EXAM_STAFF_POCO> o)
    {
      //========================
      // data backup
      //========================
      _exam_staffs.Clear();
      Mapper.Map<EXAM_STAFF_POCO[], List<DOCTOR_INFO>>(o.All.ToArray())
            .ForEach(x => _exam_staffs.Add(x.DoctorNo, x));
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
      foreach (var dr in o.ChangedAll)
      {
        var opd_room = ROOMS.Values.Where(x => x.DoctorNo == dr.DoctorNo).FirstOrDefault();
        if(_exam_staffs.ContainsKey(dr.DoctorNo))
        {
          opd_room.Doctor = _exam_staffs[dr.DoctorNo];
        }
        else
        {
          opd_room.Clear();
        }
        updated.Add(opd_room);
      }
      //---------------------------------------
      // result
      //---------------------------------------
      return updated.Any() ? new NotifyMessage<OPD_ROOM_INFO> { ID = this.ID, Updated = updated } : null;
    }
    INotifyMessage exam_pt_update(UpdateData<EXAM_PT_POCO> o)
    {
      //========================
      // data backup
      //========================
      _exam_patients.Clear();
      foreach (var p in o.All.GroupBy(x => new { x.DeptCode, x.RoomCode }))
      {
        var key = $"{p.Key.DeptCode}:{p.Key.RoomCode}";
        var room_pt = Mapper.Map<EXAM_PT_POCO[], List<PATIENT_INFO>>(p.ToArray()).OrderBy(x => x.WaitNo).ToList();
        _exam_patients.Add(key, room_pt);
      }
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
      o.ChangedAll.GroupBy(x => new { x.DeptCode, x.RoomCode }).ToList().ForEach(group =>
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

    #region internal method
    protected override INotifyMessage IsJustReady()
    {
      if (this.IsReady && this.ROOMS.Count == 0)
      {
        var all = new List<OPD_ROOM_INFO>();
        foreach (var room in _exam_rooms.Values)
        {
          all.Add(this.create_opd_room_info(room));
        }
        all.ForEach(x => this.ROOMS.Add(x.GroupKey, x));
        return new NotifyMessage<OPD_ROOM_INFO> { ID = this.ID, Updated = all };
      }
      return null;
    }
    OPD_ROOM_INFO create_opd_room_info(ROOM_INFO room)
    {
      if (_dept_names.TryGetValue(room.DeptCode, out string name))
      {
        room.DeptName = name;
      }
      var key = room.GroupKey;
      var opd_room = new OPD_ROOM_INFO() { Room = room, };
      this.update_room_patients(opd_room, key);
      this.update_doctor(opd_room, key);
      return opd_room;
    }
    void update_doctor(OPD_ROOM_INFO opd_room, string dr_no)
    {
      if(_exam_staffs.ContainsKey(dr_no))
      {
        opd_room.Doctor = _exam_staffs[dr_no];
      }
      else
      {
        opd_room.Doctor = new DOCTOR_INFO();
      }
    }
    void update_room_patients(OPD_ROOM_INFO opd_room, string key)
    {
      if (_exam_patients.ContainsKey(key))
      {
        var patients = _exam_patients[key];
        opd_room.RoomPatient = patients.Where(x => x.InRoom).FirstOrDefault();
        opd_room.WaitPatients.Clear();
        opd_room.WaitPatients.AddRange(patients.Where(x => !x.InRoom));
      }
      else
      {
        opd_room.RoomPatient = null;
        opd_room.WaitPatients.Clear();
      }
    }
    #endregion

    #region data collection
    Dictionary<string, string> _dept_names = new Dictionary<string, string>();
    Dictionary<string, ROOM_INFO> _exam_rooms = new Dictionary<string, ROOM_INFO>();
    Dictionary<string, List<PATIENT_INFO>> _exam_patients = new Dictionary<string, List<PATIENT_INFO>>();
    Dictionary<string, DOCTOR_INFO> _exam_staffs = new Dictionary<string, DOCTOR_INFO>();
    #endregion
  }
}