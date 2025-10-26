using System;

namespace Framework.Network.TCP
{
  public class TcpServerConnection<M> : TcpConnectionBase<M>
    where M : class, new()
  {
    public TcpServerConnection(IConnectionOwner<M> owner, IMessageConverter<M, ArraySegment<byte>, ArraySegment<byte>> converter)
      : base(owner, converter, NetworkState.Online)
    {
      // 여기서 SocketConnected()를 호출하면 객체 생성전에 메시지 수신이 발생할수있다
      // 그래서 객체 생성후에 호출한다
    }
  }
}