using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace EUMC.Client
{
  internal class ErPatientInformation : ContentInformation
  {
    public ErPatientViewConfig.ContentConfig CONFIG { get; set; }
    public ObservableCollection<EmergencyPatientModel> Patients { get; set; } = new ObservableCollection<EmergencyPatientModel>();
    List<EmergencyPatientModel> _all_items = new List<EmergencyPatientModel>();
    public bool IsChild { get; set; }
    int _congestionPercent;
    public int CongestionPercent { get => _congestionPercent; set => Set(ref _congestionPercent, value); }

    int _patientCount;
    public int PatientCount { get => _patientCount; set => Set(ref _patientCount, value); }

    int _averageinTime;
    public int AverageInTime { get => _averageinTime; set => Set(ref _averageinTime, value); }

    public ErPatientInformation(ErPatientViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.InitCallMessage(o.IsWideContent);
      this.CONFIG = o.Config;
      this.IsChild = o.IsChild;

      this.PageItemCount = this.CONFIG.ItemRows;
    }
    public bool Update(ER_PATIENT_RESP o)
    {
      lock (LOCK)
      {
        this.StopTimer();

        LOG.dc($"count : {o.Patients.Count}");

        // update & draw
        {
          _all_items.Clear();
          o.Patients.ForEach(x => _all_items.Add(new EmergencyPatientModel(x)));
          this.PAGE.SetPage(this.GetPageCount());
          this.Refresh();
        }

        if (this.PAGE.IsRotate)
        {
          this.StartTimer(this.Refresh);
        }
        else
        {
          LoopCounter = 0;
        }
        return true;
      }
    }
    internal bool UpdateCongestion(ER_CONGESTION_RESP o)
    {
      this.CongestionPercent = o.TotalPercent;
      return true;
    }

    internal bool UpdateStatistics(ER_STATISTICS_RESP o)
    {
      this.PatientCount = o.PatientCount;
      this.AverageInTime = o.AverageInTime;
      return true;
    }

    void Refresh()
    {
      lock (LOCK)
      {
        int page = this.PAGE.RotatePage(this.LoopCounter++);
        this.Patients.Clear();
        _all_items.GetPageItems(page, this.PageItemCount)
                  .ForEach(x => this.Patients.Add(x));
      }
    }
    protected override int GetPageCount()
    {
      return _all_items.CalcPageCount(this.PageItemCount);
    }
    internal bool CallPatient(CALL_PATIENT_NOTI o)
    {
      this.call_message(o.PatientNameTTS, o.CallMessage, o.Speech);
      return true;
    }
  }

  public class EmergencyPatientModel
  {
    public string PatientName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public ER_KTAS KtasCode { get; set; }

    public bool IsFirstVisit { get; set; }

    public EmergencyPatientState DoctorState { get; set; }
    public EmergencyPatientState BloodState { get; set; }
    public EmergencyPatientState ConState { get; set; }
    public EmergencyPatientState RadState { get; set; }
    public EmergencyPatientState InOutState { get; set; }

    public ER_RADIO_STATE RadioStateCode { get; set; }
    public ER_BLOOD_STATE BloodStateCode { get; set; }
    public ER_COLLABO_STATE CollaboStateCode { get; set; }
    public ER_HOSPITALIZED_STATE HospitalStateCode { get; set; }
    public ER_MEDICAL_STATE MedicalStateCode { get; set; }
    public ER_WARD_STATE WardStateCode { get; set; }

    public EmergencyPatientModel(ER_PATIENT_INFO o)
    {
      PatientName = o.PatientName;
      Gender = o.Gender;
      Age = o.Age;
      DeptName = o.DeptName;
      AreaName = o.RoomName;
      KtasCode = o.KtasCode;

      this.IsFirstVisit = o.IsFirstVisit;
      this.RadioStateCode = o.RadioStateCode;
      this.BloodStateCode = o.BloodStateCode;
      this.CollaboStateCode = o.CollaboStateCode;
      this.HospitalStateCode = o.HospitalStateCode;
      this.MedicalStateCode = o.MedicalStateCode;
      this.WardStateCode = o.WardStateCode;

      DoctorState = new EmergencyPatientState(o.DoctorState, "#555555");
      BloodState = new EmergencyPatientState(o.BloodState, "#EE0043");
      ConState = new EmergencyPatientState(o.ConState, "#125F50");
      RadState = new EmergencyPatientState(o.RadioState, "#F68D38");
      InOutState = new EmergencyPatientState(o.InOutState, "#14CA66");
    }
  }
  public class EmergencyPatientState
  {
    public bool Finished { get; set; }
    public bool Hide { get; set; }
    public string State { get; set; }
    public Brush Color { get; set; }

    public EmergencyPatientState(string state, string color)
    {
      this.State = state;
      this.Hide = string.IsNullOrEmpty(state);
      this.Color = (Brush)new BrushConverter().ConvertFromString(string.IsNullOrEmpty(color) ? "#FFFFFFFF" : color);
      this.Finished = state == "완료" || state == "퇴원";
    }
  }
}