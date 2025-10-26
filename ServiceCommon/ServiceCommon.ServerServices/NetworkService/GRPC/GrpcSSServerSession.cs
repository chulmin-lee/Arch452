/*
using Framework.Network.GRPC;
using Grpc.Core;
using HospitalServices;
using ServiceCommon.GrpcService;

namespace ServiceCommon.ServerServices;

public class GrpcSSServerSession : ServerSessionBase
{
  public GrpcSSServerSession(IServerSessionOwner owner,
                             IServerStreamWriter<SERVER_MESSAGE> response,
                             ServerCallContext context,
                             int key, CLIENT_REGISTER_REQ config)
    : base(key, owner)
  {
    this.PackageInfo = config.PackageInfo;
    this.ClientInfo = config.ClientInfo;
    this.Connection = new GrpcSSServerConnection<ServiceMessage, SERVER_MESSAGE, CLIENT_MESSAGE>
      (this, response, context, GrpcMessageConverter.ServerConverter);
  }
}
*/