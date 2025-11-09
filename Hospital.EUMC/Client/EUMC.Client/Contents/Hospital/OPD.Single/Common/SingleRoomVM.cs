using EUMC.ClientServices;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using UIControls;

namespace EUMC.Client
{
  internal class SingleRoomVM : ViewModelBase
  {
    public int ItemRows { get; set; }
    public string Key { get; private set; }

    OpdSingleViewConfig.ContentConfig CONFIG;
    public SingleRoomVM(OpdSingleViewConfig o)
    {
      this.ItemRows = o.Config.ItemRows;
      this.Key = o.Room.GetKey();

      this.CONFIG = o.Config;
      this.DeptName = o.Room.DeptName;
      this.RoomName = o.Room.RoomName;

      for (int i = 0; i < this.ItemRows; i++)
      {
        var title = o.WaitMesages.Count > i ? o.WaitMesages[i] : o.WaitMesages.Last();
        this.Patients.Add(new SingleRoomPatientVM(i + 1, title));
      }
    }

    public void Update(OPD_ROOM_INFO o)
    {
      this.update_room(o.Room);
      this.update_doctor(o.Doctor);
      this.update_room_patient(o);
      this.update_wait_patients(o.WaitPatients);
      this.update_delay_time(o.Room);
    }

    //========================================
    // 진료중 환자 정보 (optional)
    //========================================
    void update_room_patient(OPD_ROOM_INFO o)
    {
      this.RoomPatientName = o.RoomPatient?.PatientName ?? string.Empty;
      this.IsRoomPatientVisible = !string.IsNullOrEmpty(this.RoomPatientName);
    }
    public string RoomPatientName { get => _roomPatientName; set => Set(ref _roomPatientName, value); }
    public bool IsRoomPatientVisible { get => _isRoomPatientVisible; set => Set(ref _isRoomPatientVisible, value); }

    string _roomPatientName = string.Empty;
    bool _isRoomPatientVisible = false;

    //========================================
    // 대기중 환자 정보 (optional)
    //========================================
    void update_wait_patients(List<PATIENT_INFO> list)
    {
      this.WaitCount = list.Count;
      var patients = list.Take(this.ItemRows).ToList();
      for (int i = 0; i < this.ItemRows; i++)
      {
        this.Patients[i].PatientName = patients.Count > i ? patients[i].PatientName : string.Empty;
      }
    }

    public List<SingleRoomPatientVM> Patients { get; set; } = new List<SingleRoomPatientVM>();
    public int WaitCount { get => _waitCount; set => Set(ref _waitCount, value); }
    int _waitCount = 0;

    //========================================
    // 진료실 정보
    //========================================
    void update_room(ROOM_INFO o)
    {
      this.DeptName = o.DeptName;
      this.RoomName = o.RoomName;
    }

    public string DeptName { get => _deptName; set => Set(ref _deptName, value); }
    public string RoomName { get => _roomName; set => Set(ref _roomName, value); }
    string _deptName = string.Empty;
    string _roomName = string.Empty;

    //========================================
    // 의사 정보
    //========================================
    void update_doctor(DOCTOR_INFO o)
    {
      if (o != null)
      {
        this.DoctorName = o.DoctorName;
        this.DoctorPhotoUrl = o.PhotoUrl;
        this.DoctorDeptName = o.DoctorDeptName;

        if (!string.IsNullOrEmpty(o.SpecialPart))
        {
          var part = o.SpecialPart.Split(',').Take(5);
          this.DoctorPart = string.Join(Environment.NewLine, part);
        }
        else
        {
          this.DoctorPart = string.Empty;
        }
      }
      else
      {
        this.DoctorName = string.Empty;
        this.DoctorPhotoUrl = string.Empty;
        this.DoctorDeptName = string.Empty;
        this.DoctorPart = string.Empty;
      }
    }

    public string DoctorName { get => _doctorName; set => Set(ref _doctorName, value); }
    public string DoctorDeptName { get => _doctorDeptName; set => Set(ref _doctorDeptName, value); }
    public string DoctorPart { get => _doctorPart; set => Set(ref _doctorPart, value); }
    public string DoctorPhotoUrl
    {
      get => _doctorPhotoUrl;
      set
      {
        if (Set(ref _doctorPhotoUrl, value))
        {
          this.Photo = null;
          if (!string.IsNullOrEmpty(value))
          {
            UIContextHelper.CheckBeginInvokeOnUI(async () =>
            {
              // ImageSource는 UI Thread에서 만들어야 한다
              this.Photo = await ImageLoader.GetImageFromUrlAsync(value);
            });
          }
        }
      }
    }
    public ImageSource Photo { get => _photo; set => Set(ref _photo, value); }

    string _doctorName = string.Empty;
    string _doctorDeptName = string.Empty;
    string _doctorPart = string.Empty;
    string _doctorPhotoUrl = string.Empty;
    ImageSource _photo = null;
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