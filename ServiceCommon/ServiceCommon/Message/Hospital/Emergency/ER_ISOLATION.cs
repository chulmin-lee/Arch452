using System.Collections.Generic;

namespace ServiceCommon
{
  public class ER_ISOLATION_REQ : ServiceMessage
  {
    public List<string> BedCodes { get; set; } = new List<string>();
    public ER_ISOLATION_REQ() : base(SERVICE_ID.ER_ISOLATION) { }
    public ER_ISOLATION_REQ(List<string> bedCodes) : this()
    {
      this.BedCodes = bedCodes;
    }
    public ER_ISOLATION_REQ(string code) : this()
    {
      this.BedCodes.Add(code);
    }
  }
  public class ER_ISOLATION_RESP : ServiceMessage
  {
    public List<ER_ISOLATION_INFO> Isolations { get; set; } = new List<ER_ISOLATION_INFO>();
    public ER_ISOLATION_RESP() : base(SERVICE_ID.ER_ISOLATION) { }
    public ER_ISOLATION_RESP(ER_ISOLATION_INFO d) : this()
    {
      this.Isolations.Add(d);
    }
    public ER_ISOLATION_RESP(List<ER_ISOLATION_INFO> d) : this()
    {
      this.Isolations = d;
    }
  }

  public class ER_ISOLATION_INFO : IGroupKeyData<string>
  {
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;

    public EMERGENCY_INFO Patient { get; set; } = new EMERGENCY_INFO();

    public string GroupKey => this.RoomCode;
  }
}