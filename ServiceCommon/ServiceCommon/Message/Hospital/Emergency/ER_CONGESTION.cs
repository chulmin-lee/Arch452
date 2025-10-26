namespace ServiceCommon
{
  /// <summary>
  /// 응급실 혼잡도
  /// </summary>
  public class ER_CONGESTION_REQ : ServiceMessage
  {
    public bool IsChild { get; set; }
    public ER_CONGESTION_REQ() : base(SERVICE_ID.ER_CONGESTION) { }
    public ER_CONGESTION_REQ(bool child) : this()
    {
      this.IsChild = child;
    }
  }

  public class ER_CONGESTION_RESP : ServiceMessage
  {
    public bool IsChild { get; set; }
    public int TotalBedCount { get; set; } // totalBedCnt
    public int TotalInBedCount { get; set; } // inCnt
    public int TotalPercent { get; set; }  // satPer

    public ER_CONGESTION_RESP() : base(SERVICE_ID.ER_CONGESTION) { }
    public ER_CONGESTION_RESP(ER_CONGESTION_INFO d) : this()
    {
      this.IsChild = d.IsChild;
      this.TotalBedCount = d.TotalBedCount;
      this.TotalInBedCount = d.TotalInBedCount;
      this.TotalPercent = d.TotalPercent;
    }
  }

  /// <summary>
  /// 응급실 혼잡도
  /// </summary>
  public class ER_CONGESTION_INFO : IGroupKeyData<bool>
  {
    public int TotalBedCount { get; set; } // totalBedCnt
    public int TotalInBedCount { get; set; } // inCnt
    public int TotalPercent { get; set; }  // satPer
    public bool IsChild { get; set; }

    public bool GroupKey => IsChild;
  }
}