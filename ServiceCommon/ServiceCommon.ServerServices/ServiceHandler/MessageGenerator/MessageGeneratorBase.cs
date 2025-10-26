using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ServiceCommon.ServerServices
{
  public abstract class MessageGeneratorBase<D> : IMessageGenerator, IGeneratorMemberOwner
    where D : Enum
  {
    public string ServiceName { get; private set; }
    public bool IsBackup { get; protected set; }
    public string BackupDataPath { get; protected set; }
    public string SimDataPath { get; protected set; }
    public int ScheduleInterval { get; protected set; }
    public string ServiceDir { get; private set; }
    // members
    protected Dictionary<D, IDataExtractor<D>> Extractors = new Dictionary<D, IDataExtractor<D>>();
    protected Dictionary<SERVICE_ID, IMessageTransformer<D>> Transformers = new Dictionary<SERVICE_ID, IMessageTransformer<D>>();

    public MessageGeneratorBase(string name)
    {
      this.ServiceName = name;
      this.ServiceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.ServiceName);
      this.BackupDataPath = Path.Combine(this.ServiceDir, "DATA");
      this.SimDataPath = Path.Combine(this.ServiceDir, "SIM");
    }

    public virtual void Start() { }
    public bool MessageSubscribe(IMessageSubscriber loader)
    {
      bool created = false;
      var transformer = this.GetTransformer(loader.ID, out created);
      transformer.Subscribe(loader);

      if (created)
      {
        this.Transformers.Add(transformer.ID, transformer);

        var run_list = new List<IDataExtractor<D>>();
        foreach (var d in transformer.SubscribeList)
        {
          if (this.Extractors.ContainsKey(d))
          {
            this.Extractors[d].Subscribe(transformer);
          }
          else
          {
            run_list.AddRange(this.subscribe_to_extractor(d, transformer));
          }
        }

        // run_list 실행
        foreach (var d in run_list)
        {
          d.Start();
        }
      }

      return true;
    }
    /// <summary>
    /// 참조하는 extractor를 생성한 후 가입한다
    /// 생성한 extractor가 참조하는 extractor가 있는 경우 재귀적으로 생성 & 가입한다
    /// </summary>
    /// <param name="id">가입 대상 extractor ID</param>
    /// <param name="subs">가입자</param>
    /// <returns></returns>
    List<IDataExtractor<D>> subscribe_to_extractor(D id, IDataSubscriber<D> subs)
    {
      var run_list = new List<IDataExtractor<D>>();

      var extractor = this.CreateExtractor(id);
      {
        run_list.Add(extractor);
        this.Extractors.Add(extractor.ID, extractor);
        extractor.Subscribe(subs);
      }

      foreach (var d in extractor.SubscribeList)
      {
        if (this.Extractors.ContainsKey(d))
        {
          this.Extractors[d].Subscribe(extractor);
        }
        else
        {
          run_list.AddRange(this.subscribe_to_extractor(d, extractor));
        }
      }
      return run_list;
    }

    protected abstract IDataExtractor<D> CreateExtractor(D id);
    protected abstract IMessageTransformer<D> GetTransformer(SERVICE_ID id, out bool created);
    public void Stop()
    {
      Extractors.Values.ToList().ForEach(data => data.Stop());
      Extractors.Clear();
      Transformers.Clear();
    }
  }
}