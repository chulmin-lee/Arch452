using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;

namespace EUMC.HospitalService
{
  internal class ENDO_Service : MessageTransformer
  {
    public ENDO_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.ENDO)
    {
      this.subscribe(DATA_ID.ENDO);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.ENDO: return endo_update(o.Data<ENDO_INFO, DATA_ID>());
      }
      return null;
    }
    /// <summary>
    /// 모두 전달한다
    /// </summary>
    INotifyMessage endo_update(UpdateData<ENDO_INFO> updated)
    {
      return new NotifyMessage<ENDO_INFO> { ID = this.ID, Updated = updated.All };
    }
  }
}