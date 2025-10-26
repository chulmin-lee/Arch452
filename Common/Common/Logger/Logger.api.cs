using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Common
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

  public partial class Logger
  {
    public string[] LEVEL { get; private set; } = new string[0];
    public bool TraceEnabled { get; private set; }
    public bool DebugEnabled { get; private set; }
    public bool InfoEnabled { get; private set; }
    public bool WarnEnabled { get; private set; }
    public bool ErrorEnabled { get; private set; }

    LOG_LEVEL _minLogLevel = LOG_LEVEL.DEBUG;
    public LOG_LEVEL MinLogLevel
    {
      get => _minLogLevel;
      set
      {
        if (value != _minLogLevel)
        {
          this.SetLogLevel(value);
        }
      }
    }
    public void SetLogLevel(LOG_LEVEL level)
    {
      _minLogLevel = level;

      this.TraceEnabled = true;
      this.DebugEnabled = true;
      this.InfoEnabled = true;
      this.WarnEnabled = true;
      this.ErrorEnabled = true;

      switch (this.MinLogLevel)
      {
        case LOG_LEVEL.DEBUG:
          this.TraceEnabled = false;
          break;
        case LOG_LEVEL.INFO:
          this.TraceEnabled = this.DebugEnabled = false;
          break;
        case LOG_LEVEL.WARN:
          this.TraceEnabled = this.DebugEnabled = this.InfoEnabled = false;
          break;
        case LOG_LEVEL.ERROR:
        case LOG_LEVEL.EXCEPTION:
          this.TraceEnabled = false;
          this.DebugEnabled = false;
          this.InfoEnabled = false;
          this.WarnEnabled = false;
          break;
      }
    }
    void LogLevelInitialize()
    {
      var list = new List<string>();
      var type = typeof(LOG_LEVEL);
      foreach (LOG_LEVEL value in Enum.GetValues(type))
      {
        var attt = type.GetField(value.ToString())?
                     .GetCustomAttributes(typeof(DescriptionAttribute), false)?
                     .FirstOrDefault() as DescriptionAttribute;
        list.Add(attt?.Description ?? value.ToString().Substring(3));
      }

      this.LEVEL = list.ToArray();
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    public ProcessingTime RunningTime(string msg)
    {
      return new ProcessingTime(this, msg);
    }
  }
}