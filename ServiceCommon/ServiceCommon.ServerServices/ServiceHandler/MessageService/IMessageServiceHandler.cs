using System.Collections.Generic;

namespace ServiceCommon.ServerServices
{
  /// <summary>
  /// Session에게 특정 서비스 제공 (예. ticket 서비스)
  /// - 제공할수있는 서비스 목록
  /// - 서비스 데이타를 가지고 있는 MessageLoader 관리
  /// </summary>
  public interface IMessageServiceHandler
  {
    void Start();
    void Stop();

    // 서비스 제공
    List<SERVICE_ID> SupportMessages { get; }
    ServiceMessage RequestService(ServiceMessage request);
    void RequestService(IServerSession session, ServiceMessage request);

    // Session 관리
    List<PACKAGE> SupportPackages { get; }
    bool Subscribe(IServerSession session);
    bool Unsubscribe(IServerSession session);
  }
}