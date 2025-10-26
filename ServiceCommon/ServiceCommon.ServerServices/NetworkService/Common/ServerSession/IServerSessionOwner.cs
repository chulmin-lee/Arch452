namespace ServiceCommon.ServerServices
{
  public interface IServerSessionOwner
  {
    void OnMessageReceived(IServerSession session, ServiceMessage message);
    void OnSessionDisconnected(IServerSession session);
  }
}