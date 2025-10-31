using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using UIControls;

namespace EUMC.Client
{
  internal class OfficeSingleInformation : ContentInformation
  {
    public OfficeSingleViewConfig.ContentConfig CONFIG { get; set; }
    public ObservableCollection<PatientViewModel> Patients { get; set; } = new ObservableCollection<PatientViewModel>();

    public OfficeSingleInformation(OfficeSingleViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.InitCallMessage(o.IsWideContent);
      this.CONFIG = o.Config;

      var room = o.Room;

      _room = new ROOM_INFO()
      {
        DeptName = room.DeptName,
        DeptCode = room.DeptCode,
        RoomCode = room.RoomCode,
        //ShortRoomName = room.RoomCode,
        RoomName = room.RoomName,
      };
      for (int i = 0; i < this.CONFIG.ItemRows; i++)
      {
        this.Patients.Add(new PatientViewModel(i + 1));
      }
    }

    public bool Update(OFFICE_RESP o)
    {
      lock (LOCK)
      {
        this.StopTimer();

        var room = o.Rooms.Where(x => x.GroupKey == this.ROOM.GroupKey).FirstOrDefault();

        if (room != null)
        {
          this.ROOM = room.Room;
          this.DOCTOR = room.Doctor;
          this.RoomPatient = room.RoomPatient;

          LOG.dc($"count: {room.WaitPatients.Count}");

          this.Patients.Clear();

          var list = room.WaitPatients.OrderBy(x => x.WaitNo).Take(this.CONFIG.ItemRows).ToList();

          for (int i = 0; i < this.CONFIG.ItemRows; i++)
          {
            var name = (list.Count > i) ? list[i].PatientName : string.Empty;
            this.Patients.Add(new PatientViewModel(i + 1, name));
          }
        }
        return true;
      }
    }

    internal bool CallPatient(CALL_PATIENT_NOTI o)
    {
      this.CallClear();
      this.call_message(o.PatientNameTTS, o.CallMessage, o.Speech);
      return true;
    }

    #region Photo
    public bool UpdatePhoto(DR_PHOTO_RESP o)
    {
      if (this.DoctorNo == o.DoctorNo)
      {
        this.UpdatePhoto(o.Photo);
      }
      else
      {
        LOG.ec($"{this.DoctorNo} != {o.DoctorNo}");
      }
      return true;
    }
    public bool PhotoNotify(DR_PHOTO_UPDATED o)
    {
      var dr_no = this.DoctorNo;
      if (o.DeletedDoctors.Contains(dr_no))
      {
        this.UpdatePhoto();
      }
      else if (o.UpdatedDoctors.Contains(dr_no))
      {
        this.UpdatePhoto();
        CLIENT_SERVICE.Send(new DR_PHOTO_REQ(dr_no));
      }
      return true;
    }
    void UpdatePhoto(string photo = null)
    {
      if (string.IsNullOrEmpty(photo))
      {
        this.Photo = null;
      }
      else
      {
        try
        {
          this.Photo = ImageLoader.BitmapFromBase64(photo);
        }
        catch (Exception ex)
        {
          LOG.except(ex);
        }
      }
    }
    #endregion Photo

    #region Binding
    public ROOM_INFO ROOM { get => _room; set => Set(ref _room, value); }
    public ImageSource Photo { get { return _photo; } set { Set(ref _photo, value); } }
    public DOCTOR_INFO DOCTOR
    {
      get => _doctor;
      set
      {
        if (Set(ref _doctor, value))
        {
          this.DoctorNo = value?.DoctorNo ?? string.Empty;
        }
      }
    }
    public string DoctorNo
    {
      get => _doctorNo;
      set
      {
        if (Set(ref _doctorNo, value))
        {
          this.Photo = null;
          if (!string.IsNullOrEmpty(value))
          {
            CLIENT_SERVICE.Send(new DR_PHOTO_REQ(value));
          }
        }
      }
    }
    public PATIENT_INFO RoomPatient
    {
      get => _roomPatient;
      set
      {
        if (Set(ref _roomPatient, value))
        {
          var key = value?.GetKey();
          if (value != null)
          {
            if (_roomPatientKey != key)
            {
              _roomPatientKey = key;

              string room_msg = "진료실 안으로 들어오십시요";
              if (this.ROOM != null)
              {
                room_msg = $"{this.ROOM.ShortRoomName}번 진료실로 들어오십시요";
              }
              var speech = $"{value.PatientNameTTS}님 {room_msg}";
              this.call_message(value.PatientNameTTS, room_msg, speech);
            }
          }
          else
          {
            _roomPatientKey = string.Empty;
          }
        }
      }
    }
    #endregion Binding

    ROOM_INFO _room;
    PATIENT_INFO _roomPatient;
    string _roomPatientKey;
    DOCTOR_INFO _doctor;
    string _doctorNo = string.Empty;
    ImageSource _photo = null;
  }
}