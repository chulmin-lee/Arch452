using Framework.DataSource;
using System;
using System.Collections.Generic;

namespace ServiceCommon.ServerServices
{
  public interface IDataSubscriber<D>
    where D : Enum
  {
    List<D> SubscribeList { get; }       // 가입 목록
    void OnDataNotify(INotifyData<D> m); // 이벤트 수신 (mediator가 분배)
  }

  public interface IDataExtractor<D> : IDataSubscriber<D>
    where D : Enum
  {
    D ID { get; }
    void Start();
    void Stop();
    void RunSchedule(); // 주기적으로 updat
    /// <summary>
    /// DataExtractor에게 통지 받을 DataSubscriber 등록
    /// </summary>
    bool Subscribe(IDataSubscriber<D> subscriber);
  }

  //===================================
  // DataExtractor의 Notify data
  //===================================
  public interface INotifyData<D>
    where D : Enum
  {
    D ID { get; }
  }

  public class NotifyData<T, D> : INotifyData<D>
    where T : class
    where D : Enum
  {
    public D ID { get; set; }
    public UpdateData<T> Data;
    public NotifyData(D id, UpdateData<T> d)
    {
      this.ID = id;
      this.Data = d;
    }
  }

  public static class NotifyDataExtension
  {
    public static UpdateData<T> Data<T, D>(this INotifyData<D> d) where T : class where D : Enum
    {
      return (d as NotifyData<T, D>)?.Data ?? throw new InvalidOperationException("Data");
    }
    public static List<T> All<T, D>(this INotifyData<D> d) where T : class where D : Enum
    {
      return (d as NotifyData<T, D>)?.Data.All ?? throw new InvalidOperationException("Data");
    }
  }
}