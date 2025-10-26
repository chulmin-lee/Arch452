using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class DeliveryContentVM : ContentViewModelBase
  {
    public DeliveryInformation ITEM { get; set; }
    public DeliveryContentVM(DeliveryRoomViewConfig config) : base(config)
    {
      this.ITEM = new DeliveryInformation(config);
    }
    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;
      switch (m.ServiceId)
      {
        case SERVICE_ID.DELIVERY_ROOM:
          {
            return this.ITEM.Update(m.CastTo<DELIVERY_ROOM_RESP>().Patients);
          }
        case SERVICE_ID.SVR_CALL_PATIENT:
          {
            return this.ITEM.CallPatient(m.CastTo<CALL_PATIENT_NOTI>());
          }
      }
      return false;
    }
    protected override void ContentClose()
    {
      LOG.ic($"{this.Config.Package}");
      this.ITEM.Close();
    }
  }
}