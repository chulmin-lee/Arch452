using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Common
{
  //--------------------------------------------
  // 주의:
  // - Release 모드에서는 최적화를 위해서 함수들이 inline으로 변경될 수 있다.
  //   이 경우 호출 계층을 고려해서 작성된 StackFrame 코드가 맞지 않을 수 있으므로
  //   StackFrame을 사용하는 함수들은 inline되지 않도록 해야한다.
  //--------------------------------------------
  public static class LOG
  {
    static Logger _instance;
    static Logger Instance => _instance ?? (_instance = create_log_instance());

    public static void Initialize(string filename = "log.txt", string dir = "", LOG_LEVEL loglevel = LOG_LEVEL.DEBUG, bool delete = false)
    {
      if (_instance == null)
      {
        _instance = create_log_instance(filename, dir, loglevel, delete);
      }
    }
    static Logger create_log_instance(string filename = "log.txt", string dir = "", LOG_LEVEL level = LOG_LEVEL.DEBUG, bool delete = false)
    {
      if (delete)
      {
        var path = Path.Combine(dir, filename);
        File.Delete(path);
      }

      var log = new Logger(filename, dir, level);
      AppDomain.CurrentDomain.ProcessExit += (s, e) => { log.StopLogging(); };
      return log;
    }

    //==========================================
    // Log Level 처리
    //==========================================
    public static LOG_LEVEL LogLevel => Instance.MinLogLevel;
    public static bool TraceEnabled => Instance.TraceEnabled;
    public static bool DebugEnabled => Instance.DebugEnabled;
    public static bool InfoEnabled => Instance.InfoEnabled;
    public static bool WarnEnabled => Instance.WarnEnabled;
    public static bool ErrorEnabled => Instance.ErrorEnabled;

    public static void ChangeLogLevel(LOG_LEVEL level)
    {
      Instance.SetLogLevel(level);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void logger(string s, LOG_LEVEL level, bool caller = false)
    {
      if (Instance.MinLogLevel <= level)
      {
        if (caller)
        {
          var m = new StackFrame(2).GetMethod();
          if (m != null)
          {
            s = $"{m.ReflectedType?.Name}::{m.Name}() {s}";
          }
        }
        Instance.Enqueue(string.Format("{0} [{1}][{2}] {3}",
                              DateTime.Now.ToString("HH:mm:ss.fff"),
                              Instance.LEVEL[(int)level],
                              Thread.CurrentThread.ManagedThreadId,
                              s));
      }
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void except(Exception ex)
    {
      if (ex == null) return;
      var sb = new StringBuilder();
      exception(ex, sb);
      logger(sb.ToString(), LOG_LEVEL.EXCEPTION);
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void exception(Exception ex, StringBuilder sb)
    {
      if (ex == null) return;

      string caller = string.Empty;

      var m = new StackFrame(3).GetMethod();
      if (m != null)
      {
        caller = $"{m.ReflectedType?.Name}::{m.Name}() ";
      }

      sb.AppendLine($"Exception: {caller}");
      sb.AppendLine($"{ex.GetType().FullName}");
      sb.AppendLine($"{ex.Message}");
      var src = ex.TargetSite == null || ex.TargetSite.DeclaringType == null ? ex.Source : string.Format("{0}.{1}", ex.TargetSite.DeclaringType.FullName, ex.TargetSite.Name);
      sb.AppendLine($"Source : {src}");

      if (ex.InnerException == null)
      {
        sb.AppendLine("StackTrace");
        sb.AppendLine(ex.StackTrace);
      }
      else
      {
        exception(ex.InnerException, sb);
      }
    }
    public static ProcessingTime RunningTime(string s)
    {
      return Instance.RunningTime(s);
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
    // etc
    //----------------------------------------------------
  }
}