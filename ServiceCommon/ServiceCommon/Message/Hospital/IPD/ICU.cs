using System.Collections.Generic;

namespace ServiceCommon
{
  public class ICU_REQ : ServiceMessage
  {
    public List<string> WardCodes { get; set; } = new List<string>();
    public ICU_REQ() : base(SERVICE_ID.ICU) { }
    public ICU_REQ(List<string> d) : this()
    {
      this.WardCodes = d;
    }
  }

  public class ICU_RESP : ServiceMessage
  {
    public List<ICU_PT_INFO> Datas { get; set; } = new List<ICU_PT_INFO>();
    public ICU_RESP() : base(SERVICE_ID.ICU) { }
    public ICU_RESP(ICU_PT_INFO d) : this()
    {
      this.Datas.Add(d);
    }
    public ICU_RESP(List<ICU_PT_INFO> d) : this()
    {
      this.Datas.AddRange(d);
    }
  }

  public class ICU_PT_INFO : IGroupKeyData<string>
  {
    public string WardCode { get; set; } = string.Empty;
    public string WardName { get; set; } = string.Empty;
    public List<ICU_INFO> Patients { get; set; } = new List<ICU_INFO>();
    public string GroupKey => this.WardCode;
  }

  public class ICU_INFO
  {
    public string DeptName { get; set; } = string.Empty;
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string BedNo { get; set; } = string.Empty;

    public string DoctorName { get; set; } = string.Empty;
    public string Infections { get; set; } = string.Empty;
    public string Delimeter { get; set; } = string.Empty;

    // extension
    public string PatientInfo => $"({this.Gender} / {this.Age})";

    // not used
    public string WardCode { get; set; } = string.Empty;
    public string WardName { get; set; } = string.Empty;
    public string DeptCode { get; set; } = string.Empty;
    public string IcuCode { get; set; } = string.Empty;  // MICU, MECU,
    public string StateCode { get; set; } = string.Empty;
    public string StateName { get; set; } = string.Empty;
  }
}