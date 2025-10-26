namespace Framework.Network
{
  public interface IConnectionOwner<M>
    where M : class, new()
  {
    void OnMessageReceived(M message);
    void OnStateChanged(NetworkState state);
  }
}