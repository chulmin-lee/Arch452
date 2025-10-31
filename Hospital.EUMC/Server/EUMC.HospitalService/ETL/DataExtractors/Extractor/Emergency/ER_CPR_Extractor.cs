using Framework.DataSource;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class ER_CPR_Extractor : DataExtractor<ER_CPR_DTO>
  {
    public ER_CPR_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ER_CPR)
    {
    }
    protected override List<ER_CPR_DTO> query() => this.Repository.ER_CPR();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ER_CPR_DTO> updated)
    {
      return new DataEventData<ER_CPR_DTO>(this.ID, updated);
    }
  }
}