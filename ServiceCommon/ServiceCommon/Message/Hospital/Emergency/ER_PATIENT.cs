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
    public string RadState { get; set; } = string.Empty;
    public string InOutState { get; set; } = string.Empty;
  }

  public class ER_PATIENT_GROUP : IGroupKeyData<bool>
  {
    public bool IsChild { get; set; }
    public List<ER_PATIENT_INFO> Patients { get; set; } = new List<ER_PATIENT_INFO>();
    public bool GroupKey => this.IsChild;
  }
}