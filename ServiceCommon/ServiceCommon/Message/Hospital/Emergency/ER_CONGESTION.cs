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
    public ER_CONGESTION_INFO Congestion { get; set; } = new ER_CONGESTION_INFO();
    public ER_CONGESTION_RESP() : base(SERVICE_ID.ER_CONGESTION) { }
    public ER_CONGESTION_RESP(ER_CONGESTION_INFO d) : this()
    {
      this.Congestion = d;
    }
  }

  /// <summary>
  /// 응급실 혼잡도
  /// </summary>
  public class ER_CONGESTION_INFO : IGroupKeyData<bool>
  {
    public bool IsChild { get; set; } // 성인/소아
    public int CongestPercent { get; set; }  // CALC_VALUE,  혼잡도
    public int PatientWaitTime { get; set; } // ER_STAY_TM  응급환자 평균대기시간
    public int TreatmentWaitTime { get; set; } // ER_MED_TM  응급 진료
    public int CTWaitTime { get; set; }  // CT_EXAM_TM  CT검사
    public int XrayWaitTime { get; set; } //XRAY_EXAM_TM  X=ray 검사
    public bool GroupKey => IsChild;
  }
}