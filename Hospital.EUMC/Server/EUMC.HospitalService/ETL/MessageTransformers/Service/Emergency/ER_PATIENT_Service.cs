using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_PATIENT_Service : MessageTransformer
  {
    public ER_PATIENT_Service(IHospitalMemberOwner owner) : base(owner, SERVICE_ID.ER_PATIENT)
    {
      this.subscribe(DATA_ID.ER_PATIENT);
    }

    protected override INotifyMessage data_notified(INotifyData<DATA_ID> o)
    {
      switch (o.ID)
      {
        case DATA_ID.ER_PATIENT: return emergency_update(o.Data<ER_PATIENT_INFO, DATA_ID>());
      }
      return null;
    }
    /// <summary>
    /// 변경된 ER_TYPE 데이터만 통지한다
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    INotifyMessage emergency_update(UpdateData<ER_PATIENT_INFO> updated)
    {
      var items = new List<ER_PATIENT_GROUP>();
      // 성인/소아 변경 사항 구분
      var types = updated.ChangedAll.Select(x => x.IsChild).Distinct().ToList();

      foreach (var type in types)
      {
        var type_items = updated.All.Where(x => x.IsChild == type).ToList();
        items.Add(new ER_PATIENT_GROUP
        {
          IsChild = type,
          Patients = type_items
        });
      }

      return new NotifyMessage<ER_PATIENT_GROUP> { ID = this.ID, Updated = items };
    }
  }
}