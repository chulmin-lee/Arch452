using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class WARD_Service : MessageTransformer
  {
    public WARD_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.WARD_ROOMS)
    {
      this.subscribe(DATA_ID.WARD_ROOMS);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.WARD_ROOMS: return ward_update(o.Data<WARD_POCO, DATA_ID>());
      }
      return null;
    }
    INotifyMessage ward_update(UpdateData<WARD_POCO> updated)
    {
      // 데이타가 변경된 층과 구역 목록
      // 진료실/검사실의 경우 DeptCode와 roomCode 는 unique한 조합이지만,
      // Ward의 경우, Floor + Area 내부에는 여러 ward가 존재한다.

      var dic = new Dictionary<string, AREA_WARD_INFO>(); // key: floor:area
      foreach (var group in updated.ChangedAll.GroupBy(x => new { x.Floor, x.AreaCode, x.RoomCode }))
      {
        var floor = group.Key.Floor;
        var area = group.Key.AreaCode;
        var key = $"{floor}:{area}";

        if (!dic.TryGetValue(key, out var floor_area))
        {
          floor_area = new AREA_WARD_INFO
          {
            Floor = floor,
            AreaCode = area
          };
          dic.Add(key, floor_area);
        }

        var roomCode = group.Key.RoomCode;

        // all 에서 검사
        var patients = updated.All.Where(x => x.IsMatch(floor, area, roomCode)).ToList();

        if (patients.Any())
        {
          var ward = Mapper.Map<WARD_POCO, AREA_WARD_INFO.AREA_WARD>(patients.First());
          ward.Patients.AddRange(Mapper.Map<WARD_POCO[], List<AREA_WARD_INFO.WARD_PATIENT>>(patients.ToArray()).ToList());
          floor_area.AreaWards.Add(ward);
        }
        else
        {
          // 빈 병실 생성
          var xxx = updated.Deleted.Where(x => x.IsMatch(floor, area, roomCode)).First();

          var ward = Mapper.Map<WARD_POCO, AREA_WARD_INFO.AREA_WARD>(xxx);
          floor_area.AreaWards.Add(ward);
        }
      }
      return new NotifyMessage<AREA_WARD_INFO> { ID = this.ID, Updated = dic.Values.ToList() };
    }
  }
}