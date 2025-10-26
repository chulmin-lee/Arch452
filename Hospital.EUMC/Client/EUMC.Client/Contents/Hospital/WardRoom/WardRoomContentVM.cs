using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class WardRoomContentVM : ContentViewModelBase
  {
    public WardRoomInformation ITEM { get; set; }
    public WardRoomContentVM(WardRoomViewConfig config) : base(config)
    {
      this.ITEM = new WardRoomInformation(config);
    }
    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;

      LOG.dc($"{m.ServiceId}");
      switch (m.ServiceId)
      {
        case SERVICE_ID.WARD_ROOMS:
          {
            return this.ITEM.Update(m.CastTo<WARD_ROOM_RESP>().Ward);
          }
      }
      return false;
    }
    protected override void ContentClose()
    {
      this.ITEM.Close();
    }
  }
}