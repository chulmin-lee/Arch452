using System.Collections.Generic;

namespace ServiceCommon
{
  public class DELIVERY_ROOM_REQ : ServiceMessage
  {
    public DELIVERY_ROOM_REQ() : base(SERVICE_ID.DELIVERY_ROOM) { }
  }

  public class DELIVERY_ROOM_RESP : ServiceMessage
  {
    public List<DELIVERY_INFO> Patients { get; set; } = new List<DELIVERY_INFO>();
    public DELIVERY_ROOM_RESP() : base(SERVICE_ID.DELIVERY_ROOM) { }
    public DELIVERY_ROOM_RESP(List<DELIVERY_INFO> d) : this()
    {
      Patients = d;
    }
  }

  public class DELIVERY_INFO
  {
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string PatientMaskedName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsChildBirth { get; set; }
    public bool IsMale { get; set; }  // true: 남아, false: 여아
  }
}