using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_CONGESTION_Extractor : DataExtractor<ER_CONGESTION_DTO>
  {
    public ER_CONGESTION_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ER_CONGESTION)
    {
    }
    protected override List<ER_CONGESTION_DTO> query() => this.Repository.ER_CONGESTION_DATA();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ER_CONGESTION_DTO> updated)
    {
      var o = new UpdateData<ER_CONGESTION_INFO>()
      {
        Constant = Mapper.Map<ER_CONGESTION_DTO[], List<ER_CONGESTION_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<ER_CONGESTION_DTO[], List<ER_CONGESTION_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<ER_CONGESTION_DTO[], List<ER_CONGESTION_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<ER_CONGESTION_DTO[], List<ER_CONGESTION_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<ER_CONGESTION_INFO>(this.ID, o);
    }
  }
}