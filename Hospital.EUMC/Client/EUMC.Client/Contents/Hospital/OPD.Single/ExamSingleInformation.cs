using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UIControls;

namespace EUMC.Client
{
  internal class ExamSingleInformation : ContentInformation
  {
    public ExamSingleViewConfig.ContentConfig CONFIG { get; set; }
    public ObservableCollection<SinglePatientViewModel> Patients { get; set; } = new ObservableCollection<SinglePatientViewModel>();

    public ExamSingleInformation(ExamSingleViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.InitCallMessage(o.IsWideContent);
      this.CONFIG = o.Config;

      var room = o.Room;

      this.ROOM = new ROOM_INFO()
      {
        DeptName = room.DeptName,
        DeptCode = room.DeptCode,
        RoomCode = room.RoomCode,
        RoomName = room.RoomName,
      };
      for (int i = 0; i < this.CONFIG.ItemRows; i++)
      {
        this.Patients.Add(new SinglePatientViewModel(i + 1));
      }
    }

    public bool Update(EXAM_RESP o)
    {
      lock (LOCK)
      {
        //this.StopTimer();

        var room = o.Rooms.Where(x => x.GroupKey == this.ROOM.GroupKey).FirstOrDefault();

        if (room != null)
        {
          this.ROOM = room.Room;
          this.DOCTOR = room.Doctor;

          this.RoomPatient = room.RoomPatient;
          this.Patients.Clear();

          var list = room.WaitPatients.OrderBy(x => x.WaitNo).Take(this.CONFIG.ItemRows).ToList();
          for (int i = 0; i < this.CONFIG.ItemRows; i++)
          {
            var name = (list.Count > i) ? list[i].PatientName : string.Empty;
            this.Patients.Add(new SinglePatientViewModel(i + 1, name));
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

    #region Binding
    public ROOM_INFO ROOM { get => _room; set => Set(ref _room, value); }
    public DOCTOR_INFO DOCTOR { get => _doctor; set => Set(ref _doctor, value); }
    bool _roomPatientExist;
    public bool RoomPatientExist { get => _roomPatientExist; set => Set(ref _roomPatientExist, value); }
    public PATIENT_INFO RoomPatient
    {
      get => _roomPatient;
      set
      {
        if (Set(ref _roomPatient, value))
        {
          this.RoomPatientExist = value != null;
        }
      }
    }
    #endregion Binding

    ROOM_INFO _room;
    PATIENT_INFO _roomPatient;
    string _roomPatientKey;
    DOCTOR_INFO _doctor;
    string _doctorNo = string.Empty;
  }
}
