using System;
using System.Collections.Generic;
using System.IO;

namespace ServiceCommon.ClientServices
{
  public enum PLAYLIST_SOUND_TYPE { NONE, BELL, SPEECH, BELL_SPEECH }

  public class PlaylistSchedule
  {
    public string HospitalCode { get; set; } = "01";
    public int No { get; set; }
    public string Mode { get; set; } = string.Empty;
    public bool IsDefaultPackage { get; set; }

    public TimeSpan StartTime => this.TimeRange.Start;
    public TimeSpan EndTime => this.TimeRange.End;
    public bool IsAlwaysOn => this.TimeRange.IsAlwaysOn;
    public TimeRange TimeRange { get; set; } = new TimeRange();
    public DateRange DateRange { get; set; } = new DateRange();
    public WeekdayRange WeekdayRange { get; set; } = new WeekdayRange();
    public string PackageName { get; set; }
    public PlaylistMedical Medical { get; set; }
    public NoticeConfig NoticeConfig { get; set; } = new NoticeConfig();
    public int Duration { get; set; }  // 홍보 로테이션 시간
    public int PatientPerRoom { get; set; } // 대기자수
    public bool ShowDelayTime { get; set; }
    public TVSetting TVSetting { get; set; } = new TVSetting();

    public List<REMOTE_FILE> RemoteContents { get; set; } = new List<REMOTE_FILE>();
    public List<CONTENT_FILE> ContentFiles { get; set; } = new List<CONTENT_FILE>();
    public bool ContentUse { get; set; }
    public double MediaVolume { get; set; } // 0 ~ 1
    public PLAYLIST_SOUND_TYPE SoundType { get; set; } = PLAYLIST_SOUND_TYPE.NONE;
    //public string SoundType { get; set; } = string.Empty;
    public PACKAGE_ERROR PackageError { get; set; } = PACKAGE_ERROR.Success;
    public bool Success => this.PackageError == PACKAGE_ERROR.Success;

    public PlaylistSchedule(string package, TimeRange range, PlaylistMedical medical = null)
    {
      this.PackageName = package;
      this.TimeRange = range;
      this.Medical = medical;
    }
    public PlaylistSchedule(PACKAGE_ERROR error)
    {
      this.PackageError = error;
      this.PackageName = "er";
    }

    public bool CanRunning(DateTime d)
    {
      if (this.DateRange.IsAlwaysOn)
      {
        if (!this.WeekdayRange.IsInRange(d)) return false;
      }
      else
      {
        if (!this.DateRange.IsInRange(d.Date)) return false;
      }
      return this.TimeRange.IsInRange(d);
    }
    public bool CanTodayRunning(DateTime d)
    {
      if (this.DateRange.IsAlwaysOn)
      {
        if (!this.WeekdayRange.IsInRange(d)) return false;
      }
      else
      {
        if (!this.DateRange.IsInRange(d.Date)) return false;
      }
      return true;
    }
    public bool IsInRange(TimeSpan ts)
    {
      return this.TimeRange.IsInRange(ts);
    }
    /// <summary>
    /// 기간이 설정된 경우 실행 가능성이 있는가?
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public bool IsAvailableDate(DateTime d)
    {
      return this.DateRange.IsAvailable(d);
    }

    public void UpdateContentFile(string contentsDir)
    {
      this.ContentFiles.Clear();

      foreach (var file in this.RemoteContents)
      {
        if (!string.IsNullOrEmpty(file.FileName) && file.Size > 0)
        {
          var path = Path.Combine(contentsDir, file.FileName);
          if (File.Exists(path))
          {
            this.ContentFiles.Add(new CONTENT_FILE(path, file.Id));
          }
        }
      }
    }

    public override string ToString()
    {
      return $"{this.PackageName} : AlwaysOn: {this.IsAlwaysOn}";
    }
  }
  public class REMOTE_FILE
  {
    public string Path;  // 23/인생은예술이다.jpg
    public string FileName;  // 인생은예술이다.jpg
    public long Size;
    public int Id;

    public REMOTE_FILE(string path, long size, int id)
    {
      this.Path = path;
      this.FileName = System.IO.Path.GetFileName(path);
      this.Size = size;
      this.Id = id;
    }
  }
  public class CONTENT_FILE
  {
    public string ContentPath;
    public bool IsMedia;
    public bool IsImage;
    public int Id;
    public CONTENT_FILE(string path, int id)
    {
      this.ContentPath = path;
      this.Id = id;

      var ext = Path.GetExtension(path).ToLower();

      if (image.Contains(ext))
      {
        this.IsMedia = true;
        this.IsImage = true;
      }
      else if (video.Contains(ext))
      {
        this.IsMedia = true;
        this.IsImage = false;
      }
    }

    static List<string> image = new List<string> { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
    static List<string> video = new List<string> { ".avi", ".wmv", "divx", ".mp4", ".asf", ".3gp"};
  }

  public class TVSetting
  {
    public bool UseTV;
    public int TvChannelNo;
    public string TvChannelName = string.Empty;
  }

  public enum SOUND_TYPE { NONE, BEEP, SPEECH, }
}