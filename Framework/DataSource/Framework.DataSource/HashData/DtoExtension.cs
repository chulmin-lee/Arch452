using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.DataSource
{
  public static class DtoExtension
  {
    static Type _modelAttribute = typeof(OriginAttribute);
    static Type _stringType = typeof(string);

    public static bool IsUniqueKeys<T>(this IEnumerable<T> news) where T : OriginDataModel
    {
      var hashcodes = news.Select(x => x.HashCode);
      return hashcodes.Count() == hashcodes.Distinct().Count();
    }

    public static UpdateData<T> CheckUpdate<T>(this Dictionary<int, T> dic, List<T> news, ModelEqualityComparer<T> compare)
      where T : OriginDataModel
    {
      //if (!news.IsUniqueKeys())
      //{
      //  LOG.e($"has duplicate key");
      //  var news_dic = new Dictionary<int, T>();
      //  foreach (var p in news)
      //  {
      //    if (!news_dic.ContainsKey(p.ID))
      //    {
      //      news_dic.Add(p.ID, p);
      //    }
      //    else
      //    {
      //      log.e($"{this.PUBLISHER} dup remove: {p}");
      //    }
      //  }
      //  news = news_dic.Values.ToList();
      //}

      // 중복 제거
      if (news.Count != news.Select(x => x.HashCode).Distinct().Count())
      {
        news = news.Distinct(compare).ToList();
      }
      var origin = dic.Values;

      var d = origin.Except(news, compare).ToList();
      var a = news.Except(origin, compare).ToList();

      // UpdatedData<T> 생성
      var o = new UpdateData<T>()
      {
        Deleted = origin.Except(news, compare).ToList(),
        Added = news.Except(origin, compare).ToList(),
      }.Compose();

      // 조회 데이타 중 같은 id를 가진 데이타들
      foreach (var new_data in news.Intersect(origin, compare))
      {
        if (new_data.IsEqualModel(dic[new_data.HashCode]))
        {
          o.Constant.Add(new_data);
        }
        else
        {
          o.Updated.Add(new_data);
        }
      }
      o.Compose();

      if (!o.IsValid())
        throw new Exception("Updated error");

      return o;
    }
    static bool IsEqualModel<T>(this T origin, T another) where T : OriginDataModel
    {
      if (ReferenceEquals(origin, another)) return true;
      if ((origin == null) || (another == null)) return false;

      foreach (var pi in origin.GetType().GetProperties().Where(pi => Attribute.IsDefined(pi, _modelAttribute)))
      {
        var attr = pi.GetCustomAttributes(_modelAttribute, false).First() as OriginAttribute;

        if (attr == null || !attr.IsNormal) continue;

        var pi2 = another.GetType().GetProperty(pi.Name);
        if (pi2 == null) return false;

        var a = pi.GetValue(origin);
        var b = pi2.GetValue(another);

        if (pi.PropertyType == _stringType)
        {
          // 문자열인 경우 "", null 을 같게 처리하기 위해서
          if (!IsSameString(a as string, b as string))
          {
            return false;
          }
        }
        //else if (!a.Equals(b))
        //{
        //  return false;
        //}
        else
        {
          if (a == null ^ b == null) return false;
          if (a != null && b != null)
          {
            if (!a.Equals(b))
              return false;
          }
        }
      }
      return true;
    }
    public static bool IsSameString(string a, string b)
    {
      return string.IsNullOrWhiteSpace(a) ? string.IsNullOrWhiteSpace(b) : string.Equals(a?.Trim(), b?.Trim());
    }
  }

  public class ModelEqualityComparer<T> : IEqualityComparer<T> where T : OriginDataModel
  {
    public bool Equals(T x, T y)
    {
      if (x == null || y == null) return false;

      if (ReferenceEquals(x, y)) return true;
      return x.GetHashCode() == y.GetHashCode();
    }

    public int GetHashCode(T obj)
    {
      return obj?.GetHashCode() ?? 0;
    }
  }
}