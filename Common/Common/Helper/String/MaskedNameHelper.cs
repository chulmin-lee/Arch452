using System;
using System.Collections.Generic;

namespace Common
{
  public enum MaskPosition
  {
    Left, Middle, Right
  }
  public class MaskedNameHelper
  {
    MaskPosition _position = MaskPosition.Right;
    double _ratio = 0.5;
    int _maxLength = 80;
    List<string> _postfix = new List<string> { "아기" };
    public MaskedNameHelper(MaskPosition p, double ratio, int max, List<string> postfix)
    {
      _position = p;
      _ratio = ratio;
      _maxLength = max;
      _postfix = postfix;
    }
    public string MaskedName(string name)
    {
      name = name.Trim();
      if (string.IsNullOrEmpty(name) || name.Contains("*"))
        return name;

      string postfix = string.Empty;
      foreach (var p in _postfix)
      {
        if (name.Length <= p.Length) continue;
        if (name.Length - p.Length < 2) continue;
        if (name.EndsWith(p))
        {
          postfix = p;
          name = name.Replace(postfix, string.Empty).Trim();
          break;
        }
      }

      if(name.Length > _maxLength)
      {
        name = name.Substring(0, _maxLength).Trim();
      }

      var count = (int)Math.Ceiling(name.Length * _ratio);
      switch(_position)
      {
        case MaskPosition.Right:
          {
            name = name.Substring(0, count).Trim() + "*";
          } break;
        case MaskPosition.Left:
          {
            name = "*" + name.Substring(name.Length - count).Trim();
          } break;
        case MaskPosition.Middle:
          {
            int right = count / 2;
            int left = (right * 2 == count) ? right : right+1;
            var prev = name.Substring(0, left).Trim();
            var post = name.Substring(name.Length-right).Trim();
            name = $"{prev}*{post}";
          }
          break;
      }
      return $"{name}{postfix}";
    }
  }
}
