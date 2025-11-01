using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_CONGESTION_Service : MessageTransformer
  {
    public ER_CONGESTION_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.ER_CONGESTION)
    {
      this.subscribe(DATA_ID.ER_CONGESTION);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.ER_CONGESTION: return congest_update(o.Data<ER_CONGESTION_INFO>());
      }
      return null;
    }
    /// <summary>
    /// 모두 새로 만든다
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    INotifyMessage congest_update(UpdateData<ER_CONGESTION_INFO> updated)
    {
      var items = new List<ER_CONGESTION_INFO>();

      var types = updated.ChangedAll.Select(x => x.IsChild).Distinct().ToList();
      foreach (var type in types)
      {
        var item = updated.All.Where(x => x.IsChild == type).FirstOrDefault();
        if (item != null) items.Add(item);
      }
      return new NotifyMessage<ER_CONGESTION_INFO> { ID = this.ID, Updated = items };
    }
  }
}