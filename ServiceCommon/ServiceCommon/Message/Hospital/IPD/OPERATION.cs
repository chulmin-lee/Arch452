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
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public OPERATION_STATE StateCode { get; set; }
  }
  public enum OPERATION_STATE
  {
    None = 0,
    Waiting = 1, // 대기중
    Operating = 2, // 수술중
    RecoveryRoom = 3, // 회복실
    WardRoom = 4, // 병실
    IcuRoom = 5, // 중환자실
  }
}