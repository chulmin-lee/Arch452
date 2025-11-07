using Common;
using EUMC.Client;
using EUMC.ClientServices;
using ServiceCommon;
using ServiceCommon.ClientServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using UIControls;

namespace EUMC.Client
{
  internal class SingleRoomViewModel : ViewModelBase
  {
    public string DeptName { get => _deptName; set => Set(ref _deptName, value); }
    public string RoomName { get => _roomName; set => Set(ref _roomName, value); }
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
    public string RoomPatientName { get => _roomPatientName; set => Set(ref _roomPatientName, value); }
    public bool IsRoomPatientVisible { get => _isRoomPatientVisible; set => Set(ref _isRoomPatientVisible, value); }
    public List<SinglePatientViewModel> Patients { get; set; } = new List<SinglePatientViewModel>();
    public int WaitCount { get => _waitCount; set => Set(ref _waitCount, value); }
    public string DelayReason { get => _delayReason; set => Set(ref _delayReason, value); }
    public string DelayTime { get => _delayTime; set => Set(ref _delayTime, value); }

    int ItemRows = 0;
    public string Key { get; private set; }

    public SingleRoomViewModel(int item_row, OpdRoomConfig room, List<string> titles)
    {
      this.ItemRows = item_row;
      this.Key = room.GetKey();
      this.DeptName = room.DeptName;
      this.RoomName = room.RoomName;

      for (int i = 0; i < this.ItemRows; i++)
      {
        var title = titles.Count > i ? titles[i] : titles.Last();
        this.Patients.Add(new SinglePatientViewModel(i + 1, title));
      }
    }

    public void Update(OPD_ROOM_INFO o)
    {
      // room info
      this.DeptName = o.Room.DeptName;
      this.RoomName = o.Room.RoomName;

      // doctor info
      this.DoctorPhotoUrl = o.Doctor?.PhotoUrl ?? string.Empty;
      this.DoctorName = o.Doctor?.DoctorName ?? string.Empty;

      if (!string.IsNullOrEmpty(o.Doctor?.SpecialPart))
      {
        var part = o.Doctor.SpecialPart.Split(',').Take(5);
        this.DoctorPart = string.Join(Environment.NewLine, part);
      }
      else
      {
        this.DoctorPart = string.Empty;
      }

      // room patient info
      this.RoomPatientName = o.RoomPatient?.PatientName ?? string.Empty;
      this.IsRoomPatientVisible = !string.IsNullOrEmpty(this.RoomPatientName);

      // wait patients info
      this.WaitCount = o.WaitPatients.Count;
      var patient = o.WaitPatients.Take(this.ItemRows).ToList();
      for (int i = 0; i < this.ItemRows; i++)
      {
        if (patient.Count > i)
        {
          this.Patients[i].PatientName = patient[i].PatientName;
        }
        else
        {
          this.Patients[i].Clear();
        }
      }
    }

    string _deptName = string.Empty;
    string _roomName = string.Empty;
    string _doctorName = string.Empty;
    string _doctorDeptName = string.Empty;
    string _doctorPart = string.Empty;
    string _doctorPhotoUrl = string.Empty;
    ImageSource _photo = null;
    string _roomPatientName = string.Empty;
    bool _isRoomPatientVisible = false;
    int _waitCount = 0;
    string _delayReason = string.Empty;
    string _delayTime = string.Empty;
  }
}