namespace ServiceCommon
{
  public class TICKET_CALL_REQ : ServiceMessage
  {
    public int DivId { get; set; }
    public int WndNo { get; set; }
    public TICKET_CALL_REQ() : base(SERVICE_ID.TICKET_CALL) { }
    public TICKET_CALL_REQ(int div, int wnd) : this()
    {
      this.DivId = div;
      this.WndNo = wnd;
    }
  }

  public class TICKET_CALL_RESP : ServiceMessage
  {
    public int DivId { get; set; }
    public int WndNo { get; set; }
    public int WaitNo { get; set; }
    public TICKET_CALL_RESP() : base(SERVICE_ID.TICKET_CALL) { }
    public TICKET_CALL_RESP(int div, int wndno, int waitno) : this()
    {
      this.DivId = div;
      this.WaitNo = waitno;
      this.WndNo = wndno;
    }
  }

  /// <summary>
  /// 메시징 서비스로 대체?
  /// 이러면 Ticket과 messaging은 동시 운영한다는 가정이 필요함
  /// 그러므로 메시징으로 처리하면 안됨
  /// </summary>
  public class TICKET_RECALL_REQ : ServiceMessage
  {
    public int DivId { get; set; }
    public int WndNo { get; set; }
    public int WaitNo { get; set; }

    public TICKET_RECALL_REQ() : base(SERVICE_ID.TICKET_RECALL) { }
    public TICKET_RECALL_REQ(int div, int wnd, int waitno, int client) : this()
    {
      this.DivId = div;
      this.WndNo = wnd;
      this.WaitNo = waitno;
    }
  }
  public class TICKET_RECALL_RESP : ServiceMessage
  {
    public int DivId { get; set; }
    public int WndNo { get; set; }
    public int WaitNo { get; set; }

    public TICKET_RECALL_RESP() : base(SERVICE_ID.TICKET_RECALL) { }
    public TICKET_RECALL_RESP(int div, int wnd, int waitno, int client) : this()
    {
      this.DivId = div;
      this.WndNo = wnd;
      this.WaitNo = waitno;
    }
  }
}