using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon
{
  public class FUNERAL_REQ : ServiceMessage
  {
    public List<string> RoomCodes { get; set; } = new List<string>();
    public FUNERAL_REQ() : base(SERVICE_ID.FUNERAL_ROOM) { }
    public FUNERAL_REQ(string code) : this()
    {
      this.RoomCodes.Add(code);
    }
    public FUNERAL_REQ(List<string> codes) : this()
    {
      this.RoomCodes.AddRange(codes);
    }
  }
  public class FUNERAL_RESP : ServiceMessage
  {
    public List<FUNERAL_ALTAR_INFO> Altars { get; set; } = new List<FUNERAL_ALTAR_INFO>();

    public FUNERAL_RESP() : base(SERVICE_ID.FUNERAL_ROOM) { }
    public FUNERAL_RESP(FUNERAL_ALTAR_INFO d) : this()
    {
      this.Altars.Add(d);
    }
    public FUNERAL_RESP(List<FUNERAL_ALTAR_INFO> d) : this()
    {
      this.Altars = d;
    }
  }

  public class FUNERAL_ALTAR_INFO : IGroupKeyData<string>
  {
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string PatientNo { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public string BurialDate { get; set; } = string.Empty; // 입관
    public string ProcessionDate { get; set; } = string.Empty; // 발인
    public string BurialPlace { get; set; } = string.Empty; // 장지

    public string Spouse { get; set; } = string.Empty;
    public List<string> Son { get; set; } = new List<string>();  // 아들
    public List<string> Daughter { get; set; } = new List<string>();
    public List<string> SonInLaw { get; set; } = new List<string>();
    public List<string> DaughterInLaw { get; set; } = new List<string>();
    public List<string> GrandChildren { get; set; } = new List<string>();

    public string GroupKey => this.RoomCode;

    public int MaxMemberCount()
    {
      var counts = new List<int>();
      counts.Add(Son.Count);
      counts.Add(Daughter.Count);
      counts.Add(SonInLaw.Count);
      counts.Add(DaughterInLaw.Count);
      counts.Add(GrandChildren.Count);
      return counts.Max();
    }
  }
}