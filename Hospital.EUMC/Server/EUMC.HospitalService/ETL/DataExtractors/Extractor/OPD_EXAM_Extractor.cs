using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class OPD_EXAM_Extractor : DataExtractor<EXAM_DTO>
  {
    public OPD_EXAM_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.EXAM_ROOM)
    {
    }
    protected override List<EXAM_DTO> query() => this.Repository.EXAM_DATA();

    protected override INotifyData<DATA_ID> data_mapping(UpdateData<EXAM_DTO> updated)
    {
      var o = new UpdateData<EXAM_POCO>()
      {
        Constant = Mapper.Map<EXAM_DTO[], List<EXAM_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<EXAM_DTO[], List<EXAM_POCO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<EXAM_DTO[], List<EXAM_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<EXAM_DTO[], List<EXAM_POCO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<EXAM_POCO>(this.ID, o);
    }
  }
}