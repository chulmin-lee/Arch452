namespace Framework.Network
{
  public class ClientConnectionOption
  {
    public bool RetryConnecting;  // 연결될때까지 접속 시도
    public int ConnectingInterval;  // 연결 실패시 재시도 간격
    public bool RecoverConnectionLost; // 원격 연결이 끊어졌을때 재접속 시도 여부
    public int RecoverInterval; // 연결이 끊어지고 접속 시도 까지 지연 시간
  }
}