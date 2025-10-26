using System.Collections.Generic;

namespace ServiceCommon
{
  public class DR_SCH_REQ : ServiceMessage
  {
    public DR_SCH_REQ() : base(SERVICE_ID.DR_SCH) { }
  }

  public class DR_SCH_RESP : ServiceMessage
  {
    public List<DR_SCH_INFO> Schedules { get; set; } = new List<DR_SCH_INFO>();
    public DR_SCH_RESP() : base(SERVICE_ID.DR_SCH) { }
    public DR_SCH_RESP(List<DR_SCH_INFO> d) : this()
    {
      this.Schedules = d;
    }
  }

  public class DR_SCH_INFO
  {
    public string DeptCode { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public int DeptOrder { get; set; } = 0;
    public string DoctorNo { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public int DoctorOrder { get; set; } = 0;
    public string SpecialPart { get; set; } = string.Empty;

    public bool MonAM { get; set; }
    public bool MonPM { get; set; }
    public bool TueAM { get; set; }
    public bool TuePM { get; set; }
    public bool WedAM { get; set; }
    public bool WedPM { get; set; }
    public bool ThuAM { get; set; }
    public bool ThuPM { get; set; }
    public bool FriAM { get; set; }
    public bool FriPM { get; set; }
    public bool SatAM { get; set; }
    public bool SatPM { get; set; }
  }
}