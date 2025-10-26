using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_ISOLATION_Extractor : DataExtractor<ER_ISOLATION_DTO>
  {
    public ER_ISOLATION_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ER_ISOLATION)
    {
    }
    protected override List<ER_ISOLATION_DTO> query() => this.Repository.ER_ISOLATION_DATA();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ER_ISOLATION_DTO> updated)
    {
      var o = new UpdateData<ER_ISOLATION_POCO>()
      {
        Constant = Mapper.Map<ER_ISOLATION_DTO[], List<ER_ISOLATION_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<ER_ISOLATION_DTO[], List<ER_ISOLATION_POCO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<ER_ISOLATION_DTO[], List<ER_ISOLATION_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<ER_ISOLATION_DTO[], List<ER_ISOLATION_POCO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<ER_ISOLATION_POCO>(this.ID, o);
    }
  }
}