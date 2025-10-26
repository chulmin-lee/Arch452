using Framework.Network;
using System;

namespace ServiceCommon
{
  public static class TcpMessageConverter
  {
    // TCP 처럼 분할/중첩 데이타 수신이 발생하는 경우, 내부에 버퍼를 운영해야하므로 매번 MessageConverter를 생성해야 한다
    public static IMessageConverter<ServiceMessage, ArraySegment<byte>, ArraySegment<byte>> ClientConverter => new TcpMessageConverterClient();
    public static IMessageConverter<ServiceMessage, ArraySegment<byte>, ArraySegment<byte>> ServerConverter => new TcpMessageConverterServer();
  }
}