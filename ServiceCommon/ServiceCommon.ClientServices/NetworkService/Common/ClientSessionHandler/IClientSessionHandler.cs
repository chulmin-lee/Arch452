namespace ServiceCommon.ClientServices
{
  public interface IClientSessionHandler
  {
    bool IsConnected { get; }
    void Connect(ServiceMessage m);
    void Connect();
    void Send(ServiceMessage m);
    void Close();
  }
}