namespace ServiceCommon
{
  public class ER_STATISTICS_REQ : ServiceMessage
  {
    public bool IsChild { get; set; }
    public ER_STATISTICS_REQ() : base(SERVICE_ID.ER_STATISTICS) { }
    public ER_STATISTICS_REQ(bool child) : this()
    {
      this.IsChild = child;
    }
  }

  public class ER_STATISTICS_RESP : ServiceMessage
  {
    public bool IsChild { get; set; }
    public int PatientCount { get; set; } // 재원 환자수
    public int AverageInTime { get; set; } //  평균재실시간 (분 단위)
    public ER_STATISTICS_RESP() : base(SERVICE_ID.ER_STATISTICS) { }
    public ER_STATISTICS_RESP(ER_STATISTIC_INFO o) : this()
    {
      this.IsChild = o.IsChild;
      this.PatientCount = o.PatientCount;
      this.AverageInTime = o.AverageInTime;
    }
  }

  public class ER_STATISTIC_INFO : IGroupKeyData<bool>
  {
    public bool IsChild { get; set; }
    public int PatientCount { get; set; } // 재원 환자수
    public int AverageInTime { get; set; } //  평균재실시간 (분 단위)

    public bool GroupKey => IsChild;
  }
}