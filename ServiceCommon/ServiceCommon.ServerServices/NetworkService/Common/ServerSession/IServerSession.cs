using Framework.Network;

namespace ServiceCommon.ServerServices
{
  public interface IServerSession : INetworkSession<ServiceMessage>
  {
    int SessionId { get; }
    int Key { get; }
    bool UseService { get; }
    PACKAGE Package { get; }
    PackageInfo PackageInfo { get; }
    CLIENT_INFO ClientInfo { get; }
  }
}