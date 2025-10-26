using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.ClientServices
{
  public class PackageScheduler
  {
    string EmptyPackage = "no";
    DateTime _scheduleDate = DateTime.MinValue;
    Holidays Holidays = new Holidays();
    List<PlaylistSchedule> _schedule_master = new List<PlaylistSchedule>();
    List<ScheduleUnit> _today_schedules = new List<ScheduleUnit>();
    public PackageScheduler() { }
    public PackageScheduler(Holidays holiday, List<PlaylistSchedule> schedules)
    {
      this.Holidays = holiday;
      _schedule_master = schedules;
    }

    public PlaylistSchedule GetCurrentSchedule(DateTime date)
    {
      if (date.Date != _scheduleDate)
      {
        // 날짜가 바뀌었으면 오늘 스케쥴을 갱신한다.
        _today_schedules = this.CreateTodaySchedules(DateTime.Now.Date);
      }
      var unit = _today_schedules.Where(x => x.IsInRange(date.TimeOfDay)).FirstOrDefault();
      if (unit == null)
      {
        throw new Exception("No schedule found for the current time range.");
      }
      return unit.Schedule;
    }

    List<ScheduleUnit> CreateTodaySchedules(DateTime date)
    {
      _scheduleDate = date.Date;
      var schedules = new List<PlaylistSchedule>();

      if (!this.Holidays.IsHoliday(date.Date))
      {
        foreach (var sch in _schedule_master)
        {
          if (sch.CanTodayRunning(DateTime.Now))
          {
            schedules.Add(sch);
          }
        }

        // 기간 설정이 우선 (C > B > A)
        var date_sch = schedules.Where(x => x.Mode == "C").FirstOrDefault();
        if (date_sch != null)
        {
          schedules.Clear();
          schedules.Add(date_sch);
        }
      }

      //================================================
      // 우선순위로 스케쥴 정리
      //================================================
      var orderd_sch = new List<ScheduleUnit>()
    {
      new ScheduleUnit(this.EmptyPackage)
    };

      var list = schedules.Select(x => new ScheduleUnit(x)).ToList();
      orderd_sch.AddRange(list.Where(x => x.IsDefault).ToList()); // 기본 스케쥴
      orderd_sch.AddRange(list.Where(x => !x.IsDefault).OrderBy(x => x.StartTime).ToList()); // 요일 스케쥴

      //================================================
      // merge schedule
      //================================================
      var today = new List<ScheduleUnit>();
      foreach (var p in orderd_sch)
      {
        // 완전 덮어쓰는것 제거
        today.Where(x => p.StartTime <= x.StartTime && x.EndTime <= p.EndTime).ToList()
             .ForEach(x => today.Remove(x));

        // 겹치는것
        var intersect = today.Where(x => x.StartTime < p.EndTime && x.EndTime > p.StartTime).ToList();

        today.Add(p);

        if (intersect.Any())
        {
          foreach (var ov in intersect)
          {
            if (p.StartTime <= ov.StartTime)
            {
              ov.StartTime = p.EndTime;
            }
            else if (ov.StartTime < p.StartTime)
            {
              var end = ov.EndTime;
              ov.EndTime = p.StartTime;

              if (p.EndTime < end)
              {
                var range = new TimeRange(p.EndTime, end);
                today.Add(ov.CreateClone(range));
              }
            }
          }
        }
      }

      today = today.OrderBy(x => x.StartTime).ToList();

      //================================================
      // check
      //================================================
      #region check
      {
        var first_sch = today.First();
        if (first_sch.StartTime != TimeSpan.Zero)
        {
          throw new Exception($"first schedule start time is not zero: {first_sch.StartTime}");
        }

        var last_sch = today.Last();
        if (last_sch.EndTime != new TimeSpan(24, 0, 0))
        {
          throw new Exception($"last schedule end time is not 24:00:00");
        }

        foreach (var sch in today.Skip(1))
        {
          if (sch.StartTime != first_sch.EndTime)
          {
            throw new Exception($"schedule start time is not equal to previous end time: {sch.StartTime} != {first_sch.EndTime}");
          }
          first_sch = sch; // 다음 스케줄로 이동
        }
      }
      #endregion check

      // list up
      foreach (var p in today)
      {
        LOG.dc(p.ToString());
      }

      return today;
    }

    public List<REMOTE_FILE> GetDownloadContents()
    {
      Dictionary<string, REMOTE_FILE> contents = new Dictionary<string, REMOTE_FILE>();

      foreach (var s in _schedule_master)
      {
        foreach (var c in s.RemoteContents)
        {
          if (!contents.ContainsKey(c.FileName))
          {
            contents.Add(c.FileName, c);
          }
        }
      }
      return contents.Values.ToList();

      //return _schedule_master.SelectMany(x => x.RemoteContents).Select(x => x.Name).Distinct().ToList();
    }
    public void ConetentUpdated(string content_dir)
    {
      foreach (var p in _schedule_master)
      {
        p.UpdateContentFile(content_dir);
      }
    }
  }
}