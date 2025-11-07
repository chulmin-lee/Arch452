using EUMC.ClientServices;
using ServiceCommon;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.Client
{
  internal class OfficeSingleInformation
  {
    public OfficeSingleViewConfig.ContentConfig CONFIG { get; set; }
    public SingleRoomViewModel ROOM { get; set; }

    public OfficeSingleInformation(OfficeSingleViewConfig o)
    {
      this.CONFIG = o.Config;
      this.ROOM = new SingleRoomViewModel(o.Config.ItemRows, o.Room, o.WaitMesages);
    }

    public bool Update(OFFICE_RESP o)
    {
      var room = o.Rooms.Where(x => x.GroupKey == this.ROOM.Key).FirstOrDefault();
      if (room != null)
      {
        this.ROOM.Update(room);
      }
      return true;
    }
  }
}