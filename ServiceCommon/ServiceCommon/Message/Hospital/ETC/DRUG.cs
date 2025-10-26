using System.Collections.Generic;

namespace ServiceCommon
{
  public class DRUG_REQ : ServiceMessage
  {
    public DRUG_REQ() : base(SERVICE_ID.DRUG) { }
  }

  public class DRUG_RESP : ServiceMessage
  {
    public List<DRUG_INFO> Drugs { get; set; } = new List<DRUG_INFO>();
    public DRUG_RESP() : base(SERVICE_ID.DRUG) { }
    public DRUG_RESP(List<DRUG_INFO> drugs) : this()
    {
      this.Drugs.AddRange(drugs);
    }
  }

  public class DRUG_INFO
  {
    public DRUG_INFO() { }
    public DRUG_INFO(string name, string no, string deptnm, bool done)
    {
      this.PatientName = name;
      this.DrugNo = no;
      this.DeptName = deptnm;
      this.IsDone = done;
    }
    public string DrugNo { get; set; } = string.Empty;
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public bool IsDone { get; set; }
  }
}