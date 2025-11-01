using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class EXAM_PT_Extractor : DataExtractor<EXAM_PT_DTO>
  {
    List<string> exam_dept_codes = new List<string>();
    public EXAM_PT_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.EXAM_PT)
    {
    }
    protected override List<EXAM_PT_DTO> query() => this.Repository.EXAM_PT(exam_dept_codes);
    protected override bool data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.EXAM_DEPT:
          {
            this.exam_dept_codes.Clear();
            var dept_codes = o.All<EXAM_DEPT_POCO>().Select(x => x.DeptCode);
            this.exam_dept_codes.AddRange(dept_codes);
            return true;
          }
      }
      return false;
    }
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<EXAM_PT_DTO> updated)
    {
      var o = new UpdateData<EXAM_PT_POCO>()
      {
        Constant = Mapper.Map<EXAM_PT_DTO[], List<EXAM_PT_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<EXAM_PT_DTO[], List<EXAM_PT_POCO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<EXAM_PT_DTO[], List<EXAM_PT_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<EXAM_PT_DTO[], List<EXAM_PT_POCO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<EXAM_PT_POCO>(this.ID, o);
    }
  }
}