using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
  // Instance mode로 사용할때는 Stop()을 호출해줘야 한다.

  public partial class Logger
  {
    LogFileManager FM;
    object LOCK = new object();
    ProducerMonitor<string> _sendChannel;
    public Logger(string filename = "log.txt", string dir = "", LOG_LEVEL loglevel = LOG_LEVEL.DEBUG)
    {
      this.MinLogLevel = loglevel;
      this.LogLevelInitialize();

      _sendChannel = new ProducerMonitor<string>(write_log_proc);

      this.FM = new LogFileManager(filename, dir);
      this.FM.DayChanged += (s, e) => { lock (LOCK) FM.CurrentLogBackup(e); };

      this.Enqueue($"--------- logging started ({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")})---------");
    }
    void write_log_proc(string s)
    {
      using (StreamWriter sw = File.AppendText(FM.LogFilePath))
      {
        sw.WriteLine(s);
      }
    }
    /// <summary>
    /// 외부에서 호출하는 경우 이미 caller에 대한 처리를 마친 상태이다.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Enqueue(string s)
    {
      _sendChannel.Write(s);
    }
    public void Enqueue(string s, LOG_LEVEL level)
    {
      Enqueue(string.Format("{0} [{1}][{2}] {3}",
                              DateTime.Now.ToString("HH:mm:ss.fff"),
                              LEVEL[(int)level],
                              Thread.CurrentThread.ManagedThreadId,
                              s));
    }
    public void StopLogging()
    {
      Trace.WriteLine("stop logging");
      _sendChannel.Close();
    }
  }
}