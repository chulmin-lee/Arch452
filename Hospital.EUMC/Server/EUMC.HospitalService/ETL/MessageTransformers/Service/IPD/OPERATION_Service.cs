using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class OPERATION_Service : MessageTransformer
  {
    public OPERATION_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.OPERATION)
    {
      this.subscribe(DATA_ID.OPERATION);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.OPERATION: return operation_update(o.Data<OPERATION_INFO>());
      }
      return null;
    }
    /// <summary>
    /// 모두 새로 만든다
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    INotifyMessage operation_update(UpdateData<OPERATION_INFO> updated)
    {
      return new NotifyMessage<OPERATION_INFO> { ID = this.ID, Updated = updated.All };
    }

    internal class EventData : INotifyMessage
    {
      public SERVICE_ID ID => SERVICE_ID.OPERATION;
      public List<OPERATION_INFO> Datas;
      public EventData(List<OPERATION_INFO> o) => this.Datas = o;
    }
  }
}