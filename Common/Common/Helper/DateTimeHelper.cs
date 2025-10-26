using System;

namespace Common
{
  public static class DateTimeHelper
  {
    static string [] _date_formats = { "yyyyMMdd", "yyyy-MM-dd" };

    public static DateTime ToDate(this string date)
    {
      if (DateTime.TryParseExact(date.Trim(), _date_formats, System.Globalization.CultureInfo.InvariantCulture,
                                 System.Globalization.DateTimeStyles.None, out var result))
      {
        return result;
      }
      return DateTime.MinValue;
    }
    /// <summary>
    /// 현재 날짜를 이용하여 파일이름 생성
    /// </summary>
    /// <param name="ext">확장자</param>
    public static string ToFileName(string ext)
    {
      return DateTime.Now.ToFileName(ext);
    }
    public static string ToFileName(this DateTime d, string ext)
    {
      return $"{d.Year}.{d.Month}.{d.Day}-{d.Hour}.{d.Minute}.{d.Second}.{ext}";
    }
    /// <summary>
    /// 현재 날짜를 이용하여 파일 경로 생성
    /// </summary>
    /// <returns></returns>
    public static string ToFilePath() => DateTime.Now.ToFilePath();
    public static string ToFilePath(this DateTime d)
    {
      return $"{d.Year}.{d.Month}.{d.Day}-{d.Hour}.{d.Minute}.{d.Second}";
    }
    /// <summary>
    /// 하루를 기준으로 변환하는데 사용한다. 그러므로 하루의 마지막은 24:00:00이다.
    /// "24:00" 문제 때문에 TryParse를 사용하지 않고 직접 파싱한다.
    /// "24:00:00"은 변환하지 않는다. new TimeSpan(24,0,0)과 같이 다르다
    /// </summary>
    public static TimeSpan ToTimeSpan(this string s, bool is_end_time = false)
    {
      if (!string.IsNullOrWhiteSpace(s))
      {
        // 주의사항
        // - 문자열 앞뒤에 공백이 들어가면 파싱에 실패한다
        // - "24:00"은 파싱에 실퍠한다.
        // - 시작 시간에 24:00:00이 오면 0시로 처리
        // - 끝 시간에 00:00:00이 오면 24시로 처리

        int days = 0;
        int hours = 0;
        int minutes = 0;
        int seconds = 0;

        s = s.Trim();

        // check days
        var arr = s.Split('.');
        if (arr.Length > 1)
        {
          // d 또는 D가 붙은 경우 제거
          var days_str = arr[0].Trim().ToLower().Replace("d","");
          int.TryParse(days_str, out days);
          s = arr[1].Trim();
        }

        arr = s.Split(':');
        if (arr.Length >= 1)
        {
          int.TryParse(arr[0], out hours);
        }

        if (arr.Length >= 2)
        {
          int.TryParse(arr[1], out minutes);
        }

        if (arr.Length >= 3)
        {
          int.TryParse(arr[2], out seconds);
        }

        if (is_end_time)
        {
          if (hours == 0 && minutes == 0 && seconds == 0)
          {
            hours = 24;
          }
        }
        else
        {
          // start time
          if (hours == 24 && minutes == 0 && seconds == 0)
          {
            hours = 0;
          }
        }

        return new TimeSpan(days, hours, minutes, seconds);
      }
      return TimeSpan.Zero;
    }
  }
}