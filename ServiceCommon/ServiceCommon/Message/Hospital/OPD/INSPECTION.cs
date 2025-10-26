using System.Collections.Generic;

namespace ServiceCommon
{
  public class INSPECTION_REQ : ServiceMessage
  {
    public int Type { get; set; }
    public INSPECTION_REQ() : base(SERVICE_ID.INSPECTION) { }
    public INSPECTION_REQ(int type) : this()
    {
      this.Type = type;
    }
  }

  public class INSPECTION_RESP : ServiceMessage
  {
    public int Type { get; set; }
    public List<INSPECTION_INFO> Patients { get; set; } = new List<INSPECTION_INFO>();

    public INSPECTION_RESP() : base(SERVICE_ID.INSPECTION) { }

    public INSPECTION_RESP(List<INSPECTION_INFO> resp) : this()
    {
      this.Patients = resp;
    }
  }

  public class INSPECTION_INFO
  {
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Gubun { get; set; } = string.Empty;
    public string DelayTime { get; set; } = string.Empty;
    public string PatientDetail => $"{this.PatientNo}     {this.PatientName}";
  }
}