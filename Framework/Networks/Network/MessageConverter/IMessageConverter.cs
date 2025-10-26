namespace Framework.Network
{
  /// <summary>
  /// message mapper
  /// </summary>
  /// <typeparam name="M">사용자 메시지</typeparam>
  /// <typeparam name="S">전송 Packet type</typeparam>
  /// <typeparam name="R">수신 Packet type</typeparam>
  public interface IMessageConverter<M, S, R>
    where M : class, new()
  {
    S SendMessage(M m);  // 사용자 메시지를 전송용 네트웍 타입으로 변환
    M ReadMessage(R m);  // 수신 네트웍 데이타를 사용자 메시지로 변환
    M ReadMessage();  // 수신된 사용자 메시지 가져오기 (분할/중첩 수신이 가능한 경우 처리용)

    /// <summary>
    /// 내부에 버퍼가 있는 경우 버퍼 정리
    /// - 연결될때, 연결이 끊어질때 초기화
    /// </summary>
    void Clear();
    void Close();
  }
}