using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class DRUG_Extractor : DataExtractor<DRUG_DTO>
  {
    public DRUG_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.DRUG)
    {
    }
    protected override List<DRUG_DTO> query() => this.Repository.DRUG();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<DRUG_DTO> updated)
    {
      //모두 전달한다
      var o = new UpdateData<DRUG_INFO>()
      {
        Constant = Mapper.Map<DRUG_DTO[], List<DRUG_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<DRUG_DTO[], List<DRUG_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<DRUG_DTO[], List<DRUG_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<DRUG_DTO[], List<DRUG_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<DRUG_INFO>(this.ID, o);
    }
  }
}