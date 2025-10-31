using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class OFFICE_PT_Extractor: DataExtractor<OFFICE_PT_DTO>
  {
    Dictionary<string, List<string>> dept_room_codes = new Dictionary<string, List<string>>();
    public OFFICE_PT_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.OFFICE_PT)
    {
      this.subscribe(DATA_ID.OFFICE_ROOM);
    }
    protected override List<OFFICE_PT_DTO> query() => Repository.OFFICE_PT(dept_room_codes);

    protected override bool data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.OFFICE_ROOM:
          {
            dept_room_codes.Clear();
            var all = o.All<OFFICE_ROOM_POCO>();

            foreach(var g in all.GroupBy(x => x.DeptCode))
            {
              var deptCode = g.Key;
              var roomCodes = g.ToList().Select(x => x.RoomCode).ToList();
              dept_room_codes.Add(deptCode, roomCodes);
            }
            return true;
          }
      }
      return false;
    }

    protected override INotifyData<DATA_ID> data_mapping(UpdateData<OFFICE_PT_DTO> updated)
    {
      var o = new UpdateData<OFFICE_PT_POCO>()
      {
        Constant = Mapper.Map<OFFICE_PT_DTO[], List<OFFICE_PT_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<OFFICE_PT_DTO[], List<OFFICE_PT_POCO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<OFFICE_PT_DTO[], List<OFFICE_PT_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<OFFICE_PT_DTO[], List<OFFICE_PT_POCO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<OFFICE_PT_POCO>(this.ID, o);
    }
  }
}