using System;

namespace Framework.Network
{
  public enum NetworkState
  {
    None,
    Offline,            // 연결전 또는 disconnect 호출
    Connecting,         // 오래 걸릴수있다
    ConnectionFailed,
    Online,
    Closing,            // 오래 걸릴수 있다
    ConnectionLost      // 원격 서버와 연결이 끊어졌거나, 에러가 발생한 상태
  }

  /// <summary>
  /// Network User가 사용하는 기능
  /// - M 만 노출한다
  /// </summary>
  public interface INetworkConnection<M> : IDisposable
    where M : class, new()
  {
    string NetworkId { get; }
    NetworkState State { get; }
    bool IsOnline { get; }

    void Connect();
    /// <summary>
    /// local에서 연결을 끊는다.
    /// </summary>
    void Close();
    void Send(M m);
  }
}