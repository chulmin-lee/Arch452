using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCommon.ClientServices
{
  public class WatchDogClient
  {
    public WatchDogClient(int port)
    {
    }

    public void Restart()
    {
    }
    public void ShutDown() { }
    public void Reboot() { }
    public void Update(string path)
    {
    }

    public void Start()
    {
      // 연결만 하고, watch dog server는 실행시키지 않는다
    }

    public void Close()
    {
      // watch dog server 에게 종료 명령
    }
  }
}