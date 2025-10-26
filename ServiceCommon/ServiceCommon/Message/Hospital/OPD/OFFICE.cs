using System.Collections.Generic;

namespace ServiceCommon
{
  public class OFFICE_REQ : ServiceMessage
  {
    public string DeptCode { get; set; } = string.Empty;
    public List<string> RoomCodes { get; set; } = new List<string>();

    public OFFICE_REQ() : base(SERVICE_ID.OFFICE_ROOM) { }
    public OFFICE_REQ(string dept, string room_no) : this()
    {
      this.DeptCode = dept;
      this.RoomCodes.Add(room_no);
    }
    public OFFICE_REQ(string dept, List<string> room_nos) : this()
    {
      this.DeptCode = dept;
      this.RoomCodes.AddRange(room_nos);
    }
  }
  public class OFFICE_RESP : ServiceMessage
  {
    public List<OPD_ROOM_INFO> Rooms { get; set; } = new List<OPD_ROOM_INFO>();
    public OFFICE_RESP() : base(SERVICE_ID.OFFICE_ROOM) { }
    public OFFICE_RESP(OPD_ROOM_INFO d) : this()
    {
      this.Rooms.Add(d);
    }
    public OFFICE_RESP(List<OPD_ROOM_INFO> d) : this()
    {
      this.Rooms = d;
    }
  }
}