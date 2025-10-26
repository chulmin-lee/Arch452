using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceCommon.ServerServices
{
  public abstract class MessageTransformerBase<D> : IMessageTransformer<D>
    where D : Enum
  {
    public SERVICE_ID ID { get; private set; }
    /// <summary>
    /// 내가 가입할 대상
    /// </summary>
    public List<D> SubscribeList { get; set; } = new List<D>();
    /// <summary>
    /// 나의 가입자
    /// </summary>
    HashSet<IMessageSubscriber> Subscribers  = new HashSet<IMessageSubscriber>();
    protected object LOCK = new object();
    public MessageTransformerBase(SERVICE_ID id)
    {
      this.ID = id;
    }

    public bool Subscribe(IMessageSubscriber subscriber)
    {
      lock (LOCK)
      {
        if (this.ID != subscriber.ID)
        {
          LOG.ec($"no service id");
          return false;
        }
        this.Subscribers.Add(subscriber);
        return true;
      }
    }

    #region data 통지 받기
    protected bool IsReady => _readyFlag == _updateFlags;
    int _readyFlag = 0;
    int _updateFlags = 0;
    /// <summary>
    /// 참조할 대상 초기화
    /// </summary>
    protected virtual void subscribe(params D[] publishers)
    {
      this.SubscribeList.AddRange(publishers);
      for (int i = 0; i < this.SubscribeList.Count; i++)
      {
        _readyFlag |= 1 << i;
      }
    }
    protected abstract INotifyMessage data_notified(INotifyData<D> o);
    public void OnDataNotify(INotifyData<D> o)
    {
      lock (LOCK)
      {
        if (!this.IsReady)
        {
          _updateFlags |= 1 << this.SubscribeList.IndexOf(o.ID);
          if (this.IsReady)
          {
            LOG.ic($"{this.ID}: ready");
          }
        }

        LOG.dc($"[{this.ID}] {o.ID} data updated");
        var updated = this.data_notified(o);
        if (updated != null)
        {
          LOG.dc($"[{this.ID}] event publish");
          this.Subscribers.ToList().ForEach(s => Task.Run(() => s.OnMessageNotify(updated)));
        }
      }
    }
    #endregion data 통지 받기
  }
}