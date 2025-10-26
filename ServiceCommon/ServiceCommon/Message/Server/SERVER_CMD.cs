namespace ServiceCommon
{
  public class SERVER_CMD_NOTI : ServiceMessage
  {
    public SERVER_COMMAND Command { get; set; }
    public SERVER_CMD_NOTI() : base(SERVICE_ID.SVR_COMMAND) { }
    public SERVER_CMD_NOTI(SERVER_COMMAND d) : this()
    {
      this.Command = d;
    }

    public static ServiceMessage ClientUpdate = new SERVER_CMD_NOTI(SERVER_COMMAND.ClientUpdated);
    public static ServiceMessage ClientRestart = new SERVER_CMD_NOTI(SERVER_COMMAND.Restart);
    public static ServiceMessage ClientShutdown = new SERVER_CMD_NOTI(SERVER_COMMAND.Shutdown);
    public static ServiceMessage ClientReboot = new SERVER_CMD_NOTI(SERVER_COMMAND.Reboot);
    public static ServiceMessage ClientConfigUpdate = new SERVER_CMD_NOTI(SERVER_COMMAND.ConfigUpdated);
    public static ServiceMessage ClientConfigForcedUpdate = new SERVER_CMD_NOTI(SERVER_COMMAND.ClientForcedUpdate);
    public static ServiceMessage ClientRollback = new SERVER_CMD_NOTI(SERVER_COMMAND.Rollback);
    public static ServiceMessage ClientReloadData = new SERVER_CMD_NOTI(SERVER_COMMAND.Reload);

    public enum SERVER_COMMAND
    {
      None = 0,
      ConfigUpdated = 1,
      ClientUpdated = 2,
      ClientForcedUpdate = 3,
      Reload = 4,
      Rollback = 5,
      ScreenOn = 8,
      ScreenOff = 9,

      Restart = 20,
      Reboot = 21,
      Shutdown = 22,
    }
  }
}