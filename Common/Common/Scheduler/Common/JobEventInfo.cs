using System;

namespace Common
{
  public class JobStartInfo
  {
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
  }

  public class JobEndInfo
  {
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public TimeSpan Elapsed { get; set; }
    /// <summary>
    /// Date and time of next run.
    /// </summary>
    public DateTime NextRun { get; set; }
  }

  public class JobExceptionInfo
  {
    public string Name { get; set; } = string.Empty;
    public Exception Exception { get; set; }
  }
}