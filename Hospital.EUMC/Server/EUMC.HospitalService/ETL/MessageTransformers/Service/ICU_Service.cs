using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ICU_Service : MessageTransformer
  {
    public ICU_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.ICU)
    {
      this.subscribe(DATA_ID.ICU);
    }
    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.ICU: return icu_update(o.Data<ICU_INFO, DATA_ID>());
      }
      return null;
    }
    /// <summary>
    /// IcuCode 별로 데이타를 만든다
    /// 변경된것과 그렇지 않은것을 구분한 데이타를 만든다
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    INotifyMessage icu_update(UpdateData<ICU_INFO> updated)
    {
      var items = new List<ICU_PT_INFO>();

      var wards = updated.ChangedAll.Select(x => x.WardCode).Distinct().ToList();

      var groups = updated.All.Where(x => wards.Contains(x.WardCode)).GroupBy(x => x.WardCode);
      foreach (var group in groups)
      {
        var ward_code = group.Key;
        var patients = group.ToList();
        var ward = new ICU_PT_INFO()
        {
          WardCode = patients.First().WardCode,
          WardName = patients.First().WardName,
          Patients = patients,
        };
        items.Add(ward);
      }

      var delete = updated.Deleted.Select(x => x.WardCode).Distinct().ToList();
      foreach (var ward in delete)
      {
        if (updated.All.Where(x => ward == x.WardCode).Count() == 0)
        {
          items.Add(new ICU_PT_INFO() { WardCode = ward });
        }
      }
      return new NotifyMessage<ICU_PT_INFO> { ID = this.ID, Updated = items };
    }
  }
}