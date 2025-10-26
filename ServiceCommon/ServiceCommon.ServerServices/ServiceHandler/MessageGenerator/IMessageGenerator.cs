namespace ServiceCommon.ServerServices
{
  /// <summary>
  /// 필요한 서비스 메시지를 생성한다
  /// </summary>
  public interface IMessageGenerator
  {
    void Start();
    void Stop();
    bool MessageSubscribe(IMessageSubscriber loader);
  }
}