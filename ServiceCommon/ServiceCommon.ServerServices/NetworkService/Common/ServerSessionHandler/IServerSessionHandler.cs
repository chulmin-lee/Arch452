namespace ServiceCommon.ServerServices
{
  public interface IServerSessionHandler
  {
    void Start();
    void Stop();
    void Send(int key, ServiceMessage m);
    void SendAll(ServiceMessage m);
  }
}