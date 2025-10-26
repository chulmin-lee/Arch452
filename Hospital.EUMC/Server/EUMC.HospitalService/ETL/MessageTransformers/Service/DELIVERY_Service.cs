using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;

namespace EUMC.HospitalService
{
  internal class DELIVERY_Service : MessageTransformer
  {
    public DELIVERY_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.DELIVERY_ROOM)
    {
      this.subscribe(DATA_ID.DELIVERY_ROOM);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.DELIVERY_ROOM: return delivery_update(o.Data<DELIVERY_INFO, DATA_ID>());
      }
      return null;
    }

    INotifyMessage delivery_update(UpdateData<DELIVERY_INFO> updated)
    {
      return new NotifyMessage<DELIVERY_INFO> { ID = this.ID, Updated = updated.All };
    }
  }
}