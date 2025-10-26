namespace ServiceCommon
{
  public class SERVER_STATUS_NOTI : ServiceMessage
  {
    public STATE State { get; set; } = STATE.ONLINE;
    public SERVER_STATUS_NOTI() : base(SERVICE_ID.SVR_STATUS) { }
    public SERVER_STATUS_NOTI(STATE s) : this()
    {
      this.State = s;
    }
    public enum STATE
    {
      NONE = 0,
      ONLINE,
      ERROR
    }
  }
}