using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class OFFICE_ROOM_Extractor : DataExtractor<OFFICE_ROOM_DTO>
  {
    public OFFICE_ROOM_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.OFFICE_ROOM)
    {
    }
    protected override List<OFFICE_ROOM_DTO> query() => this.Repository.OFFICE_ROOM();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<OFFICE_ROOM_DTO> updated)
    {
      var o = new UpdateData<OFFICE_ROOM_POCO>()
      {
        Constant = Mapper.Map<OFFICE_ROOM_DTO[], List<OFFICE_ROOM_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<OFFICE_ROOM_DTO[], List<OFFICE_ROOM_POCO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<OFFICE_ROOM_DTO[], List<OFFICE_ROOM_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<OFFICE_ROOM_DTO[], List<OFFICE_ROOM_POCO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<OFFICE_ROOM_POCO>(this.ID, o);
    }
  }
}