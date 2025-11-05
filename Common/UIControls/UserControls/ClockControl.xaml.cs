using System;
using System.Globalization;
using System.Text;
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
    DispatcherTimer _timer = new DispatcherTimer();
    DateTime CurrentDate = DateTime.MinValue;

    public ClockControl()
    {
      InitializeComponent();
      this.Loaded += ClockControl_Loaded;
      this.Unloaded += (s, e) => _timer?.Stop();
    }
    void ClockControl_Loaded(object sender, RoutedEventArgs e)
    {
      var o = this.ClockSetting;
      if (o.ShowClock)
      {
        this.MultiLine = o.MultiLine && o.ShowDate && o.ShowTime;
        if (this.MultiLine == false)
        {
          if (o.ShowDate && o.ShowTime)
          {
            // 폰트 크기를 같게 한다.
            var size = (this.TimeFontSize + this.DateFontSize) / 2;
            this.DateFontSize = this.TimeFontSize = size;
          }
        }
        // timer
        _timer.Interval = this.ClockSetting.ShowSecond ? TimeSpan.FromSeconds(0.5) : TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e1) =>
        {
          DateTime today = DateTime.Now;
          if (this.CurrentDate != today.Date)
          {
            // 날짜가 바뀔때만 그린다
            this.CurrentDate = today.Date;
            this.Date.Text = this.ClockSetting.GetDate(today);
          }
          this.Time.Text = this.ClockSetting.GetTime(today);
        };
        _timer.Start();
      }
    }

    #region Font
    public double DateFontSize
    {
      get { return (double)GetValue(DateFontSizeProperty); }
      set { SetValue(DateFontSizeProperty, value); }
    }
    public static readonly DependencyProperty DateFontSizeProperty =
      DependencyProperty.Register(nameof(DateFontSize), typeof(double), typeof(ClockControl), new PropertyMetadata(25.0));

    public double TimeFontSize
    {
      get { return (double)GetValue(TimeFontSizeProperty); }
      set { SetValue(TimeFontSizeProperty, value); }
    }
    public static readonly DependencyProperty TimeFontSizeProperty =
      DependencyProperty.Register(nameof(TimeFontSize), typeof(double), typeof(ClockControl), new PropertyMetadata(40.0));

    public SolidColorBrush DateFontColor
    {
      get { return (SolidColorBrush)GetValue(DateFontColorProperty); }
      set { SetValue(DateFontColorProperty, value); }
    }
    public static readonly DependencyProperty DateFontColorProperty =
      DependencyProperty.Register(nameof(DateFontColor), typeof(SolidColorBrush), typeof(ClockControl), new PropertyMetadata(Brushes.Black));

    public SolidColorBrush TimeFontColor
    {
      get { return (SolidColorBrush)GetValue(TimeFontColorProperty); }
      set { SetValue(TimeFontColorProperty, value); }
    }
    public static readonly DependencyProperty TimeFontColorProperty =
      DependencyProperty.Register(nameof(TimeFontColor), typeof(SolidColorBrush), typeof(ClockControl), new PropertyMetadata(Brushes.Black));
    #endregion Font

    #region ClockSetting
    public ClockSetting ClockSetting
    {
      get { return (ClockSetting)GetValue(ClockSettingProperty); }
      set { SetValue(ClockSettingProperty, value); }
    }
    public static readonly DependencyProperty ClockSettingProperty =
      DependencyProperty.Register(nameof(ClockSetting), typeof(ClockSetting), typeof(ClockControl), new PropertyMetadata(new ClockSetting()));

    public bool MultiLine
    {
      get { return (bool)GetValue(MultiLineProperty); }
      set { SetValue(MultiLineProperty, value); }
    }
    public static readonly DependencyProperty MultiLineProperty =
      DependencyProperty.Register(nameof(MultiLine), typeof(bool), typeof(ClockControl),
        new PropertyMetadata(true));

    #endregion ClockSetting
  }

  public class ClockSetting
  {
    public string GetDate(DateTime d)
    {
      if (string.IsNullOrEmpty(_date_format))
      {
        var sep = this.get_date_seperator();
        var sb = new StringBuilder();
        if (this.YearLength > 0)
        {
          if (this.YearLength > 4) this.YearLength = 4;
          sb.Append(new string('y', this.YearLength));
          sb.Append(sep);
        }
        sb.Append("MM");
        sb.Append(sep);
        sb.Append("dd");
        if (this.ShowWeekOfDay)
        {
          sb.Append(" '('ddd')'"); // 요일
        }
        _date_culture = CultureInfo.CreateSpecificCulture("ko-Kr");
        _date_format = sb.ToString();
      }
      return d.ToString(_date_format, _date_culture);
    }
    public string GetTime(DateTime d)
    {
      if (string.IsNullOrEmpty(_time_format))
      {
        var sep = this.get_time_seperator();
        var sb = new StringBuilder();
        if (this.AmPmShow)
        {
          sb.Append("tt ");  // AM/PM, 오전/오후
        }
        sb.Append(this.Use24Hour ? "HH" : "hh");
        sb.Append(sep);
        sb.Append("mm");
        if (this.ShowSecond)
        {
          sb.Append(sep);
          sb.Append("ss");
        }
        _time_format = sb.ToString();
        _time_culture = this.AmPmEnglish ? CultureInfo.CreateSpecificCulture("en-US") :
                                           CultureInfo.CreateSpecificCulture("ko-Kr");
      }
      return d.ToString(_time_format, _time_culture);
    }
    string get_date_seperator() => $"'{this.DateSeperator}'";
    string get_time_seperator() => $"'{this.TimeSeperator}'";

    public bool ShowClock => this.Enabled && (this.ShowDate || this.ShowTime);
    public bool MultiLine { get; set; } = true;
    public bool Enabled { get; set; } = true;
    //==========================================
    // date
    //==========================================
    public bool ShowDate { get; set; } = true;
    public int YearLength { get; set; } = 4;
    public string DateSeperator { get; set; } = "/";
    public bool ShowWeekOfDay { get; set; } = true;
    //=============================================
    // Time
    //=============================================
    public bool ShowTime { get; set; } = true;
    public string TimeSeperator { get; set; } = ":";
    public bool Use24Hour { get; set; } = false;
    public bool AmPmShow { get; set; } = true;
    public bool AmPmEnglish { get; set; }
    public bool ShowSecond { get; set; } = false;

    string _date_format = string.Empty;
    string _time_format = string.Empty;
    CultureInfo _date_culture = CultureInfo.CreateSpecificCulture("ko-Kr");
    CultureInfo _time_culture = CultureInfo.CreateSpecificCulture("ko-Kr");
  }
}