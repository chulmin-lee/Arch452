using System.Collections.Generic;

namespace ServiceCommon
{
  public class ER_AREA_CONGEST_REQ : ServiceMessage
  {
    public bool IsChild { get; set; }
    public ER_AREA_CONGEST_REQ() : base(SERVICE_ID.ER_AREA_CONGEST) { }
    public ER_AREA_CONGEST_REQ(bool child) : this()
    {
      this.IsChild = child;
    }
  }

  public class ER_AREA_CONGEST_RESP : ServiceMessage
  {
    public List<ER_AREA_CONGEST_INFO> AreaCongests { get; set; } = new List<ER_AREA_CONGEST_INFO>();
    public ER_AREA_CONGEST_RESP() : base(SERVICE_ID.ER_AREA_CONGEST) { }
    public ER_AREA_CONGEST_RESP(List<ER_AREA_CONGEST_INFO> d) : this()
    {
      this.AreaCongests = d;
    }
  }

  /// <summary>
  /// 응급 구역별 (성인/소아) 혼잡도
  /// </summary>
  public class ER_AREA_CONGEST_INFO
  {
    public int AreaCode { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public int BedCount { get; set; } // totalBedCnt
    public int InBedCount { get; set; } // inCnt
    public int Percent { get; set; }  // satPer
    public bool IsChild { get; set; }
  }

  public class ER_AREA_CONGEST_GROUP : IGroupKeyData<bool>
  {
    public bool IsChild { get; set; }
    public List<ER_AREA_CONGEST_INFO> AreaCongests { get; set; } = new List<ER_AREA_CONGEST_INFO>();

    public bool GroupKey => this.IsChild;
  }
}