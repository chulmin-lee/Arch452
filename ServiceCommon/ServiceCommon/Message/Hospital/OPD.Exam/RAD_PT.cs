using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCommon
{
  public class RAD_PT_REQ : ServiceMessage
  {
    public RAD_PT_REQ() :base(SERVICE_ID.RAD_SEOUL)
    {
    }
  }
  public class RAD_PT_RESP : ServiceMessage
  {
    public List<RAD_PT_INFO> Patients { get; set; } = new List<RAD_PT_INFO>();
    public RAD_PT_RESP() : base(SERVICE_ID.RAD_SEOUL) { }
    public RAD_PT_RESP(List<RAD_PT_INFO> p) : this()
    {
      this.Patients = p;
    }
  }


  public class RAD_PT_INFO
  {
    public string RoomCode { get; set; }
    public string RoomName { get; set; }
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public int WaitNo { get; set; }
  }
}
