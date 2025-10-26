using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class DELIVERY_Extractor : DataExtractor<DELIVERY_ROOM_DTO>
  {
    public DELIVERY_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.DELIVERY_ROOM)
    {
    }
    protected override List<DELIVERY_ROOM_DTO> query() => this.Repository.DELIVERY_ROOM_DATA();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<DELIVERY_ROOM_DTO> updated)
    {
      var o = new UpdateData<DELIVERY_INFO>()
      {
        Constant = Mapper.Map<DELIVERY_ROOM_DTO[], List<DELIVERY_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<DELIVERY_ROOM_DTO[], List<DELIVERY_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<DELIVERY_ROOM_DTO[], List<DELIVERY_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<DELIVERY_ROOM_DTO[], List<DELIVERY_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<DELIVERY_INFO>(this.ID, o);
    }
  }
}