using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class RAD_TR_Extractor : DataExtractor<RAD_TR_PT_DTO>
  {
    public RAD_TR_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ENDO)
    {
    }
    protected override List<RAD_TR_PT_DTO> query() => this.Repository.RAD_TR_PT();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<RAD_TR_PT_DTO> updated)
    {
      var o = new UpdateData<RAD_TR_PT_INFO>()
      {
        Constant = Mapper.Map<RAD_TR_PT_DTO[], List<RAD_TR_PT_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<RAD_TR_PT_DTO[], List<RAD_TR_PT_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<RAD_TR_PT_DTO[], List<RAD_TR_PT_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<RAD_TR_PT_DTO[], List<RAD_TR_PT_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<RAD_TR_PT_INFO>(this.ID, o);
    }
  }
}