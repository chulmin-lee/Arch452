using System.Collections.Generic;

namespace ServiceCommon
{
  // 내시경
  public class ENDO_REQ : ServiceMessage
  {
    public ENDO_TYPE Type { get; set; }
    public ENDO_REQ() : base(SERVICE_ID.ENDO) { }
    public ENDO_REQ(ENDO_TYPE type) : this()
    {
      this.Type = type;
    }
  }

  public class ENDO_RESP : ServiceMessage
  {
    public List<ENDO_PT_INFO> Patients { get; set; } = new List<ENDO_PT_INFO>();
    public ENDO_RESP() : base(SERVICE_ID.ENDO) { }
    public ENDO_RESP(ENDO_PT_INFO d) : this()
    {
      this.Patients.Add(d);
    }
    public ENDO_RESP(List<ENDO_PT_INFO> d) : this()
    {
      this.Patients = d;
    }
  }

  public class ENDO_PT_INFO : IGroupKeyData<ENDO_TYPE>
  {
    public ENDO_TYPE Type { get; set; }
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Gubn { get; set; } = string.Empty;

    public ENDO_TYPE GroupKey => this.Type;
  }
  public enum ENDO_TYPE
  {
    None,
    Normal =1,
    WGO = 2
  }
}