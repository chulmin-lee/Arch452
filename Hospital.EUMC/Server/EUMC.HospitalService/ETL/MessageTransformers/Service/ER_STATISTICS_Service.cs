using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_STATISTICS_Service : MessageTransformer
  {
    public ER_STATISTICS_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.ER_STATISTICS)
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
      var items = new List<ER_STATISTIC_INFO>();
      //child
      {
        var child = updated.All.Where(x => x.IsChild == true);
        items.Add(new ER_STATISTIC_INFO
        {
          IsChild = true,
          PatientCount = child.Select(x => x.InBedCount).Sum(),
          AverageInTime = (int)child.Select(x => x.Percent).Average(),
        });
      }

      // adult
      {
        var adult = updated.All.Where(x => x.IsChild == false);
        items.Add(new ER_STATISTIC_INFO
        {
          IsChild = false,
          PatientCount = adult.Select(x => x.InBedCount).Sum(),
          AverageInTime = (int)adult.Select(x => x.Percent).Average(),
        });
      }
      return new NotifyMessage<ER_STATISTIC_INFO> { ID = this.ID, Updated = items };
    }
  }
}