namespace ServiceCommon.ServerServices
{
  public interface IMessageLoader : IMessageSubscriber
  {
    ServiceMessage RequestService(ServiceMessage m);
    void RequestService(IServerSession session, ServiceMessage m);

    void Subscribe(IServerSession s);
    void Unsubscribe(IServerSession s);
  }
}