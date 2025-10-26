using System;

namespace ClientUpdater
{
  /// <summary>
  /// using을 사용하여 특정 블럭에 소요된 시간을 출력한다.
  /// </summary>
  public class ProcessingTime : IDisposable
  {
    DateTime _start = DateTime.Now;
    string _msg;
    string _stack;
    IStaticLog instance;
    int _error_time_ms;

    public ProcessingTime(IStaticLog instance, string msg, string stack, int error_time_ms = 0)
    {
      _msg = msg;
      _stack = stack;
      _error_time_ms = error_time_ms;
      this.instance = instance;
      this.instance.Enqueue($"{_stack}[{_msg}] RunningTime Check Started", LOG_LEVEL.WARN);
    }
    public void Dispose()
    {
      var elapsed = (DateTime.Now - _start).TotalMilliseconds;
      var msg = $"{_stack}[{_msg}] RunningTime Elapsed: {elapsed} ms";
      LOG_LEVEL level = (_error_time_ms != 0 && elapsed >= _error_time_ms) ? LOG_LEVEL.ERROR : LOG_LEVEL.INFO;
      this.instance.Enqueue(msg, level);
    }
  }
}