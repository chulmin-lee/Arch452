using System;
using System.ComponentModel;

namespace ClientUpdater
{
  public enum LOG_LEVEL
  {
    [Description("TRC")] TRACE=0,
    [Description("DBG")] DEBUG,
    [Description("INF")] INFO,
    [Description("WRN")] WARN,
    [Description("ERR")] ERROR,
    [Description("EXC")] EXCEPTION,
    [Description("ALL")] ALL
  }

  public interface IStaticLog
  {
    void SetLogLevel(LOG_LEVEL level);
    void StopLogging();
    void Enqueue(string msg, LOG_LEVEL level);
    LOG_LEVEL MinLogLevel { get; }
    void except(Exception ex);
    ProcessingTime RunningTime(string msg, int error_time_ms = 0, string stack = null);
  }

  public interface ILogger : IStaticLog
  {
    void t(string s);
    void d(string s);
    void i(string s);
    void w(string s);
    void e(string s);
    void a(string s);

    void tc(string s);
    void dc(string s);
    void ic(string s);
    void wc(string s);
    void ec(string s);
    void ac(string s);
  }
}