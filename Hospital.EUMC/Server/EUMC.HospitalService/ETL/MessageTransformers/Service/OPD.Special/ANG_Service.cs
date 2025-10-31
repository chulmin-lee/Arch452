using ServiceCommon;
using ServiceCommon.ServerServices;

namespace EUMC.HospitalService
{
  internal class ANG_Service : MessageTransformer
  {
    public ANG_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.ANG)
    {
      this.subscribe(DATA_ID.ANG, DATA_ID.ANG2, DATA_ID.ANG_IMC);
    }

    /// <summary>
    /// 모두 전달한다
    /// </summary>
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      ANG_TYPE type = ANG_TYPE.None;
      switch (o.ID)
      {
        case DATA_ID.ANG: type = ANG_TYPE.Angiography;  break;
        case DATA_ID.ANG2: type = ANG_TYPE.Angiography_3F;  break;
        case DATA_ID.ANG_IMC: type = ANG_TYPE.IMC; break;
        default: return null;
      }
      var all = o.All<ANG_PT_INFO>();
      all.ForEach(x => x.Type = type);
      return new NotifyMessage<ANG_PT_INFO> { ID = this.ID, Updated = all };
    }
  }
}
