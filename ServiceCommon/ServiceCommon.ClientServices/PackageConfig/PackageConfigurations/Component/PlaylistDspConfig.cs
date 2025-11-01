using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ServiceCommon.ClientServices
{
  public class PlaylistDspConfig
  {
    public List<TimeRange> TimeRanges { get; set; } = new List<TimeRange>();
    public WeekdayRange WeekdayRange { get; set; } = new WeekdayRange();
    public Holidays Holidays { get; set; } = new Holidays();
    NoticeConfig _noticeConfig;
    public int Duration { get; set; }
    public double MediaVolume { get; set; } // 0~1

    public bool IsHoliday(DateTime d)
    {
      return this.Holidays.IsHoliday(d.Date);
    }
    /// <summary>
    /// NoticeConfig는 여러 스케쥴에서 사용하므로 clone 해야 한다
    /// </summary>
    public NoticeConfig GetNoticeConfigClone()
    {
      if (_noticeConfig != null) return _noticeConfig.Clone();
      throw new Exception($"Notice Config is null");
    }

    public NoticeConfig GetNoticeConfigClone(bool use_notice, string message)
    {
      var notice = this.GetNoticeConfigClone();
      notice.UseNotice = use_notice;
      notice.NoticeMessage = message.Trim();
      return notice;
    }

    public void SetNoticeConfig(NoticeConfig noticeConfig)
    {
      _noticeConfig = noticeConfig;
    }
  }

  public class NoticeConfig
  {
    // 기본 설정에 있는 notice 메시지 사용 (순천향)
    public bool UseCommonNotice { get; set; }
    public string CommonNoticeMessage { get; set; } = string.Empty;
    /// <summary>
    /// 한 화면을 순환하는데 걸리는 시간 (초)
    /// </summary>
    public int ScrollSpeed { get; set; }
    public int FontSize { get; set; }
    public Brush Foreground { get; set; } = Brushes.White;
    public Brush Background { get; set; } = Brushes.DarkBlue;
    public bool UseNotice { get; set; } = false;
    public string NoticeMessage { get; set; } = string.Empty;

    public NoticeConfig Clone()
    {
      return (NoticeConfig)this.MemberwiseClone();
    }
  }
}