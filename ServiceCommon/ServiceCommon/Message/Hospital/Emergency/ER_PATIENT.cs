using System.Collections.Generic;

namespace ServiceCommon
{
  public class ER_PATIENT_REQ : ServiceMessage
  {
    public bool IsChild { get; set; }
    public ER_PATIENT_REQ() : base(SERVICE_ID.ER_PATIENT) { }
    public ER_PATIENT_REQ(bool child) : this()
    {
      this.IsChild = child;
    }
  }

  public class ER_PATIENT_RESP : ServiceMessage
  {
    public bool IsChild { get; set; }
    public List<ER_PATIENT_INFO> Patients { get; set; } = new List<ER_PATIENT_INFO>();
    public ER_PATIENT_RESP() : base(SERVICE_ID.ER_PATIENT) { }
    public ER_PATIENT_RESP(bool isChild) : this()
    {
      this.IsChild = isChild;
    }
    public ER_PATIENT_RESP(List<ER_PATIENT_INFO> d, bool isChild) : this()
    {
      this.Patients = d;
      this.IsChild = isChild;
    }
  }

  public class ER_PATIENT_INFO
  {
    public string AreaCode { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    // 환자
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;

    public string DoctorState { get; set; } = string.Empty;
    public string BloodState { get; set; } = string.Empty;
    public string ConState { get; set; } = string.Empty;
    public string RadioState { get; set; } = string.Empty;
    public string InOutState { get; set; } = string.Empty;
  }
  public enum ER_RADIO_STATE
  {
    None = 0,
    Wait = 1,     // 대기
    Finished = 2, // 완료
  }

  public enum ER_BLOOD_STATE
  {
    None = 0,
    Progress = 1,  // 진행
    Inspecting   = 2,  // 검사중
    Finished = 3,  // 완료
  }
  // 진료 상태
  public enum ER_MEDICAL_STATE
  {
    None = 0,
    Progress = 1, // 진행
    Visited = 2,   // 방문
  }
  // 협진
  public enum ER_COLLABO_STATE
  {
    None = 0,
    Progress = 1,  // 진행
    Visited = 2,    // 방문
  }
  // 입퇴원
  public enum ER_HOSPITALIZED_STATE
  {
    None = 0,
    Hospitalization = 1, // 입원
    Discharged = 2,   // 퇴원
  }

  // 병실 배정
  public enum ER_WARD_STATE
  {
    None = 0,
    Waiting = 1,  // 대기
    Assigned = 2, // 배정
  }
  public class ER_PATIENT_GROUP : IGroupKeyData<bool>
  {
    public bool IsChild { get; set; }
    public List<ER_PATIENT_INFO> Patients { get; set; } = new List<ER_PATIENT_INFO>();
    public bool GroupKey => this.IsChild;
  }
}