namespace ServiceCommon.ClientServices
{
  public class ServerConfig
  {
    public int ClientID { get; set; }
    public string ServerIP { get; set; } = string.Empty;   // IP 또는 URL
    public int ServerPort { get; set; }

    public string ApiServerIP { get; set; } = string.Empty;
    public int ApiServerPort { get; set; }

    public string HTTP_HOME { get; set; } = string.Empty;
    public int HTTP_PORT { get; set; } = 80;

    public string ProductName { get; set; } = string.Empty;
  }
}