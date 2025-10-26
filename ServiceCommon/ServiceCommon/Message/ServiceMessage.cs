namespace ServiceCommon
{
  public class ServiceMessage
  {
    public MessageErrorCode MessageErrorCode { get; set; } = MessageErrorCode.NoResponse;
    public SERVICE_ID ServiceId { get; set; } = SERVICE_ID.NONE;
    public ServiceMessage() { }
    public ServiceMessage(SERVICE_ID s)
    {
      this.MessageErrorCode = MessageErrorCode.success;
      this.ServiceId = s;
    }
    public ServiceMessage(SERVICE_ID s, MessageErrorCode code)
    {
      this.MessageErrorCode = code;
      this.ServiceId = s;
    }
    public static ServiceMessage None = new ServiceMessage(SERVICE_ID.NONE, MessageErrorCode.NoResponse);
    public static ServiceMessage Error(SERVICE_ID id, MessageErrorCode code) => new ServiceMessage(id, code);

    public override string ToString()
    {
      return $"ServiceMessage.{this.ServiceId}";
    }
  }

  public enum MessageErrorCode
  {
    success = 0,
    NoResponse,
    Error,
    Unavailable
  }
}