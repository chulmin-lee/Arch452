using System;
using System.Collections.Generic;

namespace ServiceCommon.ServerServices
{
  public interface IMessageSubscriber
  {
    SERVICE_ID ID { get; }
    void OnMessageNotify(INotifyMessage e);
  }

  public interface IMessageTransformer<D> : IDataSubscriber<D>
    where D : Enum
  {
    SERVICE_ID ID { get; }
    bool Subscribe(IMessageSubscriber subscriber);
  }

  public interface INotifyMessage
  {
    SERVICE_ID ID { get; }
  }

  public interface INotifyMessage<T> : INotifyMessage
  {
    List<T> Updated { get; }
  }

  public class NotifyMessage<T> : INotifyMessage
  {
    public SERVICE_ID ID { get; set; }
    public List<T> Updated { get; set; } = new List<T>();
  }
}