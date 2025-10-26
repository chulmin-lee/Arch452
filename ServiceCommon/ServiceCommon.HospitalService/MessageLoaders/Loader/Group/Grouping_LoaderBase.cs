using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  /// <summary>
  /// 데이타가 그룹으로 나눠지는 경우 처리
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="K">group key type</typeparam>
  public abstract class Grouping_LoaderBase<T, K> : MessageLoaderBase
    where T : class, IGroupKeyData<K>
    //where K : notnull
  {
    protected Dictionary<int, GroupingDataSession<T,K>> Sessions = new Dictionary<int, GroupingDataSession<T, K>>();
    protected GroupingDataCollection<T,K> Items = new GroupingDataCollection<T, K> { };

    public Grouping_LoaderBase(SERVICE_ID id) : base(id)
    {
    }

    protected override void message_notified(INotifyMessage m)
    {
      var data = m as NotifyMessage<T>;
      if (data == null) throw new Exception($"{m.ID} not supported");
      if (this.Items == null) throw new Exception($"{this.ID} not init");

      Items.Update(data.Updated);

      foreach (var session in this.Sessions.Values)
      {
        var changed = session.SendOrNot(data.Updated);
        if (changed.Any())
        {
          session.Send(this.create_message(changed));
        }
      }
    }

    protected ServiceMessage create_message(K key)
    {
      var item = this.Items.Select(key);
      return item != null ? this.create_message(item) : null;
    }
    protected virtual ServiceMessage create_message(List<K> keys)
    {
      var items = this.Items.Select(keys);
      return items.Any() ? this.create_message(items) : null;
    }

    protected abstract ServiceMessage create_message(T item);
    protected abstract ServiceMessage create_message(List<T> list);

    protected override void unsubscribe_session(IServerSession s)
    {
      this.Sessions.Remove(s.Key);
    }
  }
}