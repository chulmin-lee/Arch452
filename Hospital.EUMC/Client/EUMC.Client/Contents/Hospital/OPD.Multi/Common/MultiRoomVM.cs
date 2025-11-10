using EUMC.ClientServices;
using ServiceCommon;
using System.Collections.Generic;
using System.Linq;
using UIControls;

namespace EUMC.Client
{
  internal class MultiRoomVM : ViewModelBase
  {
    public bool IsPanel { get; set; }
    public string Key { get; set; } = string.Empty;
    public int ItemRows => CONFIG.ItemRows;
    OpdMultiViewConfig.ContentConfig CONFIG;
    public MultiRoomVM(OpdMultiViewConfig o, RoomPanelConfig panel)
    {
      this.IsPanel = true;
      this.CONFIG = o.Config;
      this.RoomName = panel.RoomTitle;
      this.DoctorName = panel.DoctorTitle;
      for (int i = 0; i < this.ItemRows; i++)
      {
        string msg =  panel.WaitMesages.Count > i ? panel.WaitMesages[i] : panel.WaitMesages.Last();
        this.Patients.Add(new MultiRoomPatientVM(i + 1, msg));
      }
    }

    public MultiRoomVM(OpdMultiViewConfig o, PackageRoomConfig room)
    {
      this.CONFIG = o.Config;
      this.Key = room.Key;
      this.RoomName = room.RoomName;
      if (string.IsNullOrEmpty(this.RoomName))
      {
        switch (room.RoomType)
        {
          case "A": this.RoomName = $"진료실{room.RoomCode}"; break;
          default: this.RoomName = $"검사실{room.RoomCode}"; break;
        }
      }
      this.DeptName = room.DeptName;
      for (int i = 1; i <= this.ItemRows; i++)
      {
        this.Patients.Add(new MultiRoomPatientVM(i));
      }
    }
    public MultiRoomVM(OpdMultiViewConfig o)
    {
      this.CONFIG = o.Config;
      for (int i = 1; i <= this.ItemRows; i++)
      {
        this.Patients.Add(new MultiRoomPatientVM(i));
      }
    }

    public void Update(OPD_ROOM_INFO o)
    {
      this.update_room(o.Room);
      this.update_doctor(o.Doctor);
      this.update_wait_patients(o);
      this.update_delay_time(o.Room);
    }

    //========================================
    // 진료실 정보
    //========================================
    void update_room(ROOM_INFO o)
    {
      this.DeptName = o.DeptName;
      this.RoomName = o.RoomName;
    }
    public string RoomName { get => _roomName; set => Set(ref _roomName, value); }
    public string DeptName { get => _deptName; set => Set(ref _deptName, value); }
    string _roomName = string.Empty;
    string _deptName = string.Empty;
    //========================================
    // 의사 정보
    //========================================
    void update_doctor(DOCTOR_INFO o)
    {
      if (o != null)
      {
        this.DoctorName = o.DoctorName;
        this.DoctorDeptName = o.DoctorDeptName;
      }
      else
      {
        this.DoctorName = string.Empty;
        this.DoctorDeptName = string.Empty;
      }
    }
    public string DoctorName { get => _doctorName; set => Set(ref _doctorName, value); }
    public string DoctorDeptName { get => _doctorDeptName; set => Set(ref _doctorDeptName, value); }
    string _doctorDeptName = string.Empty;
    string _doctorName = string.Empty;
    //========================================
    // 대기중 환자 정보 (optional)
    //========================================
    void update_wait_patients(OPD_ROOM_INFO o)
    {
      var list = new List<string>();
      if (o.RoomPatient != null)
      {
        list.Add(o.RoomPatient.PatientName);
      }
      list.AddRange(o.WaitPatients.Select(x => x.PatientName));

      var patients = list.Take(this.ItemRows).ToList();
      for (int i = 0; i < this.ItemRows; i++)
      {
        this.Patients[i].PatientName = patients.Count > i ? patients[i] : string.Empty;
      }
    }
    public List<MultiRoomPatientVM> Patients { get; set; } = new List<MultiRoomPatientVM>();
    public string RoomPatientName { get => _roomPatientName; set => Set(ref _roomPatientName, value); }
    string _roomPatientName = string.Empty;

    //========================================
    // 지연시간
    //========================================
    void update_delay_time(ROOM_INFO o)
    {
      if (this.CONFIG.ShowDelayTime)
      {
        if (!string.IsNullOrEmpty(o.DelayReason))
        {
          var s = $"지연 사유 : {o.DelayReason}";
          if (o.DelayTime > 0)
          {
            s = $"{s} / 대기시간 : {o.DelayTime} 분";
          }
          this.DelayMessage = s;
          this.ShowDelay = true;
        }
        else
        {
          this.ShowDelay = false;
        }
      }
    }
    public bool SHowDelyPopup => this.ShowDelay && !string.IsNullOrEmpty(this.DelayMessage);
    public string DelayMessage { get => _delayMessage; set => Set(ref _delayMessage, value); }
    public bool ShowDelay { get => _showDelay; set => Set(ref _showDelay, value); }
    bool _showDelay = false;
    string _delayMessage = string.Empty;
  }
}