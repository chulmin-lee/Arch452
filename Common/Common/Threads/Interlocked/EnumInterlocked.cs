using System;
using System.Collections.Generic;
using System.Threading;

namespace Common
{
  public class EnumInterlocked<T>
    where T : struct, Enum
  {
    public event Action<T> StateChanged;

    long _value = 0;
    Dictionary<long, T> ToEnum = new Dictionary<long, T>();
    Dictionary<T, long> ToValue = new Dictionary<T,long>();

    public EnumInterlocked(T intial_state)
    {
      foreach (int value in Enum.GetValues(typeof(T)))
      {
        T e = (T)(object)value;
        ToEnum.Add(value, e);
        ToValue.Add(e, value);
      }
      _value = ToValue[intial_state];
    }

    public T State => ToEnum[Interlocked.Read(ref _value)];
    public bool Set(T v)
    {
      if (Interlocked.Exchange(ref _value, ToValue[v]) != ToValue[v])
      {
        this.StateChanged?.Invoke(v);
        return true;
      }
      return false;
    }
    public bool IsSet(T v) => Interlocked.Read(ref _value) == ToValue[v];
  }
}