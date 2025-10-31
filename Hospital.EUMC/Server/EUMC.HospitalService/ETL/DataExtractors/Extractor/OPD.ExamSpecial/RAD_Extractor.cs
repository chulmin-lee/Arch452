using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class RAD_Extractor : DataExtractor<RAD_PT_DTO>
  {
    List<string> exam_room_codes = new List<string>();
    public RAD_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.RAD)
    {
    }
    protected override List<RAD_PT_DTO> query() => this.Repository.RAD_PT(exam_room_codes);
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<RAD_PT_DTO> updated)
    {
      var o = new UpdateData<RAD_PT_INFO>()
      {
        Constant = Mapper.Map<RAD_PT_DTO[], List<RAD_PT_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<RAD_PT_DTO[], List<RAD_PT_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<RAD_PT_DTO[], List<RAD_PT_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<RAD_PT_DTO[], List<RAD_PT_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<RAD_PT_INFO>(this.ID, o);
    }
  }
}