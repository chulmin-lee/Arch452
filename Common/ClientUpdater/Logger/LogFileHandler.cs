using System;
using System.IO;

namespace ClientUpdater
{
  public static class LogHandlerFactory
  {
    public static LogFileHandler GetLogHandler(bool common, string filename = "log.txt", string dir = "", bool day_test = false)
    {
      if (common)
      {
        return new UpdaterLogFile(filename, dir, day_test: day_test);
      }
      else
      {
        return new DateLogFile(filename, dir, day_test);
      }
    }
  }

  public class DayChangeInfo
  {
    public DateTime Today;
    public string BackupPath;
    public string CureentLogFile;
  }

  /// <summary>
  /// 공통이름을 사용하는경우는 나중에 백업폴더로 옮기고
  /// 날짜기반을 사용하는 경우는 처음부터 특정 폴더에 저장한다.
  /// 날짜가 바뀌면 그때 이름만 바꾼다.
  /// </summary>
  public abstract class LogFileHandler
  {
    // 날짜가 변경되는 경우 통지
    public event EventHandler<DayChangeInfo> DayChanged;
    public bool NeedBackup { get; protected set; } // 로그 백업하는 경우
    protected string LogDirectory; // 로그 파일이 위치할 경로
    protected string Filename = "log";
    protected string Extension = "txt";
    public bool IsDayTestMode { get; protected set; }
    int _today;

    public LogFileHandler(bool backup, string filename = "log.txt", string dir = "", bool day_test = false)
    {
      this.NeedBackup = backup;
      this.IsDayTestMode = day_test;

      _today = DateTime.Now.Day;

      //LogDirectory = FileHelper.RootRelativePath(dir);
      LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);

      if (!string.IsNullOrEmpty(filename))
      {
        if (filename.IndexOf('.') == -1)
        {
          Filename = filename;
        }
        else
        {
          var arr = filename.Split('.');
          Filename = arr[0];
          Extension = arr[1];
        }
      }

      StartDayChangeTimer();
    }
    protected string GetDateFilename(DateTime d)
    {
      return (this.IsDayTestMode) ? $"{Filename}-{d.ToString("yyyy-MM-dd-hh-mm")}.{Extension}"
                                  : $"{Filename}-{d.ToString("yyyy-MM-dd")}.{Extension}";
    }
    public abstract string GetCurrentLogFilePath();

    public virtual string GetBackupFilePath(DateTime d)
    {
      return null;
    }

    System.Timers.Timer _dayChangeTimer;
    void StartDayChangeTimer()
    {
      _dayChangeTimer = new System.Timers.Timer();
      _dayChangeTimer.Elapsed += TimerExpired;
      _dayChangeTimer.Interval = get_remain_time(DateTime.Now);
      _dayChangeTimer.Start();
    }

    void TimerExpired(object sender, System.Timers.ElapsedEventArgs e)
    {
      DateTime today = DateTime.Now;
      string backupfile = null;

      if (this.NeedBackup)
      {
        if (this.IsDayTestMode)
        {
          backupfile = this.GetBackupFilePath(today);
        }
        else
        {
          today = today.Date;  // 자정 기준 시간
          if (today.Day == _today)
          {
            today = today.AddDays(1);
          }
          var backupday = today.AddDays(-1);
          backupfile = this.GetBackupFilePath(backupday);
        }
      }

      var p = new DayChangeInfo()
      {
        Today = today,
        BackupPath = backupfile,
        CureentLogFile = this.GetCurrentLogFilePath()
      };

      this.DayChanged?.Invoke(this, p);

      _today = today.Day;
      (sender as System.Timers.Timer).Interval = get_remain_time(today);
    }
    /// <summary>
    /// 자정까지 남은 시간을 ms로 계산
    /// Timer 만료가 부정확해서 자정 직전에 만료된 경우가 있을 수 있으므로
    /// </summary>
    /// <param name="today">만료 계산 기준 시간</param>
    /// <returns></returns>
    double get_remain_time(DateTime today)
    {
      DateTime tomorrow;
      if (this.IsDayTestMode)
      {
        // 테스트 목적임. 무조건 2분후에 파일 바꾸기
        tomorrow = today.AddMinutes(2);
      }
      else
      {
        tomorrow = today.Date.AddDays(1).AddMilliseconds(1);
      }
      var ts = tomorrow - DateTime.Now;
      return ts.TotalMilliseconds;
    }
  }

  /// <summary>
  /// 고정 로그 파일명을 사용하고 백업시에 날짜를 붙인다.
  /// </summary>
  public class UpdaterLogFile : LogFileHandler
  {
    string BackupDirectory;
    public UpdaterLogFile(string filename = "log.txt", string dir = "", string backupdir = "LOG-BACKUP", bool day_test = false) : base(true, filename, dir, day_test)
    {
      BackupDirectory = Path.Combine(LogDirectory, backupdir);
    }
    public override string GetCurrentLogFilePath()
    {
      return Path.Combine(LogDirectory, $"{Filename}.{Extension}");
    }

    public override string GetBackupFilePath(DateTime d)
    {
      string filename = GetDateFilename(d);
      string path = Path.Combine(BackupDirectory, d.Year.ToString(), d.Month.ToString("D2"));

      Directory.CreateDirectory(path);

      return Path.Combine(path, filename);
    }
  }

  /// <summary>
  /// 날짜 기반 로그 파일명을 사용한다. 백업은 없다.
  /// </summary>
  public class DateLogFile : LogFileHandler
  {
    public DateLogFile(string filename = "log.txt", string dir = "LOG", bool day_test = false) : base(false, filename, dir, day_test)
    {
    }

    public override string GetCurrentLogFilePath()
    {
      var d = DateTime.Now;
      string path = Path.Combine(LogDirectory, d.Year.ToString(), d.Month.ToString("D2"));
      Directory.CreateDirectory(path);
      return Path.Combine(path, GetDateFilename(d));
    }
  }
}