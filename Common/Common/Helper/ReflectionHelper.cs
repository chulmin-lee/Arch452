using System;
using System.Linq;

namespace Common
{
  public static class ReflectionHelper
  {
    public static T CastTo<T>(this object m) where T : class
    {
      if (m == null)
        throw new ArgumentNullException($"object is null");

      var o = m as T;
      if (o == null)
      {
        var src = m.GetType().Name;
        var dest = typeof(T).Name;
        throw new ArgumentNullException($"{src} convert to {dest} failed");
      }
      return o;
    }
    static Type[] _types = new[] { typeof(String), typeof(Decimal), typeof(DateTime),
               typeof(DateTimeOffset), typeof(TimeSpan), typeof(Guid)};
    public static bool IsSimpleType(this Type type)
    {
      return type.IsValueType || type.IsPrimitive || type.IsArray ||
             _types.Contains(type) || (Convert.GetTypeCode(type) != TypeCode.Object);
    }

    public static bool Copy<T>(T origin, T dest) where T : class
    {
      if (ReferenceEquals(origin, dest)) return true;
      if ((origin == null) || (dest == null)) return false;

      foreach (var pi in origin.GetType().GetProperties())
      {
        var pi2 = dest.GetType().GetProperty(pi.Name);
        if (pi2 == null) return false;
        if (pi.CanWrite && pi.PropertyType.IsSimpleType())
        {
          var value = pi.GetValue(origin, null);
          pi2.SetValue(dest, value);
        }
      }
      return true;
    }
  }
}