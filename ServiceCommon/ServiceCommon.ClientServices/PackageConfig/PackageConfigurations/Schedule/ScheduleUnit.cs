using System;

namespace ServiceCommon.ClientServices
{
  public class ScheduleUnit
  {
    public bool IsValid = true;
    public bool IsDefault { get; set; }
    public TimeRange TimeRange { get; set; }

    public PlaylistSchedule Schedule { get; set; }

    public TimeSpan StartTime
    {
      get => this.TimeRange.Start;
      set
      {
        this.TimeRange.ChangeStartTime(value);
      }
    }
    public TimeSpan EndTime
    {
      get => this.TimeRange.End;
      set
      {
        this.TimeRange.ChangeEndTime(value);
      }
    }

    public ScheduleUnit(PlaylistSchedule config)
    {
      if (config == null) throw new ArgumentNullException(nameof(config));
      this.Schedule = config;
      this.TimeRange = config.TimeRange;
      this.IsDefault = config.IsDefaultPackage;
    }
    public ScheduleUnit(string package)
    {
      this.Schedule = new PlaylistSchedule(package, new TimeRange());
      this.TimeRange = this.Schedule.TimeRange;
    }

    public ScheduleUnit CreateClone(TimeRange range)
    {
      var o = (ScheduleUnit)this.MemberwiseClone();
      o.TimeRange = range;
      return o;
    }
    public bool IsInRange(TimeSpan ts)
    {
      return this.TimeRange.IsInRange(ts);
    }

    public override string ToString()
    {
      return $"{this.Schedule.PackageName}/{this.Schedule.Mode}/{this.Schedule.No} : {this.TimeRange.Start} ~ {this.TimeRange.End}";
    }
  }
}