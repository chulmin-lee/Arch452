using Framework.DataSource;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class DEPT_MASTER_Extractor : DataExtractor<DEPT_MASTER_DTO>
  {
    public DEPT_MASTER_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.DEPT_MASTER)
    {
    }
    protected override List<DEPT_MASTER_DTO> query() => this.Repository.DEPT_MASTER();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<DEPT_MASTER_DTO> updated)
    {
      var o = new UpdateData<DEPT_MASTER_POCO>()
      {
        Constant = Mapper.Map<DEPT_MASTER_DTO[], List<DEPT_MASTER_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<DEPT_MASTER_DTO[], List<DEPT_MASTER_POCO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<DEPT_MASTER_DTO[], List<DEPT_MASTER_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<DEPT_MASTER_DTO[], List<DEPT_MASTER_POCO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<DEPT_MASTER_POCO>(this.ID, o);
    }
  }
}