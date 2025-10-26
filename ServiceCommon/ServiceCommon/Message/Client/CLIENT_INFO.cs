namespace ServiceCommon
{
  public class CLIENT_INFO
  {
    public string IPAddress { get; set; } = string.Empty; // 물리적 고유ID, 보통 IP
    public string MacAddress { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;   // client version
    public string HospitalCode { get; set; } = string.Empty;
    public int ClientId { get; set; }  // 논리적 고유 ID, 보통 CLientId
  }
}