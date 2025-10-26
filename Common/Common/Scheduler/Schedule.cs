using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
  public class Schedule
  {
    public string Name { get; internal set; } = string.Empty;
    public DateTime NextRunTime { get; private set; }

    /// <summary>
    /// 다음 실행 시간을 계산하는 함수.
    /// </summary>
    Func<DateTime, DateTime> CalcFunc { get; set; }

    /// <summary>
    /// 일회성 여부
    /// </summary>
    internal bool RunOnce { get; set; }

    /// <summary>
    /// 실행이 비활성화 되었는지 여부
    /// </summary>
    public bool Disabled { get; private set; }

    /// <summary>
    /// 재진입 방지 여부. 설정된 경우 재진입 금지
    /// </summary>
    internal object Reentrant { get; set; }

    internal Action Action { get; set; }

    public Schedule(Action action)
    {
      this.Action = action;
    }
    public void CalcNextRunTime(DateTime now)
    {
      if (this.CalcFunc != null)
      {
        this.NextRunTime = this.CalcFunc(now);
      }
    }

    //================================
    // 지정된 간격으로 실행
    //================================
    public Schedule RunEveryInterval(TimeSpan ts)
    {
      this.CalcFunc = (now) => now.Add(ts);
      return this;
    }
    public Schedule ToRunEverySecond(int interval)
    {
      var ts = new TimeSpan(0, 0, interval);
      return this.RunEveryInterval(ts);
    }
    public Schedule ToRunEveryMinute(int interval)
    {
      var ts = new TimeSpan(0, interval, 0);
      return this.RunEveryInterval(ts);
    }
    public Schedule ToRunEveryHour(int interval)
    {
      var ts = new TimeSpan(interval, 0, 0);
      return this.RunEveryInterval(ts);
    }

    //=============================
    // 지정된 시간에 실행
    //=============================
    /// <summary>
    /// 지정된 시간에 매일 실행한다.
    /// </summary>
    public Schedule ToRunAt(int hour, int minute) => this.RunAtTime(new TimeSpan(hour, minute, 0));
    public Schedule RunAtTime(TimeSpan ts, bool once = false)
    {
      this.RunOnce = once;
      this.CalcFunc = (now) =>
      {
        var nextRun = now.Date.Add(ts);
        if (nextRun <= now)
        {
          nextRun = nextRun.AddDays(1);
        }
        return nextRun;
      };
      return this;
    }
    public Schedule RunAtTimes(IEnumerable<TimeSpan> times, bool once = false)
    {
      // 지정된 시간 중 가장 빠른 시간을 계산한다.

      this.RunOnce = once;
      this.CalcFunc = (now) =>
      {
        var nextRun = DateTime.MaxValue;
        foreach (var ts in times)
        {
          var runTime = now.Date.Add(ts);
          if (runTime <= now)
          {
            runTime = runTime.AddDays(1);
          }
          if (runTime < nextRun)
          {
            nextRun = runTime;
          }
        }
        return nextRun;
      };
      return this;
    }

    /// <summary>
    /// 지정된 시간에 한번 실행한다.
    /// 지정된 시간이 경과했다면, 1일 후에 실행된다.
    /// </summary>
    public Schedule ToRunOnceAt(int hour, int minute) => this.ToRunOnceAt(new TimeSpan(hour, minute, 0));
    public Schedule ToRunOnceAt(TimeSpan ts) => this.RunAtTime(ts, true);

    //================================
    // 속성
    //================================
    public Schedule WithName(string name)
    {
      Name = name;
      return this;
    }
    public Schedule NonReentrant()
    {
      Reentrant = Reentrant ?? new object();
      return this;
    }

    public void Disable() => Disabled = true;
    public void Enable() => Disabled = false;

    public JobStartInfo GetJobStartInfo(DateTime d)
    {
      return new JobStartInfo
      {
        Name = this.Name,
        StartTime = d
      };
    }
    public JobEndInfo GetJobEndInfo(DateTime d, TimeSpan elapsed)
    {
      return new JobEndInfo
      {
        Name = this.Name,
        StartTime = d,
        Elapsed = elapsed,
        NextRun = this.NextRunTime
      };
    }
    public JobExceptionInfo GetJobExceptionInfo(Exception e)
    {
      var aggregate = e as AggregateException;
      if (aggregate != null && aggregate.InnerExceptions.Count == 1)
      {
        e = aggregate.InnerExceptions.Single();
      }

      return new JobExceptionInfo
      {
        Name = this.Name,
        Exception = e
      };
    }
  }
}