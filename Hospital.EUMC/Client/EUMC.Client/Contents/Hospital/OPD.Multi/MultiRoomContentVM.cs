using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class MultiRoomContentVM : ContentViewModelBase
  {
    public MultiRoomInformation ITEM { get; set; }
    public MultiRoomContentVM(OpdMultiViewConfig config) : base(config)
    {
      this.ITEM = new MultiRoomInformation(config);
    }
    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;

      LOG.dc($"{m.ServiceId}");
      switch (m.ServiceId)
      {
        case SERVICE_ID.OFFICE_PT:
          {
            this.ITEM.Update(m.CastTo<OFFICE_RESP>().Rooms);
            return true;
          }
        case SERVICE_ID.EXAM_PT:
          {
            this.ITEM.Update(m.CastTo<EXAM_RESP>().Rooms);
            return true;
          }
      }
      return false;
    }
  }
}