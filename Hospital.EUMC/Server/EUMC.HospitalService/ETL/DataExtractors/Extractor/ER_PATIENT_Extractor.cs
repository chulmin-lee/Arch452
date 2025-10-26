using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_PATIENT_Extractor : DataExtractor<ER_PATIENT_DTO>
  {
    public ER_PATIENT_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ER_PATIENT)
    {
    }
    protected override List<ER_PATIENT_DTO> query() => this.Repository.EMERGENCY_DATA();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ER_PATIENT_DTO> updated)
    {
      var o = new UpdateData<EMERGENCY_INFO>()
      {
        Constant = Mapper.Map<ER_PATIENT_DTO[], List<EMERGENCY_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<ER_PATIENT_DTO[], List<EMERGENCY_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<ER_PATIENT_DTO[], List<EMERGENCY_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<ER_PATIENT_DTO[], List<EMERGENCY_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<EMERGENCY_INFO>(this.ID, o);
    }
  }
}