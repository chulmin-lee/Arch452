using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;

namespace EUMC.HospitalService
{
  internal class INSPECTION_Service : MessageTransformer
  {
    // 환자로 필터링
    public INSPECTION_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.INSPECTION)
    {
      this.subscribe(DATA_ID.INSPECTION);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.INSPECTION: return inspection_update(o.Data<INSPECTION_INFO, DATA_ID>());
      }
      return null;
    }

    INotifyMessage inspection_update(UpdateData<INSPECTION_INFO> updated)
    {
      return new NotifyMessage<INSPECTION_INFO> { ID = this.ID, Updated = updated.All };
    }
  }
}