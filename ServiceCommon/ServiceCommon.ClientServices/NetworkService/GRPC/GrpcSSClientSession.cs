/*
using Common;
using Framework.Network;
using Framework.Network.GRPC;
using Grpc.Core;
using Grpc.Net.Client;
using HospitalServices;
using ServiceCommon.GrpcService;

namespace ServiceCommon.ClientServices;

public class GrpcSSClientSession : ClientSessionBase, IClientSession
{
  bool use_cancel = false;
  string _address;
  ServiceMessage? RegisterMessage;
  public GrpcSSClientSession(IClientSessionOwner owner, string address)
    : base(owner)
  {
    _address = address;
  }
  public override void Connect(ServiceMessage m)
  {
    LOG.dc(m.ServiceId);
    this.RegisterMessage = m;
    this.Connect();
  }
  public override void Send(ServiceMessage m)
  {
    LOG.dc(m.ServiceId);
    Task.Factory.StartNew(() =>
    {
      // blocking 방지
      var converter = GrpcMessageConverter.ClientConverter;
      var s = converter.SendMessage(m);
      var r = new HospitalService.HospitalServiceClient(GetChannel()).UnaryService(s);
      if (r != null)
      {
        var message = converter.ReadMessage(r);
        if (message != null)
        {
          this.OnMessageReceived(message);
        }
      }
    });
  }

  protected override INetworkConnection<ServiceMessage> CreateClientConnection()
  {
    var req = this.RegisterMessage.CastTo<CLIENT_REGISTER_REQ>();
    if (req == null) throw new ServiceException("info is null");

    var cts = new CancellationTokenSource();
    var reg = req.ToGrpc();

    AsyncServerStreamingCall<SERVER_MESSAGE> call;
    if (use_cancel)
    {
      call = new HospitalService.HospitalServiceClient(this.GetChannel())
                  .ServerStreamService(reg, new CallOptions(cancellationToken: cts.Token));
    }
    else
    {
      call = new HospitalService.HospitalServiceClient(this.GetChannel())
                  .ServerStreamService(reg);
    }

    return new GrpcSSClientConnection<ServiceMessage, CLIENT_MESSAGE, SERVER_MESSAGE>
                   (this, call, GrpcMessageConverter.ClientConverter);
  }
  /// <summary>
  /// 한번 생성한 채널은 서버가 재시작하지 않는 한 재활용 가능하다.
  /// 그러므로 항상 재 생성한다.
  /// </summary>
  /// <returns></returns>
  GrpcChannel GetChannel() => GrpcChannel.ForAddress(_address);
}
*/