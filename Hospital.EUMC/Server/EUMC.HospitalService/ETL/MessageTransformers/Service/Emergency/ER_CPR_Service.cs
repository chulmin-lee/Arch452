using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_CPR_Service : MessageTransformer
  {
    public ER_CPR_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.ER_CPR)
    {
      this.subscribe(DATA_ID.ER_CPR);
    }

    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.ER_CPR: return er_cpr_update(o.Data<ER_CPR_DTO>());
      }
      return null;
    }
    /// <summary>
    /// 변경된 ER_TYPE 데이터만 통지한다
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    INotifyMessage er_cpr_update(UpdateData<ER_CPR_DTO> updated)
    {
      var cpr = updated.All.First().ER_CPR_STATE == "Y";
      return new NotifyMessage<bool> { ID = this.ID, Updated = new List<bool> { cpr } };
    }
  }
}