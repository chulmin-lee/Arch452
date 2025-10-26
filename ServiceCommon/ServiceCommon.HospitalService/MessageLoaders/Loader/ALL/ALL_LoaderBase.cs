using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  /// <summary>
  /// 데이타 분류없이 사용하는 경우
  /// - 원본 데이터 변경시 모든 데이타를 가입자에게 전송한다 (예. 수술실, 약제과)
  /// </summary>
  /// <typeparam name="T"></typeparam>

  public abstract class ALL_LoaderBase<T> : MessageLoaderBase
  {
    protected Dictionary<int, SessionMember> Sessions = new Dictionary<int, SessionMember>();
    protected List<T> Items = new List<T>();
    public ALL_LoaderBase(SERVICE_ID id) : base(id) { }

    protected override ServiceMessage request_service(ServiceMessage m) => this.create_message();
    protected override void message_notified(INotifyMessage m)
    {
      var data = m as NotifyMessage<T>;
      if (data == null) throw new Exception($"{m.ID} not supported");

      this.Items.Clear();
      this.Items.AddRange(data.Updated);

      if (this.Sessions.Any())
      {
        var msg = this.create_message();
        this.Sessions.Values.ToList().ForEach(s => s.Send(msg));
      }
    }
    protected abstract ServiceMessage create_message();
    protected override void subscribe_session(IServerSession s)
    {
      var sm = new SessionMember(s);
      this.Sessions.Add(sm.ID, sm);
      sm.Send(this.create_message());
    }
    protected override void unsubscribe_session(IServerSession s)
    {
      this.Sessions.Remove(s.Key);
    }
  }
}