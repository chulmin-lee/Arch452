using System;

namespace ServiceCommon
{
  public class CLIENT_REGISTER_REQ : ServiceMessage
  {
    public CLIENT_INFO ClientInfo { get; set; } = new CLIENT_INFO();
    public PackageInfo PackageInfo { get; set; } = new PackageInfo();

    public CLIENT_REGISTER_REQ() : base(SERVICE_ID.CLIENT_REGISTER) { }
    public CLIENT_REGISTER_REQ(CLIENT_INFO client, PackageInfo p) : this()
    {
      this.ClientInfo = client;
      this.PackageInfo = p;
    }
  }

  public class CLIENT_REGISTER_RESP : ServiceMessage
  {
    public ServerInformation Data { get; set; } = new ServerInformation();
    public CLIENT_REGISTER_RESP() : base(SERVICE_ID.CLIENT_REGISTER) { }
    public CLIENT_REGISTER_RESP(ServerInformation data) : this()
    {
      this.Data = data;
    }
  }

  public class ServerInformation
  {
    public string HSP_TP_CD { get; set; } = string.Empty;
    public DateTime ServerTime { get; set; } = DateTime.MinValue;
    public int MessageVersion { get; set; }

    public ServerInformation() { }
    public ServerInformation(string hspcd, int messageVersion)
    {
      this.HSP_TP_CD = hspcd;
      this.ServerTime = DateTime.Now;
      MessageVersion = messageVersion;
    }
  }
}