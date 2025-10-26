using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
  internal class ScheduleCollection
  {
    private List<Schedule> _schedules = new List<Schedule>();

    private object _lock = new object();

    internal bool Any()
    {
      lock (_lock)
      {
        return _schedules.Any();
      }
    }

    internal void Sort()
    {
      lock (_lock)
      {
        _schedules.Sort((x, y) => DateTime.Compare(x.NextRunTime, y.NextRunTime));
      }
    }

    internal IEnumerable<Schedule> All()
    {
      lock (_lock)
      {
        return _schedules;
      }
    }

    internal void Add(Schedule schedule)
    {
      lock (_lock)
      {
        // 같은 이름으로 등록된 스케쥴이 있다면 삭제 후 추가
        this.Remove(schedule.Name);
        _schedules.Add(schedule);
      }
    }

    internal bool Remove(string name)
    {
      lock (_lock)
      {
        var schedule = Find(name);
        if (schedule != null)
        {
          _schedules.Remove(schedule);
          return true;
        }
        return false;
      }
    }

    internal bool Remove(Schedule schedule)
    {
      lock (_lock)
      {
        return _schedules.Remove(schedule);
      }
    }

    internal void RemoveAll()
    {
      lock (_lock)
      {
        _schedules.Clear();
      }
    }

    internal Schedule First()
    {
      lock (_lock)
      {
        return _schedules.FirstOrDefault();
      }
    }

    internal Schedule Find(string name)
    {
      lock (_lock)
      {
        return _schedules.FirstOrDefault(x => x.Name == name);
      }
    }
  }
}