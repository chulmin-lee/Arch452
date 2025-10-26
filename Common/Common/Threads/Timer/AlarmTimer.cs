using Microsoft.Win32;
using System;
using System.Threading;

namespace Common
{
  /// <summary>
  /// 매일 지정한 시간에 이벤트를 발생시키는 타이머
  /// </summary>
  public class AlarmTimer
  {
    public event EventHandler AlarmNotified;
    // 알람 시간
    TimeSpan AlarmTimeSpan = TimeSpan.Zero;
    // 알람 발생 날짜와 시간
    DateTime AlarmDateTime = DateTime.MinValue;
    Timer _timer;

    /// <summary>
    /// 매일 지정된 시간에 타이머 동작
    /// </summary>
    public AlarmTimer()
    {
      SystemEvents.TimeChanged += (s, e) => this.TimerExpired();
    }
    public void SetAlarm(int hour, int min, int sec) => this.SetAlarm(new TimeSpan(hour, min, sec));
    public void SetAlarm(TimeSpan ts)
    {
      AlarmTimeSpan = ts;
      this.start_alarm();
    }
    public void StopAlarm()
    {
      this.stop_timer();
      AlarmTimeSpan = TimeSpan.Zero;
      AlarmDateTime = DateTime.MinValue;
    }
    void start_alarm()
    {
      this.stop_timer();
      var now = DateTime.Now;

      var ts = now.TimeOfDay;
      var alarm_ts = AlarmTimeSpan;

      if (ts >= alarm_ts)
      {
        // 지정된 시간이 지났다면 다음날로 설정
        alarm_ts = alarm_ts.Add(new TimeSpan(1, 0, 0, 0));
      }

      AlarmDateTime = now.Date.Add(alarm_ts);
      this.start_timer(alarm_ts - ts);
    }
    void start_timer(TimeSpan after)
    {
      if (_timer == null)
      {
        _timer = new Timer(s => TimerExpired(), null, Timeout.Infinite, Timeout.Infinite);
      }
      _timer?.Change(after, TimeSpan.Zero);
    }
    void stop_timer() => _timer?.Change(-1, -1);

    /// <summary>
    /// 현재 시간이 지정된 시간 이후인지 확인하고, 알람 이벤트 발생
    /// </summary>
    protected void TimerExpired()
    {
      if (this.AlarmTimeSpan == TimeSpan.Zero)
      {
        // 알람 시간이 설정되지 않은 경우
        return;
      }

      if (DateTime.Now >= this.AlarmDateTime)
      {
        this.AlarmNotified?.Invoke(this, EventArgs.Empty);
      }
      // 다음 알람 시간 설정
      this.start_alarm();
    }
  }
}