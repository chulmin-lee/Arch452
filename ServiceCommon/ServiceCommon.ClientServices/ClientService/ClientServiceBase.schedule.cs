using Common;
using System;

namespace ServiceCommon.ClientServices
{
  public abstract partial class ClientServiceBase : IClientService
  {
    PerformanceMonitor _perform = new PerformanceMonitor();
    void schedule_clear()
    {
      this.SCHEDULER.RemoveAll();
    }
    void change_content_view(IPackageViewConfig config)
    {
      this.CurrentPackage = config;
      if (this.ClientStatus.IsScreenOn != config.IsScreenOn)
      {
        this.ClientStatus.IsScreenOn = config.IsScreenOn;
        report_client_status();
      }
      this.ClientView.ConfigChanged(config);
    }

    void schedule_package()
    {
      SCHEDULER.Remove(CLIENT_SCHEDULE.PACKAGE_SCHEDULE);

      //GuardAgainst.Null(this.PackageConfig);
      this.PackageConfig.IsNull();

      var s = PackageConfig.GetCurrentSchedule();

      var config = this.create_package_view(s);

      SCHEDULER.RunOnceAt(schedule_package, CLIENT_SCHEDULE.PACKAGE_SCHEDULE, s.EndTime).Initialize();

      var cur_package = this.CurrentPackage?.Package ?? PACKAGE.NONE;

      if (config.Package == PACKAGE.NO_SCHEDULE && cur_package != PACKAGE.UPDATER)
      {
        var m = new ScreenOnOffMessage(false);
        this.ClientView.ReceiveMessage(m);
        ClientStatus.IsScreenOn = false;
        this.report_client_status();
      }
      else
      {
        this.change_content_view(config);
        ClientStatus.IsScreenOn = config.IsScreenOn;
        SessionHandler.Connect(new CLIENT_REGISTER_REQ(this.ClientInfo, config.PackageInfo));
      }
    }
    void schedule_initialize()
    {
      this.SCHEDULER.RunAtTime(reboot_schedule, CLIENT_SCHEDULE.REBOOT_SYSTEM, this.get_reboot_time(this.ServerConfig.ClientID))
                .RunEveryMinute(report_client_status, CLIENT_SCHEDULE.REPORT, 1)
                //.RunEveryMinute(check_server_connection, SCHEDULE.SERVER_CONNECTION, 1)
                .Initialize();
    }

    #region Schedule
    void reboot_schedule()
    {
      if (CurrentPackage?.CanReboot ?? false)
      {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
        {
          LOG.wc("reboot system");
          this.reboot();
        }
        else
        {
          LOG.wc("restart system");
          this.restart();
        }
      }
    }
    TimeSpan get_reboot_time(int id)
    {
      // 랜덤을 사용하면 안된다.
      // 재시작후 또 재시작할 수 있다.
      var minute = (id % 100) / 3; // 0~99 이므로 3으로 나눈다
      return new TimeSpan(3, minute, 0);
    }
    void report_client_status()
    {
      if (CurrentPackage?.CanReboot ?? false)
      {
        LOG.wc("client status");

        var o = _perform.ScheduleAction();
        ClientStatus.FreeHDDSpace = o.HddFreeSpace;
        ClientStatus.MemoryUsage = o.MemoryUsage;
        ClientStatus.CpuUsagePercent = o.CpuUsage;
        ClientStatus.CpuTemperature = o.CpuTemperature;
        SessionHandler.Send(ClientStatus);
      }
    }
    #endregion Schedule
  }

  public enum CLIENT_SCHEDULE
  {
    CLEAR,
    PERFORMANCE,
    REBOOT_SYSTEM,
    REPORT,
    PACKAGE_SCHEDULE,
    SERVER_CONNECTION,
    SCREEN_SCHEDULE,
    SCREEN_ON,
    SCREEN_OFF,
  }
}