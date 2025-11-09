using Common;

namespace ServiceCommon.ServerServices
{
  public abstract class MessageLoaderBase : IMessageLoader
  {
    public SERVICE_ID ID { get; private set; }
    protected object LOCK = new object();
    bool _initialized = false;
    public MessageLoaderBase(SERVICE_ID id)
    {
      this.ID = id;
    }
    //======================================
    // 서비스 직접 요청
    //======================================
    public ServiceMessage RequestService(ServiceMessage m)
    {
      LOG.dc($"[{this.ID}] {m.ServiceId}");
      this.check_id(m.ServiceId);
      lock (LOCK)
      {
        return this.request_service(m);
      }
    }
    public void RequestService(IServerSession session, ServiceMessage m)
    {
      var msg = this.RequestService(m);
      if (msg != null)
      {
        session.Send(msg);
      }
    }
    protected abstract ServiceMessage request_service(ServiceMessage m);

    //======================================
    // 이벤트 통지
    //======================================
    public void OnMessageNotify(INotifyMessage m)
    {
      this.check_id(m.ID);
      lock (LOCK)
      {
        _initialized = true;
        LOG.dc($"[{this.ID}] {m.ID} updated");
        this.message_notified(m);
      }
    }
    protected abstract void message_notified(INotifyMessage m);

    //========================================
    // 가입자 처리
    //========================================
    public void Subscribe(IServerSession s)
    {
      if (!_initialized)
      {
        LOG.ec($"{this.ID} not initialized");
        return;
      }

      LOG.ic($"{this.ID} {s}");
      lock (LOCK) this.subscribe_session(s);
    }
    protected abstract void subscribe_session(IServerSession s);

    public void Unsubscribe(IServerSession s)
    {
      LOG.ic($"{this.ID} {s}");
      lock (LOCK) this.unsubscribe_session(s);
    }
    protected abstract void unsubscribe_session(IServerSession s);

    void check_id(SERVICE_ID id)
    {
#if DEBUG
      if (this.ID != id)
      {
        throw new ServiceException($"{id} not supported. im: {this.ID}");
      }
#endif
    }
  }
}