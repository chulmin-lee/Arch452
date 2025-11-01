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
    protected object LOCK = new object();
    protected Dictionary<int, T> DATA = new Dictionary<int, T>();
    /// <summary>
    /// 내가 가입할 대상
    /// </summary>
    public List<D> SubscribeList { get; private set; } = new List<D>();
    /// <summary>
    /// 나의 가입자들
    /// </summary>
    HashSet<IDataSubscriber<D>> Subscribers = new HashSet<IDataSubscriber<D>>();
    protected Schedule Schedule;
    protected string BackupJsonPath;
    protected bool IsBackup;
    protected int Interval;

    public DataExtractorBase(IGeneratorMemberOwner owner, D id)
    {
      this.ID = id;
      this.IsBackup = owner.IsBackup;
      this.BackupJsonPath = Path.Combine(owner.BackupDataPath, $"{typeof(T).Name}.json");
      this.Interval = owner.ScheduleInterval;
    }
    protected virtual void Initialize()
    {
      var s = new Schedule(this.RunSchedule).WithName(this.ID.ToString()).NonReentrant();
      s.ToRunEverySecond(this.Interval);

      this.Schedule = s;
    }
    public void Start()
    {
      this.Initialize();
      if (this.IsReady)
      {
        this.RunSchedule();
        this.start_schedule();
      }
    }
    public void Stop()
    {
      this.stop_schedule();
    }
    protected virtual List<T> query() => new List<T>();
    public virtual void RunSchedule()
    {
      lock (LOCK)
      {
        if (this.IsReady)
        {
          var data = this.query();
          this.check_update(data);
        }
      }
    }

    #region 데이터 통지 하기
    INotifyData<D> _notify_data;
    ModelEqualityComparer<T> DtoComparer = new ModelEqualityComparer<T>();
    protected virtual void check_update(List<T> news)
    {
      var updated = this.DATA.CheckUpdate(news, DtoComparer);
      if (updated.IsChanged)
      {
        LOG.ic($"{this.ID} updated. {updated}");
        this.DATA.Clear();
        updated.All.ForEach(x => this.DATA.Add(x.ID, x));
        if (this.IsBackup)
        {
          NewtonJson.Serialize(updated.All, this.BackupJsonPath);
        }

        _notify_data = this.data_mapping(updated);
        if (_notify_data != null)
        {
          this.Subscribers.ToList().ForEach(s => Task.Run(() => s.OnDataNotify(_notify_data)));
        }
      }
    }
    protected virtual INotifyData<D> data_mapping(UpdateData<T> updated) => null;

    /// <summary>
    /// 데이터 변경시 통지 받을 가입자 가입
    /// </summary>
    public bool Subscribe(IDataSubscriber<D> subscriber)
    {
      lock (LOCK)
      {
        if (!subscriber.SubscribeList.Contains(this.ID))
        {
          LOG.ec($"no service id");
          return false;
        }
        this.Subscribers.Add(subscriber);

        if(this.IsReady && _notify_data != null)
        {
          subscriber.OnDataNotify(_notify_data);
        }
        return true;
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
    protected virtual bool data_notified(INotifyData<D> o) => false;
    public void OnDataNotify(INotifyData<D> o)
    {
      lock (LOCK)
      {
        LOG.dc($"{this.ID}: {o.ID} data update");
        if (!this.data_notified(o))
        {
          throw new ServiceException($"Data event: {o.ID} error");
        }

        if (!this.IsReady)
        {
          _updateFlags |= 1 << this.SubscribeList.IndexOf(o.ID); ;
          if (this.IsReady)
          {
            LOG.ic($"{this.ID}: ready");

            // TODO 여기서 조회시작 및 start schedule??
            this.RunSchedule();
            this.start_schedule();
          }
        }
      }
    }
    #endregion data 통지 받기

    void start_schedule() => ScheduleManager.Add(this.Schedule);
    void stop_schedule() => ScheduleManager.RemoveJob(this.Schedule);
  }
}