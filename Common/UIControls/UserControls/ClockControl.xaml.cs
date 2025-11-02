using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace UIControls
{
  /// <summary>
  /// 바인딩을 쓰면 속도를 따라가지 못한다.
  /// </summary>
  public partial class ClockControl : UserControl
  {
    DispatcherTimer _timer;
    DateTime DateOnly = DateTime.MinValue;
    public ClockControl()
    {
      InitializeComponent();

      this.IsVisibleChanged += (s, e) => this.Refresh();
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);

      var o = this.ClockSetting;

      bool multi = o.MultiLine && (o.ShowDate && o.ShowTime);

      if (!multi)
      {
        if (o.ShowDate && o.ShowTime)
        {
          // 폰트 크기를 같게 한다.
          var size = (this.TimeFontSize + this.DateFontSize) / 2;
          this.DateFontSize = this.TimeFontSize = size;
        }
      }

      this.MultiLine = multi;
      this.StartTimer();
    }
    void StartTimer()
    {
      if (_timer == null)
      {
        _timer = new DispatcherTimer();
        _timer.Interval = this.ClockSetting.ShowTimeSecond ? TimeSpan.FromSeconds(0.5) : TimeSpan.FromSeconds(1);
        _timer.Tick += Timer_Tick;
      }
      if (!_timer.IsEnabled)
        _timer.Start();
    }
    protected void StopTimer()
    {
      _timer?.Stop();
    }
    void Refresh()
    {
      this.StopTimer();
      if (this.IsVisible)
      {
        if (this.ClockSetting.Enabled)
          this.StartTimer();
      }
    }
    private void Timer_Tick(object sender, EventArgs e)
    {
      DateTime today = DateTime.Now;
      if (this.DateOnly != today.Date)
      {
        this.DateOnly = today.Date;
        this.Date.Text = this.ClockSetting.GetDate(today);
      }
      this.Time.Text = this.ClockSetting.GetTime(today);
    }

    #region Font
    public double DateFontSize
    {
      get { return (double)GetValue(DateFontSizeProperty); }
      set { SetValue(DateFontSizeProperty, value); }
    }
    public static readonly DependencyProperty DateFontSizeProperty =
      DependencyProperty.Register("DateFontSize", typeof(double), typeof(ClockControl), new PropertyMetadata(25.0));

    public double TimeFontSize
    {
      get { return (double)GetValue(TimeFontSizeProperty); }
      set { SetValue(TimeFontSizeProperty, value); }
    }
    public static readonly DependencyProperty TimeFontSizeProperty =
      DependencyProperty.Register("TimeFontSize", typeof(double), typeof(ClockControl), new PropertyMetadata(40.0));

    public SolidColorBrush DateFontColor
    {
      get { return (SolidColorBrush)GetValue(DateFontColorProperty); }
      set { SetValue(DateFontColorProperty, value); }
    }
    public static readonly DependencyProperty DateFontColorProperty =
      DependencyProperty.Register("DateFontColor", typeof(SolidColorBrush), typeof(ClockControl), new PropertyMetadata(Brushes.Black));

    public SolidColorBrush TimeFontColor
    {
      get { return (SolidColorBrush)GetValue(TimeFontColorProperty); }
      set { SetValue(TimeFontColorProperty, value); }
    }
    public static readonly DependencyProperty TimeFontColorProperty =
      DependencyProperty.Register("TimeFontColor", typeof(SolidColorBrush), typeof(ClockControl), new PropertyMetadata(Brushes.Black));
    #endregion Font

    #region ClockSetting
    public ClockSetting ClockSetting
    {
      get { return (ClockSetting)GetValue(ClockSettingProperty); }
      set { SetValue(ClockSettingProperty, value); }
    }
    public static readonly DependencyProperty ClockSettingProperty =
      DependencyProperty.Register("ClockSetting", typeof(ClockSetting), typeof(ClockControl), new PropertyMetadata(new ClockSetting()));

    public bool MultiLine
    {
      get { return (bool)GetValue(MultiLineProperty); }
      set { SetValue(MultiLineProperty, value); }
    }
    public static readonly DependencyProperty MultiLineProperty =
      DependencyProperty.Register("MultiLine", typeof(bool), typeof(ClockControl),
        new PropertyMetadata(true));

    #endregion ClockSetting
  }

  /// <summary>
  /// 시계 설정
  /// - 일관성을 위해서 항상 고정인 항목이다.
  /// </summary>
  public class ClockSetting
  {
    public bool UseEnglish { get; set; }
    public bool ShowDate { get; set; } = true;
    public bool ShowTime { get; set; } = true;
    public bool MultiLine { get; set; } = true;
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 년도 자리수. 0이면 표시안함
    /// </summary>
    public int YearLength { get; set; } = 4;
    public bool ShowYear => YearLength > 0;
    public string YearDelimiter { get; set; } = "/";
    public bool ShowWeekOfDay { get; set; } = true;
    public bool Use24Hour { get; set; } = false;
    public bool ShowAmPm { get; set; } = false;
    public bool ShowTimeSecond { get; set; } = false;
    string[] _dayOfWeeks = new string [] { "일", "월", "화", "수", "목", "금", "토"};

    public string GetDate(DateTime d)
    {
      string year = d.Year.ToString();
      if (year.Length >= this.YearLength)
      {
        year = year.Substring(year.Length - this.YearLength);
      }
      var date_str = $"{d.Month.ToString("D2")}{this.YearDelimiter}{d.Day.ToString("D2")}";

      if (this.ShowYear)
      {
        date_str = $"{year}{this.YearDelimiter}{date_str}";
      }

      if (this.ShowWeekOfDay)
      {
        date_str = $"{date_str} ({_dayOfWeeks[(int)d.DayOfWeek]})";
      }
      return date_str;
    }
    public string GetTime(DateTime d)
    {
      string time_str = string.Empty;

      if (this.Use24Hour)
      {
        time_str = $"{d.Hour.ToString("D2")}:{d.Minute.ToString("D2")}";
        if (this.ShowAmPm)
        {
          var s = d.Hour <= 12 ? "오전" : "오후";
          time_str = $"{s} {time_str}";
        }
      }
      else
      {
        if (d.Hour <= 12)
        {
          time_str = $"오전 {d.Hour.ToString("D2")}:{d.Minute.ToString("D2")}";
        }
        else
        {
          var hour = d.Hour > 12 ? d.Hour - 12 : d.Hour;
          time_str = $"오후 {hour.ToString("D2")}:{d.Minute.ToString("D2")}";
        }
      }

      if (this.ShowTimeSecond)
      {
        time_str = $"{time_str}:{d.Second.ToString("D2")}";
      }
      return time_str;
    }
  }
}