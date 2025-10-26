using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
  public static class StringExtension
  {
    static string  [] newlines = new string[] { "\r\n", "\r", "\n" };
    public static string[] ToLines(this string s)
    {
      return s?.Split(newlines, StringSplitOptions.None) ?? new string[] { };
    }
    public static bool IsMultilines(this string s)
    {
      var list = ToLines(s);
      return list != null ? list.Length > 1 : false;
    }
    public static List<string> ToList(this string s, params char[] seperator)
    {
      var list = new List<string>();
      if (!string.IsNullOrWhiteSpace(s))
      {
        if (seperator.Length == 0)
        {
          seperator = new char[] { ',' };
        }

        foreach (var p in s.Split(seperator, StringSplitOptions.RemoveEmptyEntries))
        {
          if (!string.IsNullOrWhiteSpace(p))
          {
            list.Add(p.Trim());
          }
        }
      }
      return list;
    }
    public static List<int> ToIntList(this string s, params char[] seperator)
    {
      if (seperator.Length == 0)
      {
        seperator = new char[] { ',' };
      }
      var list = new List<int>();
      if (!string.IsNullOrWhiteSpace(s))
      {
        foreach (var p in s.Split(seperator, StringSplitOptions.RemoveEmptyEntries))
        {
          if (int.TryParse(p, out int v))
          {
            list.Add(v);
          }
        }
      }
      return list;
    }
    public static int ToInt(this string s, int defaultValue = 0)
    {
      if (int.TryParse(s, out int v))
      {
        return v;
      }
      return defaultValue;
    }
    public static double ToDouble(this string s, double defaultValue = 0)
    {
      if (double.TryParse(s, out double v))
      {
        return v;
      }
      return defaultValue;
    }
    static string [] yes =  {"Y", "TRUE", "YES", "1"};
    public static bool ToBoolean(this string s, bool default_value = false)
    {
      if (!string.IsNullOrWhiteSpace(s))
      {
        return yes.Contains(s.ToUpper());
      }
      return default_value;
    }
    public static string ToTimeString(this int time)
    {
      if (time == 0)
      {
        return string.Empty;
      }
      else
      {
        var hour = (time / 100).ToString().PadLeft(2, '0');
        var min = (time % 100).ToString().PadLeft(2, '0');
        return $"{hour}:{min}";
      }
    }
    public static string ToTimeString(this string time)
    {
      if (int.TryParse(time, out int v))
      {
        return v.ToTimeString();
      }
      return string.Empty;
    }
    public static bool IsString(this string s) => !string.IsNullOrWhiteSpace(s);
    public static bool IsKorean(this string s) => !string.IsNullOrWhiteSpace(s) ? is_korean(s[0]) : false;
    public static bool IsEnglish(this string s) => !string.IsNullOrWhiteSpace(s) ? is_english(s[0]) : false;

    public static bool is_korean(this char ch) => ((0xAC00 <= ch && ch <= 0xD7A3) || (0x3131 <= ch && ch <= 0x318E)) ? true : false;
    public static bool is_numeric(this char ch) => (0x30 <= ch && ch <= 0x39) ? true : false;
    public static bool is_english(this char ch) => ((0x61 <= ch && ch <= 0x7A) || (0x41 <= ch && ch <= 0x5A)) ? true : false;

    public static string MaskedName(this string name, int max_length = 7)
    {
      string masked = name.ToString();

      if (string.IsNullOrEmpty(masked) || masked.Contains("*"))
      {
        return masked;
      }

      masked = masked.Trim();

      if (masked.IsKorean())
      {
        return masked.EndsWith("아기") ? $"{mask_center(masked.Substring(0, masked.Length - 2))} 아기"
                                      : mask_center(masked);
      }
      else
      {
        masked = masked.Split(' ')[0];
        int len = Math.Max(Math.Min(masked.Length - 1, max_length), 1);
        return masked.Substring(0, len) + "*";
      }
    }
    public static string Maske_PT_NO(this string s)
    {
      s = s.ToString().Trim();

      if (string.IsNullOrEmpty(s) || s.Contains("*"))
      {
        return s;
      }

      var masked = (s.Length > 3) ? s.Substring(0, s.Length-2) : s.Substring(0, s.Length-1);
      int starCount = s.Length - masked.Length;
      return $"{masked}{new string('*', starCount)}";
    }
    /// <summary>
    /// 이름 중간에 *를 넣는다.
    /// 가나 => 가나*, 가나다 => 가*다, 가나다라마바사 => 가*****사
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    static string mask_center(string s)
    {
      s = s.Trim();
      var s1 = s.First().ToString();
      string s2 = s.Length >=3 ? s.Last().ToString() : "";
      int starCount = Math.Max(s.Length-2, 1);
      return $"{s1}{new string('*', starCount)}{s2}";
    }
    /// <summary>
    /// 이름뒤에 *를 넣는다.
    /// 가 => 가*, 가나 => 가*, 가나다 => 가나*, 가나다라마바사 => 가나*****
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    static string mask_last(string s)
    {
      s = s.Trim();
      string masked = (s.Length >=3) ? s.Substring(0, 2) : s.Substring(0, 1);
      int starCount = Math.Max(s.Length-2, 1);
      return $"{masked}{new string('*', starCount)}";
    }
    public static string MaskedName(this object originNm)
    {
      string maskedNm = originNm.ToString();

      if (string.IsNullOrEmpty(maskedNm) || maskedNm.Contains("*"))
        return maskedNm;

      try
      {
        if (maskedNm.Length == 1)//?
          return maskedNm + "*";
        else
        {
          int index = maskedNm.IndexOf("아기");
          if (index > 0)
          {
            if (maskedNm.Length == 4)   // 외자이름
              return maskedNm.Substring(0, 2) + "*아기";
            else
              return maskedNm.Substring(0, index - 1) + "*아기";
          }

          // 외국인 영문은?????
          return maskedNm.Substring(0, maskedNm.Length - 1) + "*";
        }
      }
      catch { }

      return maskedNm;
    }

    /// <summary>
    /// 지정된 구분자로 문자열을 분할한다. null string이면 크기 0 배열 리턴
    /// </summary>
    /// <returns></returns>
    public static string[] ToArray(this string s, char delimeter)
    {
      if (!string.IsNullOrWhiteSpace(s))
      {
        return s.Split(new char[] { delimeter }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
      }
      return new string[0];
    }
  }
}