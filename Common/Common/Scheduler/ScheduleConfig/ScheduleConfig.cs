using System;

namespace Common
{
  public enum SCHEDULE_MODE { NONE, PERIODIC, AT_TIME }

  public class ScheduleConfig
  {
    public SCHEDULE_MODE Mode { get; set; }
    public int Interval { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public ScheduleConfig(SCHEDULE_MODE mode = SCHEDULE_MODE.NONE)
    {
      this.Mode = mode;
    }
    public override string ToString()
    {
      return Mode == SCHEDULE_MODE.PERIODIC ? $"interval = {this.Interval} sec"
                                            : $"At {this.Hour}:{this.Minute}";
    }
  }
  public class AtTimeSchedule : ScheduleConfig
  {
    public AtTimeSchedule(TimeSpan ts) : base(SCHEDULE_MODE.AT_TIME)
    {
      this.Hour = ts.Hours;
      this.Minute = ts.Minutes;
    }
  }
  public class PeriodicSchedule : ScheduleConfig
  {
    public PeriodicSchedule(int interval) : base(SCHEDULE_MODE.PERIODIC)
    {
      this.Interval = interval;
    }
  }
}