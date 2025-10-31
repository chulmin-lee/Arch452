using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;

namespace EUMC.HospitalService
{
  internal class ENDO_Service : MessageTransformer
  {
    public ENDO_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.ENDO)
    {
      this.subscribe(DATA_ID.ENDO, DATA_ID.ENDO_WGO);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
   
      ENDO_TYPE type = ENDO_TYPE.None;
      switch (o.ID)
      {
        case DATA_ID.ENDO: type = ENDO_TYPE.Normal; break;
        case DATA_ID.ENDO_WGO: type = ENDO_TYPE.WGO; break;
        default: return null;
      }

      var all = o.All<ENDO_PT_INFO>();
      all.ForEach(x => x.Type = type);
      return new NotifyMessage<ENDO_PT_INFO> { ID = this.ID, Updated = all };
    }
  }
}