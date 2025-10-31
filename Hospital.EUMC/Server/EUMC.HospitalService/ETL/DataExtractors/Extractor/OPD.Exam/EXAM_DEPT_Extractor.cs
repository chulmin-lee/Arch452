using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class EXAM_DEPT_Extractor : DataExtractor<EXAM_DEPT_DTO>
  {
    public EXAM_DEPT_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.EXAM_DEPT)
    {
    }
    protected override List<EXAM_DEPT_DTO> query() => this.Repository.EXAM_DEPT();

    protected override INotifyData<DATA_ID> data_mapping(UpdateData<EXAM_DEPT_DTO> updated)
    {
      var o = new UpdateData<EXAM_DEPT_POCO>()
      {
        Constant = Mapper.Map<EXAM_DEPT_DTO[], List<EXAM_DEPT_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<EXAM_DEPT_DTO[], List<EXAM_DEPT_POCO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<EXAM_DEPT_DTO[], List<EXAM_DEPT_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<EXAM_DEPT_DTO[], List<EXAM_DEPT_POCO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<EXAM_DEPT_POCO>(this.ID, o);
    }
  }
}