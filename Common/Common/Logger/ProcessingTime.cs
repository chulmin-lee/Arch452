using System;

namespace Common
{
  /// <summary>
  /// using을 사용하여 특정 블럭에 소요된 시간을 출력한다.
  /// </summary>
  public class ProcessingTime : IDisposable
  {
    DateTime _start = DateTime.Now;
    string _msg;
    Logger _logger;
    public ProcessingTime(Logger logger, string msg)
    {
      _msg = msg;
      _logger = logger;
      _logger.Enqueue($"{_msg} : Started", LOG_LEVEL.INFO);
    }
    public void Dispose()
    {
      var elapsed = (DateTime.Now - _start).TotalMilliseconds;
      _logger.Enqueue($"{_msg} : Elasped (ms) = {elapsed}", LOG_LEVEL.INFO);
    }
  }
}