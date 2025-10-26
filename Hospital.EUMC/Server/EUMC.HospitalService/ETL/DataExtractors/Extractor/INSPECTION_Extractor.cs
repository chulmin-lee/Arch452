using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class INSPECTION_Extractor : DataExtractor<INSPECTION_DTO>
  {
    public INSPECTION_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.INSPECTION)
    {
    }
    protected override List<INSPECTION_DTO> query() => this.Repository.INSPECTION_DATA();

    protected override INotifyData<DATA_ID> data_mapping(UpdateData<INSPECTION_DTO> updated)
    {
      var o = new UpdateData<INSPECTION_INFO>()
      {
        Constant = Mapper.Map<INSPECTION_DTO[], List<INSPECTION_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<INSPECTION_DTO[], List<INSPECTION_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<INSPECTION_DTO[], List<INSPECTION_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<INSPECTION_DTO[], List<INSPECTION_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<INSPECTION_INFO>(this.ID, o);
    }
  }
}