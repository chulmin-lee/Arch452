/*
using Common;
using Grpc.Core;
using HospitalServices;
using ServiceCommon.GrpcService;

namespace ServiceCommon.ServerServices;
public class GrpcSSServerSessionHandler : ServerSessionHandlerBase, IGrpcServerSessionHandler
{
  GrpcAcceptor _acceptor;
  public GrpcSSServerSessionHandler(IServerService manager, int accept_port) : base(manager)
  {
    _acceptor = new GrpcAcceptor(this, accept_port);
  }

  public void ClientAccepted(IServerStreamWriter<SERVER_MESSAGE> responseStream, ServerCallContext context, CLIENT_REGISTER_REQ config)
  {
    var session = new GrpcSSServerSession(this, responseStream,context, this.NextKey, config);
    this.OnSessionConnected(session);
  }
  public ServiceMessage RequestService(ServiceMessage request)
  {
    return this.ServerService.RequestService(request);
  }
  public override void Start()
  {
    LOG.ic("start session handler");
    _acceptor.Start();
    base.Start();
  }

  public override void Stop()
  {
    LOG.wc("stop session handler");
    _acceptor?.Stop();
    base.Stop();
  }
}
*/