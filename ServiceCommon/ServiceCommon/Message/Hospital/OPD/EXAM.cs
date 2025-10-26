using System.Collections.Generic;

namespace ServiceCommon
{
  public class EXAM_REQ : ServiceMessage
  {
    public string DeptCode { get; set; } = string.Empty;
    public List<string> RoomCodes { get; set; } = new List<string>();
    public EXAM_REQ() : base(SERVICE_ID.EXAM_ROOM) { }
    public EXAM_REQ(string dept, string room_no) : this()
    {
      this.DeptCode = dept;
      this.RoomCodes.Add(room_no);
    }
    public EXAM_REQ(string dept, List<string> room_nos) : this()
    {
      this.DeptCode = dept;
      this.RoomCodes.AddRange(room_nos);
    }
  }

  public class EXAM_RESP : ServiceMessage
  {
    public List<OPD_ROOM_INFO> Rooms { get; set; } = new List<OPD_ROOM_INFO>();
    public List<PATIENT_INFO> WaitPatients { get; set; } = new List<PATIENT_INFO>();
    public EXAM_RESP() : base(SERVICE_ID.EXAM_ROOM) { }
    public EXAM_RESP(OPD_ROOM_INFO d) : this()
    {
      this.Rooms.Add(d);
    }
    public EXAM_RESP(List<OPD_ROOM_INFO> d) : this()
    {
      this.Rooms = d;
    }
    public EXAM_RESP(List<OPD_ROOM_INFO> d, List<PATIENT_INFO> patients) : this()
    {
      this.Rooms = d;
      this.WaitPatients = patients;
    }
  }
}