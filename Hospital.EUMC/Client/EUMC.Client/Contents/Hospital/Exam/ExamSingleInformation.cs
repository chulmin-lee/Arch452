using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System.Collections.ObjectModel;
using System.Linq;

namespace EUMC.Client
{
  internal class ExamSingleInformation : ContentInformation
  {
    public ExamSingleViewConfig.ContentConfig CONFIG { get; set; }
    IContentViewModel Owner;
    public ExamSingleInformation(IContentViewModel owner, ExamSingleViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.Owner = owner;
      this.InitCallMessage(o.IsWideContent);
      this.CONFIG = o.Config;

      var room = o.Room;

      _room = new ROOM_INFO()
      {
        DeptName = room.DeptName,
        DeptCode = room.DeptCode,
        RoomCode = room.RoomCode,
        ShortRoomName = room.RoomCode,
        RoomName = room.RoomName,
        DurationTime = room.DurationTime,
      };

      this.Owner.ContentTitle = _room.RoomName;
      this.DurationTime = o.Room.DurationTime;

      for (int i = 0; i < this.CONFIG.ItemRows; i++)
      {
        this.Patients.Add(new PatientViewModel(i + 1));
      }
    }
    public bool Update(EXAM_RESP o)
    {
      var room = o.Rooms.Where(x => x.GroupKey == this.ROOM.GroupKey).FirstOrDefault();
      if (room != null)
      {
        this.Owner.ContentTitle = room.Room.RoomName;

        this.ROOM = room.Room;
        this.RoomPatient = room.RoomPatient;

        LOG.dc($"PatientCount: {o.WaitPatients.Count}, InRoom: {((this.RoomPatient != null) ? "1" : "0")}");

        var list = room.WaitPatients.Where(x => x.DeptCode == this.ROOM.DeptCode && x.RoomCode == this.ROOM.RoomCode).OrderBy(x => x.WaitNo).ToList();

        this.WaitCount = list.Count;

        this.Patients.Clear();
        for (int i = 0; i < this.CONFIG.ItemRows; i++)
        {
          var name = (list.Count > i) ? list[i].PatientName : string.Empty;
          this.Patients.Add(new PatientViewModel(i + 1, name));
        }
      }
      return true;
    }

    #region Popup
    public PATIENT_INFO RoomPatient
    {
      get => _roomPatient;
      set
      {
        if (Set(ref _roomPatient, value))
        {
          //var key = value?.GetKey();
          //if (value != null)
          //{
          //  if (_roomPatientKey != key)
          //  {
          //    _roomPatientKey = key;

          //    var room_msg = $"{this.ROOM?.RoomName ?? "검사실"} 안으로 들어오십시요";
          //    var speech = $"{value.PatientNameTTS}님 {room_msg}";
          //    this.call_message(value.PatientNameTTS, room_msg, speech);
          //  }
          //}
          //else
          //{
          //  _roomPatientKey = string.Empty;
          //}
        }
      }
    }
    internal bool CallPatient(CALL_PATIENT_NOTI o)
    {
      this.call_message(o.PatientNameTTS, o.CallMessage, o.Speech);
      return true;
    }
    #endregion Popup

    #region Binding
    public ObservableCollection<PatientViewModel> Patients { get; set; } = new ObservableCollection<PatientViewModel>();
    public ROOM_INFO ROOM { get => _room; set => Set(ref _room, value); }
    public string DurationTime { get; set; }
    public int WaitCount { get => _waitCount; set => Set(ref _waitCount, value); }

    #endregion Binding

    ROOM_INFO _room;
    PATIENT_INFO _roomPatient;
    string _roomPatientKey;
    int _waitCount = 0;
  }
}