using Common;
using System;

namespace ServiceCommon
{
  public class ServiceException : Exception
  {
    public string ErrorMessage { get; set; } = string.Empty;
    public ServiceException()
    {
    }
    public ServiceException(string s) : base(s)
    {
      LOG.e($"ServiceException: {s}");
      this.ErrorMessage = s;
    }
  }
}