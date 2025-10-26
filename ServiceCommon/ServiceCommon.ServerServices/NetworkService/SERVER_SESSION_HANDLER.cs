using Common;
using Framework.Network;
using Framework.Network.TCP;
using System.Threading;

namespace ServiceCommon.ServerServices
{
  /// <summary>
  /// Session 연결 관리,
  /// Session을 message loader handler에 등록하기 위해서 ServiceManager에게 session 연결/해제 통지
  /// </summary>

  public static class SERVER_SESSION_HANDLER
  {
    //public static IServerSessionHandler Grpc(IServerService manager, int port, bool use_status = true)
    //{
    //  return new GRPC_ServerSessionHandler(manager, port);
    //}
    public static IServerSessionHandler Tcp(IServerService manager, int port, bool use_status = true)
    {
      MemoryPoolHelper.Initialize(2048, 1000);
      SaeaPoolHelper.Initialize(2048, 1000);
      return new TCP_ServerSessionHandler(manager, port);
    }
  }

  //class GRPC_ServerSessionHandler : GrpcSSServerSessionHandler
  //{
  //  Timer? _timer;
  //  public GRPC_ServerSessionHandler(IServerService manager, int accept_port, bool use_status = true) : base(manager, accept_port)
  //  {
  //    if (use_status)
  //    {
  //      _timer = new Timer(ServerStatus);
  //    }
  //  }
  //  void ServerStatus(object? state)
  //  {
  //    LOG.dc("server status");
  //    var msg = new SERVER_STATUS_NOTI();
  //    this.SendAll(msg);
  //  }
  //  public override void Start()
  //  {
  //    _timer?.Change(1000, 1000 * 60);
  //    base.Start();
  //  }
  //  public override void Stop()
  //  {
  //    _timer?.Change(Timeout.Infinite, Timeout.Infinite);
  //    base.Stop();
  //  }
  //}

  class TCP_ServerSessionHandler : TcpServerSessionHandler
  {
    Timer _timer;
    public TCP_ServerSessionHandler(IServerService manager, int accept_port, bool use_status = true) : base(manager, accept_port)
    {
      if (use_status)
      {
        _timer = new Timer(ServerStatus);
      }
    }
    void ServerStatus(object state)
    {
      LOG.dc("server status");
      var msg = new SERVER_STATUS_NOTI();
      this.SendAll(msg);
    }
    public override void Start()
    {
      _timer?.Change(1000, 1000 * 60);
      base.Start();
    }
    public override void Stop()
    {
      _timer?.Change(Timeout.Infinite, Timeout.Infinite);
      base.Stop();
    }
  }
}