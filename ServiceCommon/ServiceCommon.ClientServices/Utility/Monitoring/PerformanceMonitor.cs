using Common;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ServiceCommon.ClientServices
{
  public class PERFORM_DATA
  {
    public int CpuUsage { get; set; }
    public int MemoryUsage { get; set; }
    public int CpuTemperature { get; set; }
    public int HddFreeSpace { get; set; }
  }

  public class PerformanceMonitor
  {
    PerformanceCounter cpuCounter;
    PerformanceCounter memoryCounter;
    public PerformanceMonitor()
    {
      // performancecounter 초기화 시간이 길다.
      Task.Run(() =>
      {
        try
        {
          // 전체 CPU Usage
          cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }
        catch (Exception ex)
        {
          LOG.ec($"create Cpu counter fail: {ex.Message}");
        }

        try
        {
          // 현재 프로그램의 Memory Usage
          string processName = Process.GetCurrentProcess().ProcessName;
          memoryCounter = new PerformanceCounter("Process", "Working Set - Private", processName);
        }
        catch (Exception ex)
        {
          LOG.ec($"create memory counter fail: {ex.Message}");
        }
      });
    }

    public PERFORM_DATA ScheduleAction()
    {
      var d = new PERFORM_DATA();
      d.CpuUsage = (int)(cpuCounter?.NextValue() ?? 0);
      long freemem = (long)(memoryCounter?.NextValue() ?? 0);

      if (freemem > 0)
      {
        // MB로 변환
        d.MemoryUsage = freemem.ToUnitSize(UnitConverter.SizeUnits.MB);
      }
      else
      {
        d.MemoryUsage = -1;
      }

      long freehdd = SystemHelper.GetHddFreespace();
      if (freehdd > 0)
      {
        // GB로 변환
        d.HddFreeSpace = freehdd.ToUnitSize(UnitConverter.SizeUnits.GB);
      }
      else
      {
        d.HddFreeSpace = -1;
      }

      return d;
    }
  }
}