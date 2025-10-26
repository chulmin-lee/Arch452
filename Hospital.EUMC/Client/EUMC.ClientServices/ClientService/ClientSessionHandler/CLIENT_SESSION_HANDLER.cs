using Framework.Network;
using Framework.Network.TCP;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public static class CLIENT_SESSION_HANDLER
  {
    //public static IClientSessionHandler Grpc(IClientService owner, ServerConfig o)
    //{
    //  var address = $"http://{o.ApiServerIP}:{o.ApiServerPort}";
    //  return Grpc(owner, address);
    //}
    //public static IClientSessionHandler Grpc(IClientService owner, string address)
    //{
    //  return new SHD_GrpcSSClientSessionHandler(owner, address);
    //}
    public static IClientSessionHandler Tcp(IClientService owner, ServerConfig o)
    {
      return Tcp(owner, o.ServerIP, o.ServerPort);
    }
    public static IClientSessionHandler Tcp(IClientService owner, string ip, int port)
    {
      MemoryPoolHelper.Initialize(2048, 100);
      SaeaPoolHelper.Initialize(2048, 100);
      return new TcpClientSessionHandler(owner, ip, port);
    }
  }
}