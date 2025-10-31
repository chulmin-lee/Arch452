using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class ENDO_Extractor : DataExtractor<ENDO_PT_DTO>
  {
    public ENDO_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ENDO)
    {
    }
    protected override List<ENDO_PT_DTO> query() => this.Repository.ENDO_PT();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ENDO_PT_DTO> updated)
    {
      var o = new UpdateData<ENDO_PT_INFO>()
      {
        Constant = Mapper.Map<ENDO_PT_DTO[], List<ENDO_PT_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<ENDO_PT_DTO[], List<ENDO_PT_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<ENDO_PT_DTO[], List<ENDO_PT_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<ENDO_PT_DTO[], List<ENDO_PT_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<ENDO_PT_INFO>(this.ID, o);
    }
  }
}