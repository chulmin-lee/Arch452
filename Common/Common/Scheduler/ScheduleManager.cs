using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
  /// <summary>
  /// FluentScheduler를 간단하게 구현한 스케쥴러.
  /// </summary>
  public static class ScheduleManager
  {
    public static event Action<JobStartInfo> JobStart;
    public static event Action<JobExceptionInfo> JobException;
    public static event Action<JobEndInfo> JobEnd;

    static ScheduleCollection _schedules = new ScheduleCollection();
    static bool _useUtc = false;
    const uint _maxTimerInterval = 0xfffffffe;
    internal static DateTime Now => _useUtc ? DateTime.UtcNow : DateTime.Now;
    static readonly ISet<ScheduleTask> _running_tasks = new HashSet<ScheduleTask>();
    static ScheduleManager()
    {
      SystemEvents.TimeChanged += (s, e) => check_schedule();
    }

    public static void Add(Schedule schedule)
    {
      if (schedule == null)
      {
        throw new ArgumentNullException("schedules");
      }
      Add(new List<Schedule> { schedule });
    }
    public static void Add(List<Schedule> schedules)
    {
      if (schedules == null)
      {
        throw new ArgumentNullException("schedules");
      }

      if (schedules.Any())
      {
        Register(schedules);
        check_schedule();
      }
    }

    static void Register(IEnumerable<Schedule> schedules)
    {
      foreach (var s in schedules)
      {
        s.CalcNextRunTime(Now); // 현재 시간으로 다음 실행시간을 계산한다.
        _schedules.Add(s); // 실행 목록에 추가
      }
    }
    /// <summary>
    /// 스케쥴을 검사하고 실행한다.
    /// 재귀호출로 가장 빨리 실행될 스케쥴을 찾아서 실행한다.
    /// 하나의 loop에서 처리하는 경우 List 삭제나 정렬이 어려워서 재귀호출로 처리하는게 낫다
    /// </summary>
    static void check_schedule()
    {
      // 타이머 정지
      stop_schedule_timer();

      // 가장 빨리 실행될것으로 정렬 (NextRun 으로 정렬)
      // 그 다음것은 재귀호출로 처리된다 (만약 실행 가능하다면)
      _schedules.Sort();

      if (!_schedules.Any())
      {
        // 등록된 스케쥴이 없는 경우 타이머를 정지한다.
        // 더 이상 스케쥴링은 하지 않는다
        return;
      }

      // 호출할때마다 가장 빨리 실행되어야할 객체 1개를 가져온다.

      var first = _schedules.First();
      if (first == null) return;

      if (first.NextRunTime <= Now) // 실행해야 한다.
      {
        execute_schedule(first);  // 별도의 Task로 실행한 후 다음 실행 시간을 계산한다.

        first.CalcNextRunTime(Now.AddMilliseconds(1)); // 다음 실행시간을 계산한다.

        // 계산한 다음 실생시간이 현재보다 작거나, 일회성 실행이면 제거
        if (first.NextRunTime <= Now || first.RunOnce)
        {
          _schedules.Remove(first);
        }

        //first.RunOnce = false; //

        // 다음 작업을 검사하기 위해서 재귀호출한다.
        check_schedule();
        return;
      }

      // 등록된 스케쥴중에서 더이상 실행할 작업이 없는 경우
      // 다음번 검사할 시간을 설정한다 (가장 빠른 스케쥴의 실행시간으로 설정한다)
      var interval = first.NextRunTime - Now;

      if (interval <= TimeSpan.Zero)
      {
        // 현재 시간보다 이전에 실행되어야 하는 스케쥴이 있는 경우
        check_schedule();
        return;
      }
      else
      {
        // 최대 타이머 간격을 초과하지 않도록 설정
        if (interval.TotalMilliseconds > _maxTimerInterval)
        {
          interval = TimeSpan.FromMilliseconds(_maxTimerInterval);
        }

        // 다음 스케쥴 타이머 설정
        start_schedule_timer(interval);
      }
    }

    /// <summary>
    /// 1개의 스케쥴을 실행한다.
    /// </summary>
    static void execute_schedule(Schedule schedule)
    {
      if (schedule.Disabled)
      {
        return;
      }

      // 현재 실행중인 schedule 목록에서 찾기
      lock (_running_tasks)
      {
        // 현재 실행중인 작업중에서 같은 Renetrant 객체를 가진것이 있으면 실행안함
        if (schedule.Reentrant != null &&

            _running_tasks.Any(t => ReferenceEquals(t.Schedule.Reentrant, schedule.Reentrant)))
        {
          return;
        }
      }

      var sch_task = new ScheduleTask(schedule);

      var task = new Task(() =>
      {
        var start = Now;

        JobStart?.Invoke(schedule.GetJobStartInfo(start));

        var stopwatch = new Stopwatch();
        try
        {
          stopwatch.Start();

          // 굳이 task로 분리할 필요가 있나?
          // schedule.Action(); // 메인 작업 실행
          Task.Factory.StartNew(schedule.Action).Wait();

          // Jobs 순서를 지키기 위해서 Wait()를 사용한것 같다
          //schedule.Jobs.ForEach(action => Task.Factory.StartNew(action).Wait());
        }
        catch (Exception e)
        {
          JobException?.Invoke(schedule.GetJobExceptionInfo(e));
        }
        finally
        {
          lock (_running_tasks)
          {
            _running_tasks.Remove(sch_task);
          }
          JobEnd?.Invoke(schedule.GetJobEndInfo(start, stopwatch.Elapsed));
        }
      }, TaskCreationOptions.PreferFairness);

      sch_task.Task = task; // Task 설정

      lock (_running_tasks)
      {
        _running_tasks.Add(sch_task);
      }

      task.Start();
    }

    public static Schedule Find(string name)
    {
      return _schedules.Find(name);
    }
    public static IEnumerable<Schedule> AllSchedules
    {
      get
      {
        // returning a shallow copy
        return _schedules.All().ToList();
      }
    }
    public static IEnumerable<Schedule> RunningSchedules
    {
      get
      {
        lock (_running_tasks)
        {
          return _running_tasks.Select(t => t.Schedule).ToList();
        }
      }
    }

    public static void RemoveJob(string name)
    {
      if (!string.IsNullOrEmpty(name))
        _schedules.Remove(name);
    }
    public static void RemoveJob(Schedule s)
    {
      if (s != null)
      {
        _schedules.Remove(s.Name);
      }
    }
    /// <summary>
    /// Removes all schedules.
    /// </summary>
    public static void RemoveAllJobs()
    {
      _schedules.RemoveAll();
    }

    public static void StopAndBlock()
    {
      Stop();

      var tasks = new Task[0];

      // Even though Stop() was just called, a scheduling may be happening right now, that's why the loop.
      // Simply waiting for the tasks inside the lock causes a deadlock (a task may try to remove itself from
      // running, but it can't access the collection, it's blocked by the wait).
      do
      {
        lock (_running_tasks)
        {
          tasks = _running_tasks.Select(t => t.Task).ToArray();
        }

        Task.WaitAll(tasks);
      } while (tasks.Any());
    }
    public static void Start()
    {
      check_schedule();
    }

    public static void Stop()
    {
      stop_schedule_timer();
    }

    #region timer
    static Timer _timer = new Timer(state => check_schedule(), null, Timeout.Infinite, Timeout.Infinite);

    /// <summary>
    /// 지정된 시간이후에 스케쥴을 검사한다
    /// </summary>
    /// <param name="interval"></param>
    static void start_schedule_timer(TimeSpan interval)
    {
      _timer.Change(interval, interval);
    }
    static void stop_schedule_timer()
    {
      _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    #endregion timer
  }
}