using System.Collections.Generic;

namespace ServiceCommon
{
  // 내시경
  public class ENDO_REQ : ServiceMessage
  {
    public ENDO_REQ() : base(SERVICE_ID.ENDO) { }
  }

  public class ENDO_RESP : ServiceMessage
  {
    public List<ENDO_INFO> Patients { get; set; } = new List<ENDO_INFO>();
    public ENDO_RESP() : base(SERVICE_ID.ENDO) { }
    public ENDO_RESP(List<ENDO_INFO> d) : this()
    {
      this.Patients = d;
    }
  }

  public class ENDO_INFO
  {
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string PatientMaskedName { get; set; } = string.Empty;
    public STATE StateCode { get; set; }
    public string StateName { get; set; } = string.Empty;

    public enum STATE
    {
      None = 0,
      Waiting = 1,
      Inspecting = 2,
      Recovering = 3,
    }
  }
}