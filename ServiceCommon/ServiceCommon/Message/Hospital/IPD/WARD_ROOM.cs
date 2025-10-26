using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon
{
  public class WARD_ROOM_REQ : ServiceMessage
  {
    public int Floor { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string GroupKey => $"{this.Floor}:{this.AreaCode}";
    public WARD_ROOM_REQ() : base(SERVICE_ID.WARD_ROOMS) { }
    public WARD_ROOM_REQ(int floor, string code) : this()
    {
      this.Floor = floor;
      this.AreaCode = code;
    }
  }

  public class WARD_ROOM_RESP : ServiceMessage
  {
    public AREA_WARD_INFO Ward { get; set; } = new AREA_WARD_INFO();
    public WARD_ROOM_RESP() : base(SERVICE_ID.WARD_ROOMS) { }
    public WARD_ROOM_RESP(AREA_WARD_INFO d) : this()
    {
      this.Ward = d;
    }
  }

  public class AREA_WARD_INFO : IGroupKeyData<string>
  {
    // 병실 정보
    public int Floor { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public List<AREA_WARD> AreaWards { get; set; } = new List<AREA_WARD>();

    public string GroupKey => $"{this.Floor}:{this.AreaCode}";

    public int TotalCapacity => this.AreaWards.Select(w => w.Capacity).Sum();

    public class AREA_WARD : IGroupKeyData<string>
    {
      public int Floor { get; set; }
      public string AreaCode { get; set; } = string.Empty;
      public string RoomCode { get; set; } = string.Empty;
      public string RoomName { get; set; } = string.Empty;
      public int Capacity { get; set; } // 수용인원. 1 = 1인실
      public string Assistant { get; set; } = string.Empty; // 담당 간호사
      public List<WARD_PATIENT> Patients { get; set; } = new List<WARD_PATIENT>();

      public string GroupKey => $"{Floor}:{AreaCode}";
      public override string ToString()
      {
        return $"{this.GroupKey} : {RoomCode}";
      }
    }

    public class WARD_PATIENT
    {
      public string PatientNo { get; set; } = string.Empty;
      public string PatientName { get; set; } = string.Empty;
      public string PatientMaskedName { get; set; } = string.Empty;
      public bool IsMale { get; set; }  // true: 남, false: 여
      public int Age { get; set; }

      // 병실 환자 구분
      public bool Fall { get; set; }  // 낙상
      public bool Fire { get; set; } // 화재
      public bool Surgery { get; set; }  // 수술
    }
  }
}