using System.Threading.Tasks;

namespace Common
{
  internal class ScheduleTask
  {
    public Schedule Schedule { get; set; }
    public Task Task { get; set; }

    public ScheduleTask(Schedule schedule)
    {
      this.Schedule = schedule;
    }
  }
}