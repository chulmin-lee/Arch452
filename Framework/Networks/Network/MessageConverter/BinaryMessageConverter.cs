using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Network
{
  public interface IBinaryMessageConverter<M> : IMessageConverter<M, ArraySegment<byte>, ArraySegment<byte>>
    where M : class, new()
  {
  }

  /// <summary>
  /// header + PDU + ED
  /// PDU : string => byte array
  /// </summary>
  public abstract class BinaryMessageConverter<M> : IBinaryMessageConverter<M>
    where M : class, new()
  {
    MemoryBlocks MemoryBlocks = new MemoryBlocks();
    int Count => MemoryBlocks.Count;
    Queue<M> MessageQueue = new Queue<M>();
    int _wanted;
    object _lock = new object();

    //================================
    // Send
    //================================
    public abstract ArraySegment<byte> SendMessage(M m);

    //================================
    // Receive
    //================================
    public M ReadMessage(ArraySegment<byte> seg)
    {
      lock (_lock)
      {
        LOG.tc($"ReadMessage: count= {seg.Count}");

        // 버퍼에 데이타가 없을때 수신한 데이타를 바로 메시지로 변환할 수 있는 경우
        if (this.Count == 0 && seg.Count >= MessageHeader.HeaderSize)
        {
          do
          {
            if (seg.Array == null) throw new ArgumentNullException("seg.Array == null");
            if (seg.Count == 0) break;
            int message_size = MessageHeader.GetPacketSize(seg);
            if (message_size == 0 || message_size > seg.Count)
            {
              // 더 수신해야함
              break;
            }
            else if (message_size == -1)
            {
              this.Clear();
              return null;
            }
            // 완료 메시지
            else if (message_size <= seg.Count)
            {
              LOG.tc("Zero Copy Message Received: " + message_size);

              var b = new ArraySegment<byte>(seg.Array, seg.Offset, message_size);
              this.add_completed_message(b);
              // .net framework에는 slice가 없다
              seg = new ArraySegment<byte>(seg.Array, seg.Offset + message_size, seg.Count - message_size);
              //seg = seg.Slice(size);
            }
          } while (true);
        }

        if (seg.Count == 0)
        {
          return this.ReadMessage();
        }

        this.MemoryBlocks.Write(seg);

        do
        {
          // 중첩된 메시지가 있는 경우 모두 처리한다
          if (_wanted == 0 && this.Count >= MessageHeader.HeaderSize)
          {
            _wanted = MessageHeader.GetPacketSize(this.MemoryBlocks.Peek(MessageHeader.HeaderSize));
          }

          if (_wanted == -1) { this.Clear(); return null; }
          if (_wanted == 0) break;
          if (_wanted <= this.Count)
          {
            var b = this.MemoryBlocks.Read(_wanted);
            this.add_completed_message(b);
            _wanted = 0;
          }
        } while (_wanted <= this.Count);

        return this.ReadMessage();
      }
    }
    public M ReadMessage()
    {
      lock (_lock)
      {
        if (this.MessageQueue.Any())
          return this.MessageQueue.Dequeue();
        return null;
      }
    }
    void add_completed_message(ArraySegment<byte> block)
    {
      var msg = this.Deserialize(block);
      if (msg != null)
      {
        lock (_lock)
        {
          this.MessageQueue.Enqueue(msg);
        }
      }
    }
    protected abstract M Deserialize(ArraySegment<byte> r);

    public void Clear()
    {
      lock (_lock)
      {
        this.MemoryBlocks.Clear();
        this.MessageQueue.Clear();
        _wanted = 0;
      }
    }
    public void Close()
    {
      this.Clear();
    }
    public void Dispose()
    {
      this.Close();
    }
  }
}