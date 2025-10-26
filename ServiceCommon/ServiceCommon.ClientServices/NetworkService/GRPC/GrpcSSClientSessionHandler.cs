/*
using Common;
using Framework.Network;

namespace ServiceCommon.ClientServices;
public abstract class GrpcSSClientSessionHandler : ClientSessionHandlerBase, IClientSessionHandler
{
  public GrpcSSClientSessionHandler(IClientService owner, string address)
    : base(owner)
  {
    this.Session = new GrpcSSClientSession(this, address);
  }

  protected PackageInfo? PackageInfo { get; set; }
  public override void Connect(ServiceMessage m)
  {
    this.PackageInfo = m.CastTo<CLIENT_REGISTER_REQ>()?.PackageInfo;
    // GRPC인 경우 끊고 바로 연결하면 오류가 발생하기 쉽다
    // CloseImp()에서 끊어지는것을 감지해야한다
    this.Close();
    base.Connect(m);
  }
  public override void OnStateChanged(NetworkState state)
  {
    if (state == NetworkState.Online)
    {
      this.RequestInitData();
    }
    base.OnStateChanged(state);
  }
  protected abstract void RequestInitData();
}
*/