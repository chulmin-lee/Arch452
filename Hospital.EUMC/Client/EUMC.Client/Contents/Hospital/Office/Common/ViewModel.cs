using Common;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.Client
{
  internal class OfficeRoomViewModel : ContentInformation
  {
    #region Common
    public string Key { get; set; } = string.Empty;
    public int ItemRows { get; set; }
    public int ItemColumns { get; set; }
    #endregion Common

    public event EventHandler<string> RoomPatientChanged;
    public string RoomName { get => _roomName; set => Set(ref _roomName, value); }
    public string DoctorName { get => _doctorName; set => Set(ref _doctorName, value); }
    public string DeptName { get => _deptName; set => Set(ref _deptName, value); }
    public string InRoomPatientName { get => _inRoomPatientName; set => Set(ref _inRoomPatientName, value); }

    public ObservableCollection<PatientViewModel> Patients { get; set; } = new ObservableCollection<PatientViewModel>();

    public bool UseRoom { get; set; }
    public bool IsRotation { get; set; }
    public bool IsPanel { get; set; }
    public OfficeRoomViewModel(string key, int rows, string room_name) : base(ClientViewManager.RotationInterval / 2)
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

    public OfficeRoomViewModel(int rows) : base(ClientViewManager.RotationInterval / 2)
    {
      this.IsPanel = true;
      this.ItemRows = rows;
      this.RoomName = "진료실·진료센터";
      this.DoctorName = "진료의사";
      this.InRoomPatientName = "진료중";

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
        //this.StopTimer();
        this.DoctorName = o.Doctor?.DoctorName ?? string.Empty;
        this.DeptName = o.Room.DeptName;
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
        o.WaitPatients.ForEach(x => _all_patients.Add(new PatientViewModel(index++, x.PatientName, false)));

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

    //public PATIENT_INFO? RoomPatient
    //{
    //  get => _roomPatient;
    //  set
    //  {
    //    if (Set(ref _roomPatient, value))
    //    {
    //      var key = value?.GetKey();
    //      if (value != null)
    //      {
    //        if (this.UseRoom && _roomPatientKey != key)
    //        {
    //          _roomPatientKey = key;
    //          var speech = $"{value.PatientNameTTS}님 {this.RoomName}번 진료실로 들어오십시요";
    //          this.RoomPatientChanged?.Invoke(this, speech);
    //        }
    //      }
    //      else
    //      {
    //        _roomPatientKey = string.Empty;
    //      }
    //    }
    //  }
    //}

    List<PatientViewModel> _all_patients = new List<PatientViewModel>();
    string _roomName = string.Empty;
    string _doctorName = string.Empty;
    string _deptName = string.Empty;
    string _inRoomPatientName = string.Empty;
    PATIENT_INFO _roomPatient;
    string _roomPatientKey;
  }

  internal class PatientViewModel
  {
    public int Index { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsPanel { get; set; }
    public PatientViewModel(int index, string name, bool isPanel)
    {
      this.Index = index;
      this.PatientName = name;
      this.IsPanel = isPanel;
    }

    public PatientViewModel(int index, string name)
    {
      this.Index = index;
      this.PatientName = name;
    }
    public PatientViewModel(int index)
    {
      this.Index = index;
    }
  }
}
