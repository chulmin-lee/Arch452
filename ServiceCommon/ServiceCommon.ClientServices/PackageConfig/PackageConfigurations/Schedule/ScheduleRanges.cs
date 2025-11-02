using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.ClientServices
{
  public class Holidays
  {
    List<DateTime> _dates = new List<DateTime>();
    public bool IsEmpty => !_dates.Any();
    public Holidays() { }
    public Holidays(List<string> dates)
    {
      foreach (var day in dates)
      {
        this.AddDay(day);
      }
    }

    public void AddDay(string s)
    {
      var d = s.ToDate();
      if (d != DateTime.MinValue)
      {
        this._dates.Add(d.Date);
      }
    }

    public void Add(DateTime date) => _dates.Add(date.Date);
    public bool IsHoliday(DateTime d) => _dates.Contains(d.Date);
  }

  /// <summary>
  /// 시간 범위. 기본은 하루종일 이다
  /// </summary>
  public class TimeRange : IComparable<TimeRange>
  {
    public bool IsAlwaysOn { get; private set; } = true;
    public bool CanRunningNow => this.IsAlwaysOn || this.IsInRange();
    public int TotalSecond => (int)this.Interval.TotalSeconds;
    public TimeSpan Interval => _end - _start;
    public TimeSpan Start => _start;
    public TimeSpan End => _end;
    TimeSpan _start = TimeSpan.Zero;
    TimeSpan _end = new TimeSpan(24, 0, 0);

    public TimeRange() { }
    public TimeRange(string start, string end) : this(start.ToTimeSpan(), end.ToTimeSpan(true))
    {
    }
    public TimeRange(TimeSpan start, TimeSpan end)
    {
      this.IsAlwaysOn = false;
      _start = start; // < end ? start : end;
      _end = start < end ? end : new TimeSpan(24, 0, 0);
    }
    public void ChangeStartTime(TimeSpan ts)
    {
      if (ts < _end)
      {
        _start = ts;

        if (this.IsAlwaysOn)
        {
          // AlwaysOn 상태에서 시작시간을 변경하면 AlwaysOn 상태를 해제한다
          _end = new TimeSpan(24, 0, 0);
          this.IsAlwaysOn = false;
        }
      }
    }
    public void ChangeEndTime(TimeSpan ts)
    {
      if (ts > _start)
      {
        _end = ts;
        if (this.IsAlwaysOn)
        {
          // AlwaysOn 상태에서 종료시간을 변경하면 AlwaysOn 상태를 해제한다
          _start = TimeSpan.Zero;
          this.IsAlwaysOn = false;
        }
      }
    }
    public override string ToString()
    {
      if (this.IsAlwaysOn)
      {
        return $"TimeRange: AlwaysOn";
      }
      else
      {
        return $"TimeRange: {_start} ~ {_end}, Range: {this.Interval}";
      }
    }

    /// <summary>
    /// 지정된 시간이 설정된 시간에 포함되는가?
    /// </summary>
    public bool IsInRange() => this.IsInRange(DateTime.Now);
    public bool IsInRange(DateTime t) => this.IsInRange(t.TimeOfDay);
    public bool IsInRange(TimeSpan ts)
    {
      return this.IsAlwaysOn ? true : (_start <= ts) && (ts < _end);
    }
    /// <summary>
    /// 실행 가능성이 있는가? 지금은 설정 시간에 포함되어있지않아도, 이후에 포함될 수 있다면 true
    /// </summary>
    public bool IsAvailable() => this.IsAvailable(DateTime.Now.TimeOfDay);
    public bool IsAvailable(DateTime d) => this.IsAvailable(d.TimeOfDay);
    public bool IsAvailable(TimeSpan ts)
    {
      return this.IsAlwaysOn ? true : ts < _end;
    }
    public int CompareTo(TimeRange other)
    {
      if (other == null) return -1;
      if (this._start < other._start) return 1;
      else if (this._start > other._start) return -1;
      else return 0;
    }
    public override bool Equals(object obj)
    {
      var o = obj as TimeRange;
      if (Object.ReferenceEquals(this, o)) return true;
      if (o == null) return false;
      if (this.IsAlwaysOn == o.IsAlwaysOn) return true;
      return this._start == o._start && this._end == o._end;
    }
    public override int GetHashCode()
    {
      return this.IsAlwaysOn ? 0 : (_start.GetHashCode() ^ _end.GetHashCode());
    }

    public static bool operator ==(TimeRange left, TimeRange right)
    {
      if (ReferenceEquals(left, null))
      {
        return ReferenceEquals(right, null);
      }
      return left.Equals(right);
    }

    public static bool operator !=(TimeRange left, TimeRange right)
    {
      return !(left == right);
    }
    public TimeRange Intersect(TimeRange o)
    {
      if (o == null || !this.IsOverlap(o))
        return null;
      var start = this._start < o._start ? o._start : this._start;
      var end = this._end > o._end ? o._end : this._end;
      return new TimeRange(start, end);
    }

    public bool Merge(TimeRange o)
    {
      if (o != null && this.IsOverlap(o))
      {
        this._start = this._start < o._start ? this._start : o._start;
        this._end = this._end > o._end ? this._end : o._end;
        return true;
      }
      return false;
    }
    public bool IsOverlap(TimeRange o)
    {
      if (this.IsAlwaysOn || o.IsAlwaysOn)
      {
        return true;
      }
      return this._start < o._end && this._end > o._start;
    }
    /// <summary>
    /// 문자열을 날짜 범위로 변경
    /// 예) ",17:00~05:00"
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static List<TimeRange> ConvertToTimeRanges(string s)
    {
      var ranges = new List<TimeRange>();
      foreach (var p in s.ToStringList())
      {
        var times = p.ToStringList('~');
        if (times.Count >= 2)
        {
          ranges.AddRange(ConvertToTimeRanges(times[0], times[1]));
        }
      }
      if (!ranges.Any())
      {
        ranges.Add(new TimeRange());
      }
      return ranges;
    }
    public static List<TimeRange> ConvertToTimeRanges(string s, string e)
    {
      var ranges = new List<TimeRange>();

      var start = s.ToTimeSpan();
      var end = e.ToTimeSpan(true);

      if (start == end)
      {
        ranges.Add(new TimeRange());
      }
      else if (start < end)
      {
        ranges.Add(new TimeRange(start, end));
      }
      else
      {
        // ex: 17:00 ~ 07:00
        ranges.Add(new TimeRange(TimeSpan.Zero, end)); // 00:00~07:00
        ranges.Add(new TimeRange(start, new TimeSpan(24, 0, 0)));  // 17:00~24:00
      }
      return ranges;
    }
  }
  public class DateRange : IComparable<DateRange>
  {
    /// <summary>
    /// 날짜가 지정되지 않으면 항상 true이다
    /// </summary>
    public bool IsAlwaysOn { get; private set; } = true;
    public bool CanRunningNow => this.IsAlwaysOn || this.IsInRange();
    public DateTime Start => _start;
    public DateTime End => _end;
    DateTime _start = DateTime.MinValue;
    DateTime _end = DateTime.MinValue;

    public DateRange() { }
    public DateRange(string start, string end) : this(start.ToDate(), end.ToDate()) { }
    public DateRange(DateTime start, DateTime end)
    {
      if ((start == DateTime.MinValue) || (end == DateTime.MinValue))
        return;

      this.IsAlwaysOn = false;
      _start = start < end ? start : end;
      _end = start < end ? end : start;
    }

    /// <summary>
    /// 기간이 설정되어 있지않거나, 현재 날짜가 기간에 포함되면 true
    /// </summary>
    public bool IsInRange() => this.IsInRange(DateTime.Now);
    public bool IsInRange(DateTime t)
    {
      var ts = t.Date;
      return this.IsAlwaysOn ? true : (_start <= ts) && (ts <= _end);
    }

    /// <summary>
    /// 실행 가능성이 있는가?
    /// </summary>
    public bool IsAvailable() => this.IsAvailable(DateTime.Now);
    public bool IsAvailable(DateTime d)
    {
      return this.IsAlwaysOn ? true : d.Date <= _end;
    }

    public int CompareTo(DateRange other)
    {
      if (other == null) return -1;
      if (this._start < other._start) return 1;
      else if (this._start > other._start) return -1;
      else return 0;
    }

    public override bool Equals(object obj)
    {
      var o = obj as DateRange;
      if (Object.ReferenceEquals(this, o)) return true;
      if (o == null) return false;
      if (this.IsAlwaysOn == o.IsAlwaysOn) return true;
      return this._start == o._start && this._end == o._end;
    }
    public override int GetHashCode()
    {
      return this.IsAlwaysOn ? 0 : (_start.GetHashCode() ^ _end.GetHashCode());
    }
    public static bool operator ==(DateRange left, DateRange right)
    {
      if (ReferenceEquals(left, null))
      {
        return ReferenceEquals(right, null);
      }
      return left.Equals(right);
    }

    public static bool operator !=(DateRange left, DateRange right)
    {
      return !(left == right);
    }

    public bool Merge(DateRange o)
    {
      if (o != null && this.IsOverlap(o))
      {
        this._start = this._start < o._start ? this._start : o._start;
        this._end = this._end > o._end ? this._end : o._end;
        return true;
      }
      return false;
    }
    public bool IsOverlap(DateRange o)
    {
      if (this.IsAlwaysOn || o.IsAlwaysOn)
      {
        return true;
      }
      return this._start < o._end && this._end > o._start;
    }
  }

  /// <summary>
  /// 요일 범위
  /// DspConfig의 week_value는 A schedule에만 해당한다
  /// B schedule들의 week_value는 서로 겹칠 수 있다
  /// </summary>

  public class WeekdayRange
  {
    public bool IsAlwaysOn { get; private set; } = true;
    public bool CanRunningNow => this.IsAlwaysOn || this.IsInRange();
    public List<DayOfWeek> WeekDays { get; private set; } = new List<DayOfWeek>();
    public WeekdayRange() { }

    /// <summary>
    /// 숫자로 된 입력갑
    /// </summary>
    public WeekdayRange(List<int> list)
    {
      this.WeekDays = ConvertToDayOfWeeks(list);
      this.IsAlwaysOn = this.WeekDays.Count == 7;
    }
    List<DayOfWeek> ConvertToDayOfWeeks(List<int> list)
    {
      int min = (int)DayOfWeek.Sunday;
      int max = (int)DayOfWeek.Saturday;
      return list.Where(x => (min <= x && x <= max)).Select(x => (DayOfWeek)x).ToList();
    }
    public bool IsInRange() => this.IsInRange(DateTime.Now.DayOfWeek);
    public bool IsInRange(DateTime d) => this.IsInRange(d.DayOfWeek);
    public bool IsInRange(DayOfWeek t)
    {
      return this.IsAlwaysOn ? true : this.WeekDays.Contains(t);
    }
    public override bool Equals(object obj)
    {
      var o = obj as WeekdayRange;
      if (Object.ReferenceEquals(this, o)) return true;
      if (o == null) return false;
      if (this.IsAlwaysOn == o.IsAlwaysOn) return true;
      if (this.WeekDays.Count != o.WeekDays.Count) return false;

      var first = this.WeekDays.Except(o.WeekDays).ToList().Count;
      var second = o.WeekDays.Except(this.WeekDays).ToList().Count;
      return first + second == 0;
    }
    public override int GetHashCode()
    {
      return this.IsAlwaysOn ? 0 : WeekDays.GetHashCode();
    }
    public static bool operator ==(WeekdayRange left, WeekdayRange right)
    {
      if (ReferenceEquals(left, null))
      {
        return ReferenceEquals(right, null);
      }
      return left.Equals(right);
    }

    public static bool operator !=(WeekdayRange left, WeekdayRange right)
    {
      return !(left == right);
    }
  }
}