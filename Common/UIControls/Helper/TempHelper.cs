using System;
using System.Collections.Generic;

namespace UIControls
{
  static class TempHelper
  {
    public static bool AreEquals<T>(this T value, T o) where T : struct, IConvertible
    {
      return EqualityComparer<T>.Default.Equals(value, o);
    }
    public static bool IsDefaultValue<T>(this T value) where T : struct, IConvertible
    {
      return EqualityComparer<T>.Default.Equals(value, default(T));
    }
    public static bool IsNotDefaultValue<T>(this T value) where T : struct, IConvertible
    {
      return !value.IsDefaultValue();
    }
  }
}