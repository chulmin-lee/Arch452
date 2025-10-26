using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ENDO_Extractor : DataExtractor<ENDO_DTO>
  {
    public ENDO_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ENDO)
    {
    }
    protected override List<ENDO_DTO> query() => this.Repository.ENDO_DATA();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ENDO_DTO> updated)
    {
      var o = new UpdateData<ENDO_INFO>()
      {
        Constant = Mapper.Map<ENDO_DTO[], List<ENDO_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<ENDO_DTO[], List<ENDO_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<ENDO_DTO[], List<ENDO_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<ENDO_DTO[], List<ENDO_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<ENDO_INFO>(this.ID, o);
    }
  }
}