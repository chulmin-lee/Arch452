using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ER_CT_Extractor : DataExtractor<ER_CT_DTO>
  {
    public ER_CT_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ER_CT)
    {
    }
    protected override List<ER_CT_DTO> query() => this.Repository.ER_CT();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ER_CT_DTO> updated)
    {
      var o = new UpdateData<ER_CT_INFO>()
      {
        Constant = Mapper.Map<ER_CT_DTO[], List<ER_CT_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<ER_CT_DTO[], List<ER_CT_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<ER_CT_DTO[], List<ER_CT_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<ER_CT_DTO[], List<ER_CT_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<ER_CT_INFO>(this.ID, o);
    }
  }
}