using Common;
using Framework.DataSource;
using Framework.Network.HTTP;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EUMC.HospitalService
{
  internal class WARD_Extractor : DataExtractor<WARD_DTO>
  {
    public WARD_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.WARD_ROOMS)
    {
    }
    protected override List<WARD_DTO> query() => this.Repository.WARD_DATA();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<WARD_DTO> updated)
    {
      var o = new UpdateData<WARD_POCO>()
      {
        Constant = Mapper.Map<WARD_DTO[], List<WARD_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<WARD_DTO[], List<WARD_POCO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<WARD_DTO[], List<WARD_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<WARD_DTO[], List<WARD_POCO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<WARD_POCO>(this.ID, o);
    }
  }
}