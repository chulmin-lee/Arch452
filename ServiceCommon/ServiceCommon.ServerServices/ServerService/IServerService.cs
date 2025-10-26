namespace ServiceCommon.ServerServices
{
  /// <summary>
  /// Session들과 IMessageLoaderHandler 사이의 중재
  /// event broker는 아니다. (초기 연결자 역할)
  /// - message loader 핸들러 생성
  /// - 가입 session을 적절한 message loader handler에 연결
  /// - 데이타/요청 분배
  /// </summary>

  public interface IServerService
  {
    void Start();
    void Stop();
    ServiceMessage RequestService(ServiceMessage request);
    void RequestService(IServerSession session, ServiceMessage request);
    /// <summary>
    /// session을 등록한다.
    /// </summary>
    /// <param name="session"></param>
    /// <returns>등록 서비스가 없는 경우 false</returns>
    bool SessionConnected(IServerSession session);
    /// <summary>
    /// session 등록 해제
    /// </summary>
    /// <param name="session"></param>
    /// <returns>등록 서비스가 없는 경우</returns>
    bool SessionDisconnected(IServerSession session);
  }
}