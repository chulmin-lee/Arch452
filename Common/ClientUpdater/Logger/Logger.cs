using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientUpdater
{
  // Instance mode로 사용할때는 Stop()을 호출해줘야 한다.

  public class Logger : ILogger
  {
    public LOG_LEVEL MinLogLevel { get; private set; } = LOG_LEVEL.DEBUG;
    string[] LEVEL;
    BlockingCollection<string> _bc = null;
    Task _task;
    int _maxBulkLines = 1000;  // 한꺼번에 저장할 최대 라인 제한
    //int _maxLineLength = 1024; // 문자열 최대 길이 제한
    LogFileHandler _logFileInfo;
    string _logFile;   // default log file path
    object _backup_lock = new object();

    public Logger(LogFileHandler path, LOG_LEVEL loglevel = LOG_LEVEL.DEBUG)
    {
      this.MinLogLevel = loglevel;
      this.Initialize();

      _logFileInfo = path;
      _logFileInfo.DayChanged += DayChanged;
      _logFile = _logFileInfo.GetCurrentLogFilePath();

      if (_logFileInfo.NeedBackup)
      {
        backup_old_log();
      }
      _bc = new BlockingCollection<string>();
      _task = new Task(() => LoggingProc(_bc), TaskCreationOptions.LongRunning);
      _task.Start();
      _bc.Add($"--------- logging started ({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")})---------");
    }
    public void SetLogLevel(LOG_LEVEL level)
    {
      MinLogLevel = level;
    }
    void Initialize()
    {
      int count = Enum.GetValues(typeof(LOG_LEVEL)).Length;
      this.LEVEL = new string[count];

      var type = typeof(LOG_LEVEL);
      int index = 0;
      foreach (LOG_LEVEL value in Enum.GetValues(type))
      {
        string name = Enum.GetName(type, value);
        if (name != null)
        {
          FieldInfo field = type.GetField(name);
          if (field != null)
          {
            var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attr != null)
            {
              LEVEL[index++] = attr.Description;
            }
          }
        }
      }
    }
    /// <summary>
    /// 외부에서 호출하는 경우 이미 caller에 대한 처리를 마친 상태이다.
    /// </summary>
    public void Enqueue(string s, LOG_LEVEL level)
    {
      Enqueue(s, level, false);
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    void Enqueue(string s, LOG_LEVEL level, bool caller = false)
    {
      if (!_bc.IsAddingCompleted && level >= MinLogLevel)
      {
        string stack = caller ? GetCaller(3) : null;
        var msg = string.Format("{0} [{1}][{2}] {3}{4}",
                                DateTime.Now.ToString("HH:mm:ss.fff"),
                                LEVEL[(int)level],
                                Thread.CurrentThread.ManagedThreadId,
                                stack,
                                s);
        // 최대 길이 제한 (Stack Trace는 다 나와야 한다)
        // 시간 오래걸리는것은 나중에 검사하자
        //if (msg.Length > _maxLineLength && !msg.IsMultilines())
        //{
        //  msg = $"{msg.Substring(0, _maxLineLength)} ...";
        //}
        _bc.Add(msg);
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining)] public void t(string s) => Enqueue(s, LOG_LEVEL.TRACE);
    [MethodImpl(MethodImplOptions.NoInlining)] public void d(string s) => Enqueue(s, LOG_LEVEL.DEBUG);
    [MethodImpl(MethodImplOptions.NoInlining)] public void i(string s) => Enqueue(s, LOG_LEVEL.INFO);
    [MethodImpl(MethodImplOptions.NoInlining)] public void w(string s) => Enqueue(s, LOG_LEVEL.WARN);
    [MethodImpl(MethodImplOptions.NoInlining)] public void e(string s) => Enqueue(s, LOG_LEVEL.ERROR);
    [MethodImpl(MethodImplOptions.NoInlining)] public void a(string s) => Enqueue(s, LOG_LEVEL.ALL);
    // with caller
    [MethodImpl(MethodImplOptions.NoInlining)] public void tc(string s) => Enqueue(s, LOG_LEVEL.TRACE, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public void dc(string s) => Enqueue(s, LOG_LEVEL.DEBUG, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public void ic(string s) => Enqueue(s, LOG_LEVEL.INFO, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public void wc(string s) => Enqueue(s, LOG_LEVEL.WARN, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public void ec(string s) => Enqueue(s, LOG_LEVEL.ERROR, true);
    [MethodImpl(MethodImplOptions.NoInlining)] public void ac(string s) => Enqueue(s, LOG_LEVEL.ALL, true);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void except(Exception ex)
    {
      if (ex != null)
      {
        StringBuilder sb = new StringBuilder();
        exception(ex, sb);
        Enqueue(sb.ToString(), LOG_LEVEL.EXCEPTION);
      }
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    void exception(Exception ex, StringBuilder sb)
    {
      if (ex == null) return;

      sb.AppendLine($"Exception: {GetCaller(4)}");
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
    public void StopLogging()
    {
      Trace.WriteLine("stop logging");

      if (_task == null || _bc.IsAddingCompleted || _bc.IsCompleted)
      {
        return;
      }

      _bc.CompleteAdding();

      while (!_bc.IsCompleted)
      {
        // queue가 비워지기를 기다린다.
        Thread.Sleep(100);
      }
      _task.Wait(1000);
      _task = null;
      Trace.WriteLine("stop logging completed");
    }

    void LoggingProc(BlockingCollection<string> bc)
    {
      int totalLogCount = 0;
      try
      {
        while (!bc.IsCompleted) // IsAddingCompleted=true 이지만 큐에는 데이타가 있을 수 있다.
        {
          string entry;
          if (bc.TryTake(out entry, -1))
          {
            if (bc.Count > 0)
            {
              var list = new List<string>() { entry};
              int count = 0;
              while (bc.TryTake(out entry, 1))
              {
                list.Add(entry);
                if (count++ >= _maxBulkLines)
                {
                  break;
                }
              }
              Trace.WriteLine($"multiple logging START. count={list.Count}");
              totalLogCount += list.Count;
              WriteLog(list);
            }
            else
            {
              totalLogCount++;
              WriteLog(entry);
            }
          }
        }
      }
      catch (InvalidOperationException)
      {
        Trace.WriteLine("IsCompleted");
      }
      //catch (OperationCanceledException)
      //{
      //  // Token Cancel
      //  Trace.WriteLine("Token cancel");
      //}
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message);
      }
      finally
      {
        Trace.WriteLine($"Total logging count={totalLogCount}");
      }
    }
    void WriteLog(string entry)
    {
      try
      {
        lock (_backup_lock)
        {
          using (StreamWriter sw = File.AppendText(_logFile))
          {
            sw.WriteLine(entry);
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message);
      }
    }
    void WriteLog(List<string> entries)
    {
      try
      {
        lock (_backup_lock)
        {
          using (StreamWriter sw = File.AppendText(_logFile))
          {
            foreach (var p in entries)
            {
              sw.WriteLine(p);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message);
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    string GetCaller(int skip = 1)
    {
      MethodBase m = new StackFrame(skip).GetMethod();
      return $"{m.ReflectedType.Name}::{m.Name}() ";
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    public ProcessingTime RunningTime(string msg, int error_time_ms = 0, string stack = null)
    {
      if (string.IsNullOrEmpty(stack))
      {
        stack = GetCaller(2);
      }
      return new ProcessingTime(this, msg, stack, error_time_ms);
    }

    #region DAY_CHANGED
    /// <summary>
    /// 현재 로그 파일 날짜가 오늘이 아니면 백업한다.
    /// </summary>
    void backup_old_log()
    {
      // 고정 파일명을 사용할때만 백업한다.
      if (_logFileInfo.NeedBackup && File.Exists(_logFile))
      {
        var modified = File.GetLastWriteTime(_logFile);

        // 년 또는 달이 바뀔 수 있으므로 day의 대소를 비교하면 안된다.
        // 한달 차이가 나는 경우 Day가 같을 수 있다.
        if (DateTime.Today != modified.Date)
        {
          string backupfile = _logFileInfo.GetBackupFilePath(modified);
          backup_current_log(backupfile);
        }
      }
    }
    private void DayChanged(object sender, DayChangeInfo e)
    {
      lock (_backup_lock)
      {
        if (_logFileInfo.NeedBackup)
        {
          backup_current_log(e.BackupPath);
        }
        else
        {
          _logFile = e.CureentLogFile;
        }
      }
    }
    /// <summary>
    /// 현재 로그 파일을 지정된 파일로 백업한다.
    /// </summary>
    void backup_current_log(string backupfile)
    {
      try
      {
        if (File.Exists(backupfile))
        {
          File.Delete(backupfile);
        }
        File.Move(_logFile, backupfile);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        throw ex;
      }
    }
    #endregion DAY_CHANGED
  }
}