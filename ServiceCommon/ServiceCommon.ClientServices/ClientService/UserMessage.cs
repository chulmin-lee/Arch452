namespace ServiceCommon.ClientServices
{
  public enum USER_MESSAGE_TYPE
  {
    DOWNLOAD_PROGRESS = 0,
    SCREEN_ON_OFF = 1,
  }

  public class UserMessage : ServiceMessage
  {
    public USER_MESSAGE_TYPE Type { get; set; }
    public UserMessage(USER_MESSAGE_TYPE m) : base(SERVICE_ID.USER_MESSAGE)
    {
      this.Type = m;
    }
  }

  public class DownloadProgressMessage : UserMessage
  {
    /// <summary>
    /// 파일을 구분하기 위한 인덱스
    /// </summary>
    public long Index { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long Length { get; set; }
    public int Percent { get; set; }
    public string DownloadSpeed { get; set; } = string.Empty;
    public string DownloadSize { get; set; } = string.Empty;
    public string TimeLeft { get; set; } = string.Empty;
    public DownloadProgressMessage() : base(USER_MESSAGE_TYPE.DOWNLOAD_PROGRESS)
    {
    }
  }

  public class ScreenOnOffMessage : UserMessage
  {
    public bool ScreenOn { get; set; }
    public ScreenOnOffMessage(bool on) : base(USER_MESSAGE_TYPE.SCREEN_ON_OFF)
    {
      this.ScreenOn = on;
    }
  }
}