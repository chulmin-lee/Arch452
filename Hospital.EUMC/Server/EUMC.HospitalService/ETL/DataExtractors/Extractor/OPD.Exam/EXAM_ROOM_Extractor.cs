using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class EXAM_ROOM_Extractor : DataExtractor<EXAM_ROOM_DTO>
  {
    List<string> exam_dept_codes = new List<string>();
    public EXAM_ROOM_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.EXAM_ROOM)
    {
      this.subscribe(DATA_ID.EXAM_DEPT);
    }
    protected override List<EXAM_ROOM_DTO> query() => this.Repository.EXAM_ROOM(exam_dept_codes);
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
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<EXAM_ROOM_DTO> updated)
    {
      var o = new UpdateData<EXAM_ROOM_POCO>()
      {
        Constant = Mapper.Map<EXAM_ROOM_DTO[], List<EXAM_ROOM_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<EXAM_ROOM_DTO[], List<EXAM_ROOM_POCO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<EXAM_ROOM_DTO[], List<EXAM_ROOM_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<EXAM_ROOM_DTO[], List<EXAM_ROOM_POCO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<EXAM_ROOM_POCO>(this.ID, o);
    }
  }
}