using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class OPERATION_Extractor : DataExtractor<OPERATION_DTO>
  {
    public OPERATION_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.OPERATION)
    {
    }
    protected override List<OPERATION_DTO> query() => this.Repository.OPERATION();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<OPERATION_DTO> updated)
    {
      var o = new UpdateData<OPERATION_INFO>()
      {
        Constant = Mapper.Map<OPERATION_DTO[], List<OPERATION_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<OPERATION_DTO[], List<OPERATION_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<OPERATION_DTO[], List<OPERATION_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<OPERATION_DTO[], List<OPERATION_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<OPERATION_INFO>(this.ID, o);
    }
  }
}