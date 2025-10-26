using System;
using System.Threading;

namespace Common
{
  public static class TimerHelper
  {
    // UI timer
    //public static void RunOnce(Action action, int interval = 10)
    //{
    //  var timer = new System.Windows.Threading.DispatcherTimer();
    //  timer.Interval = TimeSpan.FromSeconds(interval);
    //  timer.Tick += (s, e) =>
    //  {
    //    (s as System.Windows.Threading.DispatcherTimer)?.Stop();
    //    action();
    //  };
    //  timer.Start();
    //}

    public static void RunOnce(Action action, int msec = 1000)
    {
      // msec 후 한번만 실행되는 타이머
      new Timer(s => action(), null, msec, -1);
    }
  }
}