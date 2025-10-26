using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ClientUpdater
{
  //--------------------------------------------
  // 주의:
  // - Release 모드에서는 최적화를 위해서 함수들이 inline으로 변경될 수 있다.
  //   이 경우 호출 계층을 고려해서 작성된 StackFrame 코드가 맞지 않을 수 있으므로
  //   StackFrame을 사용하는 함수들은 inline되지 않도록 해야한다.
  //--------------------------------------------
  public static class log
  {
    static IStaticLog _instance;

    public static void Initialize(string filename = "log.txt", string dir = "", LOG_LEVEL loglevel = LOG_LEVEL.DEBUG)
    {
      internal_initialize(true, filename, dir, loglevel);
    }
    public static void DateLogInitialize(string filename = "log.txt", string dir = "LOG", LOG_LEVEL loglevel = LOG_LEVEL.DEBUG)
    {
      internal_initialize(false, filename, dir, loglevel);
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="common">공통 log 파일 사용 여부</param>
    /// <param name="filename"></param>
    /// <param name="dir"></param>
    /// <param name="level"></param>
    static void internal_initialize(bool common, string filename, string dir, LOG_LEVEL level)
    {
      if (_instance == null)
      {
        bool test = false;
        var logHandler = LogHandlerFactory.GetLogHandler(common, filename, dir, test);

        _instance = new Logger(logHandler, level);
        AppDomain.CurrentDomain.ProcessExit += (s, e) => Stop();
      }
    }

    public static void Stop()
    {
      _instance?.StopLogging();
      _instance = null;
    }

    //==========================================
    // Log Level 처리
    //==========================================
    public static LOG_LEVEL LogLevel => _instance.MinLogLevel;
    public static bool TraceEnabled => can_logging(LOG_LEVEL.TRACE);
    public static bool DebugEnabled => can_logging(LOG_LEVEL.DEBUG);
    public static bool InfoEnabled => can_logging(LOG_LEVEL.INFO);
    public static bool WarnEnabled => can_logging(LOG_LEVEL.WARN);
    public static bool ErrorEnabled => can_logging(LOG_LEVEL.ERROR);

    static bool can_logging(LOG_LEVEL level)
    {
      return _instance != null ? _instance.MinLogLevel <= level : false;
    }

    public static void ChangeLogLevel(LOG_LEVEL level)
    {
      _instance?.SetLogLevel(level);
    }
    //----------------------------------------------------
    // Log Level : TRACE
    //----------------------------------------------------
    [MethodImpl(MethodImplOptions.NoInlining)] public static void t(object s) => logger(s?.ToString(), LOG_LEVEL.TRACE);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void tc(object s) => logger(s?.ToString(), LOG_LEVEL.TRACE, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void t(string s = null) => logger(s, LOG_LEVEL.TRACE);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void tc(string s = null) => logger(s, LOG_LEVEL.TRACE, true);

    // 사용예
    // log.v(() => long_func());
    // log.v(() => $"result = {long_func()}");
    [MethodImpl(MethodImplOptions.NoInlining)] public static void t(Func<string> action) { if (TraceEnabled) logger(action(), LOG_LEVEL.TRACE); }
    [MethodImpl(MethodImplOptions.NoInlining)] public static void tc(Func<string> action) { if (TraceEnabled) logger(action(), LOG_LEVEL.TRACE, true); }

    //----------------------------------------------------
    // Log Level : DEBUG
    //----------------------------------------------------
    [MethodImpl(MethodImplOptions.NoInlining)] public static void d(object s) => logger(s?.ToString(), LOG_LEVEL.DEBUG);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void dc(object s) => logger(s?.ToString(), LOG_LEVEL.DEBUG, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void d(string s = null) => logger(s, LOG_LEVEL.DEBUG);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void dc(string s = null) => logger(s, LOG_LEVEL.DEBUG, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void d(Func<string> action) { if (DebugEnabled) logger(action(), LOG_LEVEL.DEBUG); }
    [MethodImpl(MethodImplOptions.NoInlining)] public static void dc(Func<string> action) { if (DebugEnabled) logger(action(), LOG_LEVEL.DEBUG, true); }
    //----------------------------------------------------
    // Log Level : INFO
    //----------------------------------------------------
    [MethodImpl(MethodImplOptions.NoInlining)] public static void i(object s) => logger(s?.ToString(), LOG_LEVEL.INFO);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void ic(object s) => logger(s?.ToString(), LOG_LEVEL.INFO, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void i(string s = null) => logger(s, LOG_LEVEL.INFO);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void ic(string s = null) => logger(s, LOG_LEVEL.INFO, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void i(Func<string> action) { if (InfoEnabled) logger(action(), LOG_LEVEL.INFO); }
    [MethodImpl(MethodImplOptions.NoInlining)] public static void ic(Func<string> action) { if (InfoEnabled) logger(action(), LOG_LEVEL.INFO, true); }
    //----------------------------------------------------
    // Log Level : WARN
    //----------------------------------------------------
    [MethodImpl(MethodImplOptions.NoInlining)] public static void w(object s) => logger(s?.ToString(), LOG_LEVEL.WARN);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void wc(object s) => logger(s?.ToString(), LOG_LEVEL.WARN, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void w(string s = null) => logger(s, LOG_LEVEL.WARN);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void wc(string s = null) => logger(s, LOG_LEVEL.WARN, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void w(Func<string> action) { if (WarnEnabled) logger(action(), LOG_LEVEL.WARN); }
    [MethodImpl(MethodImplOptions.NoInlining)] public static void wc(Func<string> action) { if (WarnEnabled) logger(action(), LOG_LEVEL.WARN, true); }
    //----------------------------------------------------
    // Log Level : ERROR
    //----------------------------------------------------
    [MethodImpl(MethodImplOptions.NoInlining)] public static void e(object s) => logger(s?.ToString(), LOG_LEVEL.ERROR);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void ec(object s) => logger(s?.ToString(), LOG_LEVEL.ERROR, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void e(string s = null) => logger(s, LOG_LEVEL.ERROR);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void ec(string s = null) => logger(s, LOG_LEVEL.ERROR, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void Error(string s = null) => logger(s, LOG_LEVEL.ERROR, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void e(Func<string> action) { if (ErrorEnabled) logger(action(), LOG_LEVEL.ERROR); }
    [MethodImpl(MethodImplOptions.NoInlining)] public static void ec(Func<string> action) { if (ErrorEnabled) logger(action(), LOG_LEVEL.ERROR, true); }

    //----------------------------------------------------
    // Log Level : ALL. 어떤 경우에도 출력
    //----------------------------------------------------
    [MethodImpl(MethodImplOptions.NoInlining)] public static void a(object s) => logger(s?.ToString(), LOG_LEVEL.ALL);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void ac(object s) => logger(s?.ToString(), LOG_LEVEL.ALL, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void a(string s = null) => logger(s, LOG_LEVEL.ALL);
    [MethodImpl(MethodImplOptions.NoInlining)] public static void ac(string s = null) => logger(s, LOG_LEVEL.ALL, true);

    //----------------------------------------------------
    // logger
    //----------------------------------------------------
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void logger(string s, LOG_LEVEL level, bool caller = false)
    {
      if (can_logging(level))
      {
        if (caller)
        {
          s = $"{GetCaller(3)}{s}";
        }
        _instance.Enqueue(s, level);
      }
    }

    public static ProcessingTime RunningTime(string s, int error_time_ms = 0)
    {
      var stack = GetCaller(2);
      return _instance?.RunningTime(s, error_time_ms, stack);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static string GetCaller(int skip)
    {
      MethodBase m = new StackFrame(skip).GetMethod();
      return $"{m.ReflectedType.Name}::{m.Name}() ";
    }
    //----------------------------------------------------
    // Log Level : EXCEPTION
    //----------------------------------------------------
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Error(string s, Exception ex)
    {
      logger(s, LOG_LEVEL.ERROR, true);
      _instance.except(ex);
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void except(Exception ex) { _instance.except(ex); }
  }
}