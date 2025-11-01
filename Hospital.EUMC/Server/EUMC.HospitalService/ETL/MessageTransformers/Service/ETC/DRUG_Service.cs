using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;

namespace EUMC.HospitalService
{
  internal class DRUG_Service : MessageTransformer
  {
    public DRUG_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.DRUG)
    {
      this.subscribe(DATA_ID.DRUG);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.DRUG: return drug_update(o.Data<DRUG_INFO>());
      }
      return null;
    }
    /// <summary>
    /// 모두 전달한다
    /// </summary>
    INotifyMessage drug_update(UpdateData<DRUG_INFO> updated)
    {
      return new NotifyMessage<DRUG_INFO> { ID = this.ID, Updated = updated.All };
    }
  }
}