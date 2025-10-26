/*
using DataSource.Database;

namespace DS.HospitalService;
internal class HospitalParamDictionary : ParamDictionary
{
  public HospitalParamDictionary HSP_TP_CD(string value)
  {
    this.Add("IN_HSP_TP_CD", value);
    return this;
  }
  public HospitalParamDictionary DEPT_CD(string? value = null)
  {
    this.Add("IN_DEPT_CD", value);
    return this;
  }
  public HospitalParamDictionary MED_DEPT_CD(string? value = null)
  {
    this.Add("IN_MED_DEPT_CD", value);
    return this;
  }
  public HospitalParamDictionary EXAM_RM_CD(string? value = null)
  {
    this.Add("IN_EXAM_RM_CD", value);
    return this;
  }
  public HospitalParamDictionary EXRM_TP_CD(string? value = null)
  {
    this.Add("IN_EXRM_TP_CD", value);
    return this;
  }
  public HospitalParamDictionary PT_NO(string? value = null)
  {
    this.Add("IN_PT_NO", value);
    return this;
  }
  public HospitalParamDictionary SEC_RRN(string? value = null)
  {
    this.Add("IN_SEC_RRN", value);
    return this;
  }
  public HospitalParamDictionary ER_AREA_CD(string? value = null)
  {
    this.Add("IN_ER_AREA_CD", value);
    return this;
  }
  public HospitalParamDictionary STATE(string? value = null)
  {
    this.Add("IN_STATE", value);
    return this;
  }
  public HospitalParamDictionary RM_NO(string? value = null)
  {
    this.Add("IN_RM_NO", value);
    return this;
  }
  public HospitalParamDictionary IP_ADDR(string? value = null)
  {
    this.Add("IN_IP_ADDR", value);
    return this;
  }
  public HospitalParamDictionary TRP_RSV_DT()
  {
    var dt = DateTime.Now;
    var dayfmt = $"{dt.Year}-{dt.Month.ToString("D2")}-{dt.Day.ToString("D2")}";

    this.Add("IN_TRP_RSV_DT", dayfmt);
    return this;
  }
}*/