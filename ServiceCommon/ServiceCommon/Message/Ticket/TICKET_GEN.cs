namespace ServiceCommon
{
  public class TICKET_GEN_REQ : ServiceMessage
  {
    public int DivId { get; set; }
    public TICKET_GEN_REQ() : base(SERVICE_ID.TICKET_GEN) { }
    public TICKET_GEN_REQ(int div, int id) : this()
    {
      this.DivId = div;
    }
  }

  public class TICKET_GEN_RESP : ServiceMessage
  {
    public int DivId { get; set; }
    public int WaitNo { get; set; }
    public TICKET_GEN_RESP() : base(SERVICE_ID.TICKET_GEN) { }
    public TICKET_GEN_RESP(int div, int no) : this()
    {
      this.DivId = div;
      this.WaitNo = no;
    }
  }

  public class TICKET_GEN_PT_REQ : ServiceMessage
  {
    public int DivId { get; set; }
    public string SSN { get; set; } = string.Empty;
    public string PatientNo { get; set; } = string.Empty;

    public TICKET_GEN_PT_REQ() : base(SERVICE_ID.TICKET_GEN_PT) { }
    public TICKET_GEN_PT_REQ(int div, string ssn, string pt_no) : this()
    {
      this.DivId = div;
      this.SSN = ssn;
      this.PatientNo = pt_no;
    }
  }

  public class TICKET_GEN_PT_RESP : ServiceMessage
  {
    public int DivId { get; set; }
    public int WaitNo { get; set; }
    public string SSN { get; set; } = string.Empty;
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;

    public TICKET_GEN_PT_RESP() : base(SERVICE_ID.TICKET_GEN_PT) { }
  }
}