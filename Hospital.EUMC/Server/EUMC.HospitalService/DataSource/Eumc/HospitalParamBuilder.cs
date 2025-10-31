using Framework.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.HospitalService
{
  internal class HospitalParamBuilder : ParamBuilder
  {
    public HospitalParamBuilder()
    {
    }
    public HospitalParamBuilder HSP_TP_CD(string value)
    {
      this.Add("IN_HSP_TP_CD", value);
      return this;
    }
    public HospitalParamBuilder DEPT_CD(string value = null)
    {
      this.Add("IN_DEPT_CD", value);
      return this;
    }
    public HospitalParamBuilder MED_DEPT_CD(string value = null)
    {
      this.Add("IN_MED_DEPT_CD", value);
      return this;
    }
    public HospitalParamBuilder EXAM_RM_CD(string value = null)
    {
      this.Add("IN_EXAM_RM_CD", value);
      return this;
    }
    public HospitalParamBuilder EXRM_TP_CD(string value = null)
    {
      this.Add("IN_EXRM_TP_CD", value);
      return this;
    }
    public HospitalParamBuilder PT_NO(string value = null)
    {
      this.Add("IN_PT_NO", value);
      return this;
    }
    public HospitalParamBuilder SEC_RRN(string value = null)
    {
      this.Add("IN_SEC_RRN", value);
      return this;
    }
    public HospitalParamBuilder ER_AREA_CD(string value = null)
    {
      this.Add("IN_ER_AREA_CD", value);
      return this;
    }
    public HospitalParamBuilder STATE(string value = null)
    {
      this.Add("IN_STATE", value);
      return this;
    }
    public HospitalParamBuilder RM_NO(string value = null)
    {
      this.Add("IN_RM_NO", value);
      return this;
    }
    public HospitalParamBuilder IP_ADDR(string value = null)
    {
      this.Add("IN_IP_ADDR", value);
      return this;
    }
    public HospitalParamBuilder TRP_RSV_DT()
    {
      var dt = DateTime.Now;
      var dayfmt = $"{dt.Year}-{dt.Month.ToString("D2")}-{dt.Day.ToString("D2")}";

      this.Add("IN_TRP_RSV_DT", dayfmt);
      return this;
    }
  }
}
