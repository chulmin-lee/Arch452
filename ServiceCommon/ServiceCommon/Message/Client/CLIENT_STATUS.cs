namespace ServiceCommon
{
  public class CLIENT_STATUS_REQ : ServiceMessage
  {
    public int ClientID { get; set; }
    public long FreeHDDSpace { get; set; }  // g 단위
    public long MemoryUsage { get; set; }  // m 단위
    public int CpuUsagePercent { get; set; }
    public bool IsScreenOn { get; set; } = true;
    public ERROR ErrorCode { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    // 나중에
    public int CpuTemperature { get; set; }

    public CLIENT_STATUS_REQ() : base(SERVICE_ID.CLIENT_STATUS) { }
    public enum ERROR
    {
      None = 0,
      PlaylistError,
      HddFull,
      CpuUsage,
      MemoryUsage,
    }
  }

  public class CLIENT_STATUS_RESP : ServiceMessage
  {
    public bool Status { get; set; }
    public CLIENT_STATUS_RESP() : base(SERVICE_ID.CLIENT_STATUS) { }
    public CLIENT_STATUS_RESP(bool s) : this()
    { this.Status = s; }
  }
}