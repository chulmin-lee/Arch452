using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class ENDO_WGO_Extractor : DataExtractor<ENDO_WGO_PT_DTO>
  {
    public ENDO_WGO_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ENDO_WGO)
    {
    }
    protected override List<ENDO_WGO_PT_DTO> query() => this.Repository.ENDO_WGO();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ENDO_WGO_PT_DTO> updated)
    {
      var o = new UpdateData<ENDO_WGO_PT_INFO>()
      {
        Constant = Mapper.Map<ENDO_WGO_PT_DTO[], List<ENDO_WGO_PT_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<ENDO_WGO_PT_DTO[], List<ENDO_WGO_PT_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<ENDO_WGO_PT_DTO[], List<ENDO_WGO_PT_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<ENDO_WGO_PT_DTO[], List<ENDO_WGO_PT_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<ENDO_WGO_PT_INFO>(this.ID, o);
    }
  }
}