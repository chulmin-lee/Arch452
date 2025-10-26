using System.Threading;

namespace Common
{
  public class InterlockWrapper
  {
    private long _value = 0;

    public bool IsRunning => Interlocked.Read(ref _value) == 1;
    public bool Set()
    {
      return Interlocked.Exchange(ref _value, 1) == 0;
    }
    public bool Reset()
    {
      return Interlocked.Exchange(ref _value, 0) == 1;
    }
  }
}