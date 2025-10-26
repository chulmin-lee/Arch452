using Common;
using Framework.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServiceCommon
{
  public abstract class TcpMessageConverterBase : BinaryMessageConverter<ServiceMessage>
  {
    protected Dictionary<int, Type> _message_types = new Dictionary<int, Type>();

    public TcpMessageConverterBase()
    {
      this.Initialize();
    }
    protected abstract void Initialize();

    public override ArraySegment<byte> SendMessage(ServiceMessage m)
    {
      var s = NewtonJson.Serialize(m);
      var pdu = LittleEndianConverter.GetBytes(s);

      using (MemoryStream ms = new MemoryStream())
      {
        var bw = new BinaryWriter(ms);
        // header
        var header = new MessageHeader((int)m.ServiceId, pdu.Length);
        bw.Write(header.ToArray());
        bw.Write(pdu);
        bw.Write(MSG_HEADER.ED);
        return new ArraySegment<byte>(ms.ToArray());
      }
    }

    protected override ServiceMessage Deserialize(ArraySegment<byte> seg)
    {
      if (seg.Array == null)
        throw new ArgumentNullException("seg.Array == null");

      var header = MessageHeader.ToHeader(seg);
      if (header.message_size > 0 && header.message_size <= seg.Count)
      {
        var pdu = new ArraySegment<byte>(seg.Array, seg.Offset + MSG_HEADER.HeaderSize, header.PduSize);
        var message = pdu.GetString();

        var type = _message_types[header.message_id];
        var msg = NewtonJson.Deserialize<ServiceMessage>(type, message);
        if (msg == null)
        {
          LOG.ec($"{(SERVICE_ID)header.message_id} deserialize fail");
          return null;
        }
        return msg;
      }
      return null;
    }
  }
}