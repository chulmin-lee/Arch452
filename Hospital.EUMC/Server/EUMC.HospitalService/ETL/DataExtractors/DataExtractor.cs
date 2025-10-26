using AutoMapper;
using Framework.DataSource;
using ServiceCommon.ServerServices;

namespace EUMC.HospitalService
{
  internal class DataEventData<T> : NotifyData<T, DATA_ID> where T : class
  {
    public DataEventData(DATA_ID id, UpdateData<T> d) : base(id, d) { }
  }

  internal abstract class DataExtractor<T> : DataExtractorBase<T, DATA_ID>
    where T : OriginDataModel
  {
    protected IHospitalRepository Repository { get; private set; }
    protected IMapper Mapper = DataMapper.Mapper;

    public DataExtractor(IHospitalMemberOwner owner, DATA_ID id) : base(owner, id)
    {
      this.Repository = owner.Repository;
    }
  }
}