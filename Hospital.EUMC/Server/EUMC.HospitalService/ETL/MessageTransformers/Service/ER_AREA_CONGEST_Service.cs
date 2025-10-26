using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_AREA_CONGEST_Service : MessageTransformer
  {
    public ER_AREA_CONGEST_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.ER_AREA_CONGEST)
    {
      this.subscribe(DATA_ID.ER_AREA_CONGEST);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.ER_AREA_CONGEST: return congest_update(o.Data<ER_AREA_CONGEST_INFO, DATA_ID>());
      }
      return null;
    }
    /// <summary>
    /// 모두 새로 만든다
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    INotifyMessage congest_update(UpdateData<ER_AREA_CONGEST_INFO> updated)
    {
      var items = new List<ER_AREA_CONGEST_GROUP>();

      var types = updated.ChangedAll.Select(x => x.IsChild).Distinct().ToList();
      foreach (var type in types)
      {
        var type_items = updated.All.Where(x => x.IsChild == type).ToList();
        items.Add(new ER_AREA_CONGEST_GROUP
        {
          IsChild = type,
          AreaCongests = type_items
        });
      }

      return new NotifyMessage<ER_AREA_CONGEST_GROUP> { ID = this.ID, Updated = items };
    }
  }
}