using Common;
using Framework.DataSource;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ServiceCommon.ServerServices
{
  public abstract partial class DataExtractorBase<T, D> : IDataExtractor<D>
    where D : Enum
    where T : OriginDataModel
  {
    public D ID { get; private set; }
    /// <summary>
    /// 내가 가입할 대상
    /// </summary>
    public List<D> SubscribeList { get; private set; } = new List<D>();
    /// <summary>
    /// 나의 가입자들
    /// </summary>
    HashSet<IDataSubscriber<D>> Subscribers = new HashSet<IDataSubscriber<D>>();

    protected object LOCK = new object();
    protected Dictionary<int, T> DATA = new Dictionary<int, T>();
    protected string BackupJsonPath;
    protected bool IsBackup;
    protected int Interval;

    bool IsInitialized = false;
    public DataExtractorBase(IGeneratorMemberOwner owner, D id)
    {
      this.ID = id;
      this.IsBackup = owner.IsBackup;
      this.BackupJsonPath = Path.Combine(owner.BackupDataPath, $"{typeof(T).Name}.json");
      this.Interval = owner.ScheduleInterval;
    }
    public void Start()
    {
      if (this.IsReady)
      {
        this.RunSchedule();
      }
      // 무조건 스케쥴러는 시작해야 한다. IsReady가 될때까지 스케쥴링
      var s = new Schedule(this.RunSchedule).WithName(this.ID.ToString()).NonReentrant();
      s.ToRunEverySecond(this.Interval);
      ScheduleManager.Add(s);
    }
    public void Stop()
    {
      ScheduleManager.RemoveJob(this.ID.ToString());
    }
    protected virtual List<T> query() => new List<T>();
    public virtual void RunSchedule()
    {
      lock (LOCK)
      {
        if (this.IsReady)
        {
          var updated = this.DATA.CheckUpdate(this.query(), DtoComparer);

          if(!IsInitialized)
          {
            LOG.wc($"[{this.ID}] Initialized");
            IsInitialized = true;
            this.data_updated(updated);
          }
          else if (updated.IsChanged)
          {
            LOG.ic($"[{this.ID}] updated. {updated}");
            this.data_updated(updated);
          }
        }
      }
    }

    #region 데이터 통지 하기
    INotifyData<D> _notify_data;
    ModelEqualityComparer<T> DtoComparer = new ModelEqualityComparer<T>();
    void data_updated(UpdateData<T> updated)
    {
      this.DATA.Clear();
      updated.All.ForEach(x => this.DATA.Add(x.ID, x));
      if (this.IsBackup)
      {
        NewtonJson.Serialize(updated.All, this.BackupJsonPath);
      }
      _notify_data = this.data_mapping(updated);
      this.Subscribers.ToList().ForEach(s => Task.Run(() => s.OnDataNotify(_notify_data)));
    }
    protected virtual INotifyData<D> data_mapping(UpdateData<T> updated) => null;

    /// <summary>
    /// 데이터 변경시 통지 받을 가입자 가입
    /// </summary>
    public bool Subscribe(IDataSubscriber<D> subscriber)
    {
      lock (LOCK)
      {
        if (subscriber.SubscribeList.Contains(this.ID))
        {
          this.Subscribers.Add(subscriber);
          if (this.IsReady && _notify_data != null)
          {
            subscriber.OnDataNotify(_notify_data);
          }
          return true;
        }
        else
        {
          LOG.ec($"no service id");
          return false;
        }
      }
    }
    #endregion 데이터 통지 하기

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
    public void OnDataNotify(INotifyData<D> o)
    {
      lock (LOCK)
      {
        if (this.data_notified(o))
        {
          LOG.dc($"[{this.ID}] {o.ID} data update");
          if (!this.IsReady)
          {
            _updateFlags |= 1 << this.SubscribeList.IndexOf(o.ID); ;
          }
        }
        else
        {
          LOG.ec($"[{this.ID}] Data event: {o.ID} error");
        }
      }
    }
    protected virtual bool data_notified(INotifyData<D> o) => false;
    #endregion data 통지 받기
  }
}