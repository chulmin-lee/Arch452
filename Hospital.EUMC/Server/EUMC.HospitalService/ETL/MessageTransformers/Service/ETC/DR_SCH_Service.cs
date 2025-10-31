using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;

namespace EUMC.HospitalService
{
  internal class DR_SCH_Service : MessageTransformer
  {
    public DR_SCH_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.DR_SCH)
    {
      this.subscribe(DATA_ID.DR_SCH);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.DR_SCH: return data_update(o.Data<DR_SCH_INFO, DATA_ID>());
      }
      return null;
    }
    /// <summary>
    /// 모두 전달한다
    /// </summary>
    INotifyMessage data_update(UpdateData<DR_SCH_INFO> updated)
    {
      return new NotifyMessage<DR_SCH_INFO> { ID = this.ID, Updated = updated.All };
    }
  }
}