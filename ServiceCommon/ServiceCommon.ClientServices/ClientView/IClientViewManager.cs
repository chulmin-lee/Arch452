namespace ServiceCommon.ClientServices
{
  /// <summary>
  /// Client View 에서 제공
  /// </summary>
  public interface IClientViewManager
  {
    string ClientVersion { get; }
    string ClientPath { get; }
    string ClientRestartArg { get; }
    void ConfigChanged(IPackageViewConfig o);
    void ReceiveMessage(ServiceMessage m);
    void Exit();
  }
}