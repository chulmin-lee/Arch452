using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{

  internal class OPD_OFFICE_Extractor : DataExtractor<OFFICE_DTO>
  {
    public OPD_OFFICE_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.OFFICE_ROOM)
    {
    }
    protected override List<OFFICE_DTO> query() => Repository.OFFICE_DATA();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<OFFICE_DTO> updated)
    {
      var o = new UpdateData<OFFICE_POCO>()
      {
        Constant = Mapper.Map<OFFICE_DTO[], List<OFFICE_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<OFFICE_DTO[], List<OFFICE_POCO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<OFFICE_DTO[], List<OFFICE_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<OFFICE_DTO[], List<OFFICE_POCO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<OFFICE_POCO>(this.ID, o);
    }
  }
}