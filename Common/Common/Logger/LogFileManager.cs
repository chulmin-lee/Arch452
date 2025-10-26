using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Common
{
  /// <summary>
  /// 로그 파일 백업 관련
  /// </summary>
  public class LogFileManager
  {
    public event EventHandler<string> DayChanged;
    int _today = DateTime.Now.Day;
    public string LogFilePath { get; private set; }
    string LogBackupDir;
    string Filename = "log";
    string Extension = "txt";

    Timer _midnightTimer;
    public LogFileManager(string filename, string dir)
    {
      this.LogFilePath = Path.Combine(dir, filename);
      this.LogBackupDir = Path.Combine(dir, "LOG-BACKUP");
      if (!string.IsNullOrEmpty(filename))
      {
        this.Filename = Path.GetFileNameWithoutExtension(filename);
        this.Extension = Path.GetExtension(filename);
      }

      // 현재 파일 백업
      if (File.Exists(this.LogFilePath))
      {
        // 년 또는 달이 바뀔 수 있으므로 day의 대소를 비교하면 안된다.
        // 한달 차이가 나는 경우 Day가 같을 수 있다.
        var modified = File.GetLastWriteTime(this.LogFilePath);
        if (DateTime.Now.Day != modified.Day)
        {
          this.CurrentLogBackup(this.GetBackupFilePath(modified));
        }
      }

      _midnightTimer = new Timer(s => MidnightOccured(), null, Timeout.Infinite, Timeout.Infinite);

      // Microsoft.Win32.SystemEvents
      SystemEvents.TimeChanged += (s, e) => this.StartMidnightTimer();
      this.StartMidnightTimer();
    }
    void StartMidnightTimer()
    {
      // 타이머 정지
      _midnightTimer.Change(Timeout.Infinite, Timeout.Infinite);
      // 타이머 시작
      _midnightTimer.Change(this.TimeLeftUntillMidnight(), TimeSpan.Zero);
    }
    public void CurrentLogBackup(string backupfile)
    {
      if (File.Exists(backupfile))
      {
        File.Delete(backupfile);
      }
      File.Move(this.LogFilePath, backupfile);

      this.CleanBackupFile(DateTime.Now);
    }

    public void CleanBackupFiles(DateTime d, int days = 30)
    {
      this.recursive_serach(this.LogBackupDir, d.AddDays(-days));
    }
    void recursive_serach(string upper, DateTime minDate)
    {
      if (!Directory.Exists(upper))
        return;

      foreach (var dir in Directory.GetDirectories(upper, "*", SearchOption.TopDirectoryOnly))
      {
        recursive_serach(dir, minDate);
      }

      foreach (var file in Directory.GetFiles(upper))
      {
        if (File.GetLastWriteTime(file) < minDate)
        {
          File.Delete(file);
        }
      }

      if (Directory.GetFiles(upper).Length == 0 &&
         Directory.GetDirectories(upper, "*", SearchOption.TopDirectoryOnly).Length == 0)
      {
        Directory.Delete(upper, true);
      }
    }

    /// <summary>
    /// 지정 날짜 기준으로 로그 삭제
    /// </summary>
    /// <param name="d">기준 날짜</param>
    /// <param name="days">보관할 범위</param>
    public void CleanBackupFile(DateTime d, int days = 30)
    {
      if (!Directory.Exists(this.LogBackupDir))
        return;

      var s = "2025-01-22";
      int date_str_len = s.Length;
      var minDate = d.AddDays(-days);

      foreach (string top in Directory.GetDirectories(this.LogBackupDir, "*", SearchOption.TopDirectoryOnly))
      {
        // ...\Debug\net8.0\LOG-BACKUP\2025\
        if (int.TryParse(top.Split('\\').Last(), out var year))
        {
          if (year >= minDate.Year)
          {
            foreach (var m in Directory.GetDirectories(top, "*", SearchOption.TopDirectoryOnly))
            {
              // ...\Debug\net8.0\LOG-BACKUP\2025\01
              if (int.TryParse(m.Split('\\').Last(), out var month))
              {
                if (month >= minDate.Month)
                {
                  foreach (var path in Directory.GetFiles(m))
                  {
                    // ...\Debug\net8.0\LOG-BACKUP\2025\01\client_sim-2025-01-22.txt
                    var name = Path.GetFileNameWithoutExtension(path);  // client_sim-2025-01-22
                    var date = name.Substring(name.Length - date_str_len);  // 2025-01-22
                    if (DateTime.Parse(date) < minDate)
                    {
                      File.Delete(path);
                    }
                  }
                }
                else
                {
                  FileHelper.RemoveDirectory(m);
                }
              }
              else
              {
                FileHelper.RemoveDirectory(m);
              }
            }
          }
          else
          {
            FileHelper.RemoveDirectory(top);
          }
        }
        else
        {
          FileHelper.RemoveDirectory(top);
        }
      }
    }
    void MidnightOccured()
    {
      DateTime today = DateTime.Now.Date;  // 오늘이 5일이면 5일 00:00:00

      if (today.Day == _today)
      {
        today = today.AddDays(1);
      }

      this.DayChanged?.Invoke(this, this.GetBackupFilePath(today.AddDays(-1)));
      _today = today.Day;

      this.StartMidnightTimer();
    }
    TimeSpan TimeLeftUntillMidnight()
    {
      var now = DateTime.Now;
      // 자정까지 남은 시간
      return (now.Date.AddDays(1).AddSeconds(1) - now);
    }
    string GetDateFilename(DateTime d)
    {
      return $"{Filename}-{d.ToString("yyyy-MM-dd")}.{Extension}";
    }
    string GetBackupFilePath(DateTime d)
    {
      string path = Path.Combine(LogBackupDir, d.Year.ToString(), d.Month.ToString("D2"));
      Directory.CreateDirectory(path);
      return Path.Combine(path, this.GetDateFilename(d));
    }
  }
}