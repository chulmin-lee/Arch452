using System.Collections.Generic;

namespace ServiceCommon
{
  public class OPERATION_REQ : ServiceMessage
  {
    public OPERATION_REQ() : base(SERVICE_ID.OPERATION) { }
  }

  public class OPERATION_RESP : ServiceMessage
  {
    public List<OPERATION_INFO> Patients { get; set; } = new List<OPERATION_INFO>();
    public OPERATION_RESP() : base(SERVICE_ID.OPERATION) { }
    public OPERATION_RESP(List<OPERATION_INFO> patients) : this()
    {
      this.Patients.AddRange(patients);
    }
  }
  public class OPERATION_INFO
  {
    public string BuildingNo { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string PatientNameTTS { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string DeptCode { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string StateCode { get; set; } = string.Empty;
    public string StateName { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public bool CallPatient { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public string PatientInfo => $"{this.Gender}/{this.Age}";

    // 2025/09/10 SCH 수술실 이름 (OpRoomNm)
    public string RoomName { get; set; } = string.Empty;
  }
}