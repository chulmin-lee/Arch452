using Common;
using System;

namespace Framework.Network
{
  public class FrameworkException : Exception
  {
    public string ErrorMessage { get; set; } = string.Empty;
    public FrameworkException()
    {
    }
    public FrameworkException(string s) : base(s)
    {
      LOG.e($"FrameworkException: {s}");
      this.ErrorMessage = s;
    }
  }
}