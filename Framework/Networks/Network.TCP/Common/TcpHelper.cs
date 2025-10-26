using Common;
using System;
using System.Net;
using System.Net.Sockets;

namespace Framework.Network.TCP
{
  public class TcpHelper
  {
    public static M ReceiveFromSocket<M>(Socket socket, IMessageConverter<M, ArraySegment<byte>, ArraySegment<byte>> converter)
      where M : class, new()
    {
      // MemoryPoolHelper의 기본 크기를 모르므로 2048로 지정
      var block = MemoryPoolHelper.GetBlock(2048);
      converter.Clear();

      try
      {
        while (true)
        {
          int read = socket.Receive(block.Array, block.Offset, block.BlockSize, SocketFlags.None);
          if (read == 0)
          {
            LOG.ec("read == 0");
            return null;
          }
          if (read != block.Count)
            throw new Exception($"{read} != {block.Count}");
          var msg = converter.ReadMessage(block.ReadAll());
          if (msg != null)
          {
            return msg;
          }
        }
      }
      catch (Exception ex)
      {
        LOG.ec($"{ex.Message}");
        return null;
      }
      finally
      {
        LOG.ec("multi return => ReceiveFromSocket finally");
        block.ReturnBuffer();
        converter.Clear();
      }
    }
    public static M Request<M>(IMessageConverter<M, ArraySegment<byte>, ArraySegment<byte>> converter, IPEndPoint server, M request)
          where M : class, new()
    {
      using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
      {
        converter.Clear();
        try
        {
          socket.Connect(server);
          var seg = converter.SendMessage(request);

          seg.Array.IsNull();
          //GuardAgainst.Null(seg.Array);
          socket.Send(seg.Array, seg.Offset, seg.Count, SocketFlags.None);
          return ReceiveFromSocket(socket, converter);
        }
        catch (Exception ex)
        {
          LOG.ec($"Request failed: {ex.Message}");
          return null;
        }
        finally
        {
          converter.Clear();
        }
      }
    }
  }
}