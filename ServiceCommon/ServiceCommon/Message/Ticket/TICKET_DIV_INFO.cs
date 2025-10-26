using System.Collections.Generic;

namespace ServiceCommon
{
  public class TICKET_DIV_INFO_REQ : ServiceMessage
  {
    public List<int> DivIds { get; set; } = new List<int>();
    public TICKET_DIV_INFO_REQ() : base(SERVICE_ID.TICKET_DIV_INFO) { }
    public TICKET_DIV_INFO_REQ(List<int> ids) : this()
    {
      this.DivIds.AddRange(ids);
    }
  }
  /// <summary>
  /// Division info를 요청하거나, 티겟 생성/호출로 division 변경이 발생했을때 공유 목적으로 전송
  /// </summary>
  public class TICKET_DIV_INFO_RESP : ServiceMessage
  {
    public List<TICKET_DIV_INFO> Divisions { get; set; } = new List<TICKET_DIV_INFO>();
    public TICKET_DIV_INFO_RESP() : base(SERVICE_ID.TICKET_DIV_INFO) { }
    public TICKET_DIV_INFO_RESP(TICKET_DIV_INFO list) : this()
    {
      this.Divisions.Add(list);
    }
    public TICKET_DIV_INFO_RESP(List<TICKET_DIV_INFO> list) : this()
    {
      this.Divisions.AddRange(list);
    }
  }

  public class TICKET_DIV_INFO
  {
    public int DivId { get; set; }
    public int WaitCount => this.WaitNos.Count;
    public List<int> WaitNos { get; set; } = new List<int>();
    public TICKET_DIV_INFO() { }
    public TICKET_DIV_INFO(int div, List<int> nos)
    {
      this.DivId = div;
      this.WaitNos.AddRange(nos);
    }
    public override string ToString()
    {
      return $"TICKET_DIVISION: {DivId}, Count: {WaitCount}";
    }
  }
}