using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class ICU_Extractor : DataExtractor<ICU_DTO>
  {
    public ICU_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ICU)
    {
    }
    protected override List<ICU_DTO> query() => this.Repository.ICU_DATA();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ICU_DTO> updated)
    {
      var o = new UpdateData<ICU_INFO>()
      {
        Constant = Mapper.Map<ICU_DTO[], List<ICU_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<ICU_DTO[], List<ICU_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<ICU_DTO[], List<ICU_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<ICU_DTO[], List<ICU_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<ICU_INFO>(this.ID, o);
    }
  }
}