using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCommon
{
  // 방사선종양학과 검사실 환자
  public class RAD_TR_PT_REQ : ServiceMessage
  {
    public string RoomCode { get; set; }
    public RAD_TR_PT_REQ() : base(SERVICE_ID.RAD_TR) { }
    public RAD_TR_PT_REQ(string roomCode) : this()
    {
      this.RoomCode = roomCode;
    }
  }
  public class RAD_TR_PT_RESP : ServiceMessage
  {
    public List<RAD_TR_PT_INFO> Patients { get; set; } = new List<RAD_TR_PT_INFO>();
    public RAD_TR_PT_RESP() : base(SERVICE_ID.RAD_TR) { }
    public RAD_TR_PT_RESP(List<RAD_TR_PT_INFO> p) : this()
    {
      this.Patients = p;
    }
  }


  public class RAD_TR_PT_INFO : IGroupKeyData<string>
  {
    public string DeptCode { get; set; } = "TR";
    public string DeptName { get; set; } = "방사선종양학과";
    public string RoomCode { get; set; }
    public string RoomName { get; set; }
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string GroupKey => this.RoomCode;
  }

}
