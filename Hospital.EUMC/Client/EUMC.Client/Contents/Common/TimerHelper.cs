using System;
using System.Windows.Threading;

namespace EUMC.Client
{
  internal static class TimerHelper
  {
    public static void RunOnce(Action action, int interval = 10)
    {
      var timer = new DispatcherTimer();
      timer.Interval = TimeSpan.FromSeconds(interval);
      timer.Tick += (s, e) =>
      {
        (s as DispatcherTimer)?.Stop();
        action();
      };
      timer.Start();
    }
  }
}