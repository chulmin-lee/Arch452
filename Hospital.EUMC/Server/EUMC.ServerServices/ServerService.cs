using Common;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.ServerServices
{
  public partial class ServerService : IServerService
  {
    string _hspCode;
    IServerSessionHandler SessionHandler;
    List<IMessageServiceHandler> MessageHandlers = new List<IMessageServiceHandler>();
    public UserServiceHandler UserService { get; private set; }
    Dictionary<SERVICE_ID, IMessageServiceHandler> MessageMap = new Dictionary<SERVICE_ID, IMessageServiceHandler>();
    Dictionary<PACKAGE, IMessageServiceHandler> PackageMap = new Dictionary<PACKAGE, IMessageServiceHandler>();

    public ServerService(string hspCode)
    {
      _hspCode = hspCode;

      LOG.ic($"HspCode: {_hspCode}");

      var config = ServerConfigurations.Load(_hspCode, "Server.Config");
      bool grpc = config.GRPC_ENABLED;
      int port = config.SERVER_PORT;

      this.SessionHandler = //grpc ? SERVER_SESSION_HANDLER.Grpc(this, port)
                                  SERVER_SESSION_HANDLER.Tcp(this, port);

      this.init_message_handler();
      this.UserService = this.init_user_service();
      this.MessageHandlers.Add(this.UserService);
    }

    public void Start()
    {
      this.MessageHandlers.ForEach(handler => handler.Start());
      this.SessionHandler.Start();
    }
    public void Stop()
    {
      this.MessageHandlers.ForEach(handler => handler.Stop());
      this.SessionHandler.Stop();
    }
    public bool SessionConnected(IServerSession session)
    {
      LOG.dc(session);
      this.UserService.Subscribe(session);

      if (session.UseService)
      {
        if (this.PackageMap.ContainsKey(session.Package))
        {
          this.PackageMap[session.Package].Subscribe(session);
        }
        else
        {
          LOG.ec($"{session.Package} not exist");
        }
      }
      return true;
    }
    public bool SessionDisconnected(IServerSession session)
    {
      LOG.dc(session);
      this.UserService.Unsubscribe(session);

      if (session.UseService)
      {
        if (this.PackageMap.ContainsKey(session.Package))
        {
          this.PackageMap[session.Package].Unsubscribe(session);
        }
        else
        {
          LOG.ec($"{session.Package} not exist");
        }
      }
      return true;
    }
    /// <summary>
    /// 세션없이 요청이 들어오는 경우
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public ServiceMessage RequestService(ServiceMessage m)
    {
      LOG.ic($"request: {m.ServiceId}");
      if (this.MessageMap.ContainsKey(m.ServiceId))
      {
        return this.MessageMap[m.ServiceId].RequestService(m);
      }
      return ServiceMessage.None;
    }
    public void RequestService(IServerSession session, ServiceMessage m)
    {
      if (this.MessageMap.ContainsKey(m.ServiceId))
      {
        this.MessageMap[m.ServiceId].RequestService(session, m);
      }
    }
  }
}