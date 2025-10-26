using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Common
{
  public static class GuardAgainst
  {
    //public static void Null<T>([NotNull] T? value, [CallerArgumentExpression(parameterName: "value")] string? paramName = null)
    //{
    //  if (value == null) throw new ArgumentNullException(paramName);
    //}

    public static void IsNull(this object o)
    {
      if(o == null) throw new ArgumentNullException(o.GetType().Name);
    }
  }
}