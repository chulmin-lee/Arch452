using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCommon
{
  public class ER_CT_REQ : ServiceMessage
  {
    public ER_CT_REQ() :base(SERVICE_ID.ER_CT) { }
  }

  public class ER_CT_RESP : ServiceMessage
  {
    public List<ER_CT_INFO> Patients { get; set; } = new List<ER_CT_INFO>();
    public ER_CT_RESP() : base(SERVICE_ID.ER_CT) { }
    public ER_CT_RESP(List<ER_CT_INFO> p) : this()
    {
      this.Patients = p;
    }
  }



  public class ER_CT_INFO
  {
    public bool IsExam { get; set; } // true: 검사중, false: 처방받음
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int WaitNo { get; set; }
  }
}
