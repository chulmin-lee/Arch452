using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class SingleRoomInformation
  {
    public SingleRoomVM ROOM { get; set; }
    public string Key => this.ROOM.Key;
    public SingleRoomInformation(OpdSingleViewConfig o)
    {
      this.ROOM = new SingleRoomVM(o);
    }
    public void Update(OPD_ROOM_INFO room)
    {
      this.ROOM.Update(room);
    }
  }
}