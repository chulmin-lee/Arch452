using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_ISOLATION_Service : MessageTransformer
  {
    public ER_ISOLATION_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.ER_ISOLATION)
    {
      this.subscribe(DATA_ID.ER_ISOLATION);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.ER_ISOLATION: return er_isolation_updated(o.Data<ER_ISOLATION_POCO, DATA_ID>());
      }
      return null;
    }

    private INotifyMessage er_isolation_updated(UpdateData<ER_ISOLATION_POCO> updated)
    {
      var items = new List<ER_ISOLATION_INFO>();

      // 변경이 발생한 room code
      var roomCodes = updated.ChangedAll.Select(x => x.RoomCode).Distinct().ToList();

      foreach (var roomCode in roomCodes)
      {
        var patients = updated.All.Where(x => x.RoomCode == roomCode).ToList();

        if (patients.Count == 0)
        {
          var room = updated.Deleted.Where(x => x.RoomCode == roomCode).First();
          items.Add(new ER_ISOLATION_INFO
          {
            RoomCode = room.RoomCode,
            RoomName = room.RoomName,
          });
        }
        else
        {
          var room = patients.First();
          items.Add(new ER_ISOLATION_INFO
          {
            RoomCode = room.RoomCode,
            RoomName = room.RoomName,
            Patient = Mapper.Map<ER_ISOLATION_POCO, EMERGENCY_INFO>(room),
          });
        }
      }

      return new NotifyMessage<ER_ISOLATION_INFO> { ID = this.ID, Updated = items };
    }
  }
}