using System;
using System.Collections.Generic;

namespace Common
{
  public class ScheduleBuiler<T> where T : Enum
  {
    List<Schedule> _schedules = new List<Schedule>();

    public ScheduleBuiler<T> Initialize()
    {
      _schedules.ForEach(x => x.NonReentrant());
      ScheduleManager.Add(_schedules);
      _schedules.Clear();
      return this;
    }
    public ScheduleBuiler<T> RunEverySecond(Action action, T name, int sec) => RunEveryInterval(action, name, new TimeSpan(0, 0, sec));
    public ScheduleBuiler<T> RunEveryMinute(Action action, T name, int minute) => RunEveryInterval(action, name, new TimeSpan(0, minute, 0));
    public ScheduleBuiler<T> RunEveryHour(Action action, T name, int hour) => RunEveryInterval(action, name, new TimeSpan(hour, 0, 0));
    /// <summary>
    /// 지정된 시간 간격마다 실행한다
    /// </summary>
    public ScheduleBuiler<T> RunEveryInterval(Action action, T name, TimeSpan ts)
    {
      var s = new Schedule(action).WithName(name.ToString()).RunEveryInterval(ts);
      _schedules.Add(s);
      return this;
    }

    /// <summary>
    /// 매일 지정된 시간에 실행한다.
    /// 주의: 지정된 ts가 아직 지나지 않은 경우 1일후가 아닌 ts가 지난 후 실행된다.
    /// </summary>
    public ScheduleBuiler<T> RunAtTime(Action action, T name, TimeSpan ts) => RunAtTime(action, name, ts, false);
    public ScheduleBuiler<T> RunOnceAt(Action action, T name, TimeSpan ts) => RunAtTime(action, name, ts, true);
    ScheduleBuiler<T> RunAtTime(Action action, T name, TimeSpan ts, bool once)
    {
      var s = new Schedule(() => action()).WithName(name.ToString()).RunAtTime(ts);
      _schedules.Add(s);
      return this;
    }

    public ScheduleBuiler<T> RunAtTimes(Action action, T name, List<TimeSpan> times)
    {
      var s = new Schedule(() => action()).WithName(name.ToString()).RunAtTimes(times);
      _schedules.Add(s);
      return this;
    }

    public ScheduleBuiler<T> Add(Schedule s)
    {
      if (s != null)
      {
        _schedules.Add(s);
      }
      return this;
    }

    public void Remove(T name)
    {
      ScheduleManager.RemoveJob(name.ToString());
    }
    public void RemoveAll()
    {
      ScheduleManager.RemoveAllJobs();
    }
    public void Close()
    {
      ScheduleManager.RemoveAllJobs();
      ScheduleManager.Stop();
    }
  }
}