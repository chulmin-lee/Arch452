using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EUMC.Client
{
  internal class ExamMultiInformation : ContentInformation
  {
    public ExamRoomViewModel RoomPanel { get; set; }
    public ObservableCollection<ExamRoomViewModel> Rooms { get; set; } = new ObservableCollection<ExamRoomViewModel>();
    List<ExamRoomViewModel> _all_rooms { get; set; } = new List<ExamRoomViewModel>();
    List<ExamRoomViewModel> _active_rooms { get; set; } = new List<ExamRoomViewModel>();
    public ExamMultiViewConfig.ContentConfig CONFIG { get; set; }
    public ExamMultiInformation(ExamMultiViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.InitCallMessage(o.IsWideContent);
      this.CONFIG = o.Config;

      int rows = o.Config.ItemRows;

      this.RoomPanel = new ExamRoomViewModel(rows);

      foreach (var room in o.ExamRooms)
      {
        var p = new ExamRoomViewModel(room.GetKey(), rows, room.RoomName);
        p.RoomPatientChanged += (s, e) =>
        {
          this.call_message(e);
        };
        _all_rooms.Add(p);
        _active_rooms.Add(p);
      }
      this.PAGE.SetPage(this.GetPageCount());
      this.GetPageItems().ForEach(x => this.Rooms.Add(x));
    }

    public bool Update(EXAM_RESP o)
    {
      lock (LOCK)
      {
        //this.StopTimer();

        foreach (var p in o.Rooms)
        {
          var find = _all_rooms.Where(x => x.Key == p.GroupKey).FirstOrDefault();

          if (find != null)
          {
            LOG.dc($"OfficePatient: {p.GroupKey} - {p.WaitPatients.Count}, InRoom: {((p.RoomPatient != null) ? "1" : "0")}");
            find.Update(p);
          }
          else
          {
            LOG.ec($"{p.GroupKey} not found");
          }
        }

        _active_rooms = _all_rooms.Where(x => x.UseRoom).ToList();
        this.PAGE.SetPage(this.GetPageCount());
        this.Refresh();

        //if (this.PAGE.IsRotate)
        //{
        //  this.StartTimer(this.Refresh);
        //}
        //else
        //{
        //  loopCounter = 0;
        //}
        return true;
      }
    }
    void Refresh()
    {
      lock (LOCK)
      {
        this.Rooms.Clear();
        this.GetPageItems().ForEach(x => this.Rooms.Add(x));
      }
    }
    protected override int GetPageCount()
    {
      return _active_rooms.CalcPageCount(this.CONFIG.RoomPerPage);
    }
    List<ExamRoomViewModel> GetPageItems()
    {
      int pageIndex = this.PAGE.RotatePage(this.LoopCounter++);
      return _active_rooms.GetPageItems(pageIndex, this.CONFIG.RoomPerPage);
    }
    internal bool CallPatient(CALL_PATIENT_NOTI o)
    {
      this.call_message(o.PatientNameTTS, o.CallMessage, o.Speech);
      return true;
    }
    internal bool CallAnnounce(CALL_ANNOUNCE_NOTI o)
    {
      this.call_message(o.Message);
      return true;
    }
  }

  internal class ExamRoomViewModel : ContentInformation
  {
    #region Common
    public string Key { get; set; } = string.Empty;
    public int ItemRows { get; set; }
    public int ItemColumns { get; set; }
    #endregion Common

    public event EventHandler<string> RoomPatientChanged;
    public string RoomName { get => _roomName; set => Set(ref _roomName, value); }
    public string InRoomPatientName { get => _inRoomPatientName; set => Set(ref _inRoomPatientName, value); }

    public ObservableCollection<PatientViewModel> Patients { get; set; } = new ObservableCollection<PatientViewModel>();

    public bool UseRoom { get; set; }
    public bool IsRotation { get; set; }
    public bool IsPanel { get; set; }
    public ExamRoomViewModel(string key, int rows, string room_name) : base(ClientViewManager.RotationInterval / 2)
    {
      LOG.dc($"key: {key}, name: {room_name}");
      this.Key = key;
      this.ItemRows = rows;
      this.RoomName = room_name;

      for (int i = 1; i <= rows; i++)
      {
        this.Patients.Add(new PatientViewModel(i, string.Empty, false));
      }
    }

    public ExamRoomViewModel(int rows) : base(ClientViewManager.RotationInterval / 2)
    {
      this.IsPanel = true;
      this.ItemRows = rows;
      this.RoomName = "검사실";
      this.InRoomPatientName = "검사중";

      this.Patients.Add(new PatientViewModel(1, "다음 순서 입니다", true));
      for (int i = 2; i <= this.ItemRows; i++)
      {
        this.Patients.Add(new PatientViewModel(i, "대기해주세요", true));
      }
    }

    public void Update(OPD_ROOM_INFO o)
    {
      lock (LOCK)
      {
        this.RoomName = o.Room.RoomName;
        this.UseRoom = o.Room.UseRoom;

        if (o.RoomPatient != null)
        {
          this.InRoomPatientName = o.RoomPatient.PatientName;
        }
        else
        {
          this.InRoomPatientName = "";
        }

        _all_patients.Clear();
        int index = 1;
        o.WaitPatients.ForEach(x => _all_patients.Add(new PatientViewModel(index++, x.PatientName)));

        var list = _all_patients.Take(this.ItemRows);
        this.Patients.Clear();
        foreach (var p in list)
        {
          this.Patients.Add(p);
        }

        var remained = this.ItemRows - Patients.Count;
        for (int i = 0; i < remained; i++)
        {
          this.Patients.Add(new PatientViewModel(index++, string.Empty, false));
        }

        //this.IsRotation = this.UseRoom && (this.ItemRows < _all_patients.Count);

        //this.Refresh();

        //if (this.IsRotation)
        //{
        //  this.StartTimer(Refresh);
        //}
        //else
        //{
        //  loopCounter = 0;
        //}
      }
    }

    void Refresh()
    {
      lock (LOCK)
      {
        this.Patients.Clear();
        this.GetPageItems().ForEach(x => this.Patients.Add(x));
      }
    }
    protected override int GetPageCount()
    {
      return _all_patients.CalcPageCount(this.ItemRows);
    }
    List<PatientViewModel> GetPageItems()
    {
      int pageIndex = this.GetPageCount().GetCurrentPage(this.LoopCounter++);
      return _all_patients.GetPageItems(pageIndex, this.ItemRows);
    }

    List<PatientViewModel> _all_patients = new List<PatientViewModel>();
    string _roomName = string.Empty;
    string _inRoomPatientName = string.Empty;
    PATIENT_INFO _roomPatient;
    string _roomPatientKey;
  }
}