using AutoMapper;
using Framework.DataSource;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System;

namespace EUMC.HospitalService
{
  internal class DataEventData<T> : NotifyData<T, DATA_ID> where T : class
  {
    public DataEventData(DATA_ID id, UpdateData<T> d) : base(id, d) { }
  }

  public static class DataEventDataExtension
  {
    public static UpdateData<T> Data<T>(this INotifyData<DATA_ID> d) where T : class
    {
      return (d as NotifyData<T, DATA_ID>)?.Data ?? throw new InvalidOperationException("Data");
    }
    public static List<T> All<T>(this INotifyData<DATA_ID> d) where T : class
    {
      return (d as NotifyData<T, DATA_ID>)?.Data.All ?? throw new InvalidOperationException("Data");
    }
  }


  internal abstract class DataExtractor<T> : DataExtractorBase<T, DATA_ID>
    where T : OriginDataModel
  {
    protected IEumcRepository Repository { get; private set; }
    protected IMapper Mapper = DataMapper.Mapper;

    public DataExtractor(IHospitalMemberOwner owner, DATA_ID id) : base(owner, id)
    {
      this.Repository = owner.Repository;
    }
  }
}