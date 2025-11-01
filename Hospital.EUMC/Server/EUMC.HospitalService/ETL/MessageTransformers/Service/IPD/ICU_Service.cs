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
        case DATA_ID.ICU: return icu_update(o.Data<ICU_INFO>());
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

      var wards = updated.ChangedAll.Select(x => x.IcuCode).Distinct().ToList();

      var groups = updated.All.Where(x => wards.Contains(x.IcuCode)).GroupBy(x => x.IcuCode);
      foreach (var group in groups)
      {
        var ward_code = group.Key;
        var patients = group.ToList();
        var ward = new ICU_PT_INFO()
        {
          IcuCode = patients.First().IcuCode,
          IcuName = patients.First().IcuName,
          Patients = patients,
        };
        items.Add(ward);
      }

      var delete = updated.Deleted.Select(x => x.IcuCode).Distinct().ToList();
      foreach (var ward in delete)
      {
        if (updated.All.Where(x => ward == x.IcuCode).Count() == 0)
        {
          items.Add(new ICU_PT_INFO() { IcuCode = ward });
        }
      }
      return new NotifyMessage<ICU_PT_INFO> { ID = this.ID, Updated = items };
    }
  }
}