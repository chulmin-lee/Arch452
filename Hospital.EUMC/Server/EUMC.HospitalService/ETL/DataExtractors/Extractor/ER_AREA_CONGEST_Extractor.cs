using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_AREA_CONGEST_Extractor : DataExtractor<ER_AREA_CONGEST_DTO>
  {
    public ER_AREA_CONGEST_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ER_AREA_CONGEST)
    {
    }
    protected override List<ER_AREA_CONGEST_DTO> query() => this.Repository.ER_AREA_CONGEST_DATA();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ER_AREA_CONGEST_DTO> updated)
    {
      var o = new UpdateData<ER_AREA_CONGEST_INFO>()
      {
        Constant = Mapper.Map<ER_AREA_CONGEST_DTO[], List<ER_AREA_CONGEST_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<ER_AREA_CONGEST_DTO[], List<ER_AREA_CONGEST_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<ER_AREA_CONGEST_DTO[], List<ER_AREA_CONGEST_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<ER_AREA_CONGEST_DTO[], List<ER_AREA_CONGEST_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<ER_AREA_CONGEST_INFO>(this.ID, o);
    }
  }
}