using ServiceCommon;
using ServiceCommon.ClientServices;
using System;

namespace EUMC.ClientServices
{
  public static class CLIENT_SERVICE
  {
    public static int ClientId => Instance.ClientId;
    static IClientService _instance;
    static IClientService Instance => _instance ?? throw new Exception("not initialized");

    public static void Initialize(IClientViewManager vm)
    {
      _instance = new ClientServiceImpl(vm);
    }
    public static void Start()
    {
      Instance.Start();
    }

    public static void Send(ServiceMessage m)
    {
      Instance.SendMessage(m);
    }

    public static void Close()
    {
      // 명시적으로 클라이언트 종료시
      // watchdog 서버도 종료시킬것
      Instance.Close();
    }

    public static void Restart()
    {
      throw new NotImplementedException();
    }
  }
}