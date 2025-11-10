using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.IO;

namespace EUMC.HospitalService
{
  internal class ANG2_Extractor : DataExtractor<ANG_PT_DTO>
  {
    public ANG2_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.ANG2)
    {
      this.BackupJsonPath = Path.Combine(owner.BackupDataPath, $"ANG2_PT_DTO.json");
    }
    protected override List<ANG_PT_DTO> query() => this.Repository.ANG2_PT();
    protected override INotifyData<DATA_ID> data_mapping(UpdateData<ANG_PT_DTO> updated)
    {
      //모두 전달한다
      var o = new UpdateData<ANG_PT_INFO>()
      {
        Constant = Mapper.Map<ANG_PT_DTO[], List<ANG_PT_INFO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<ANG_PT_DTO[], List<ANG_PT_INFO>>(updated.Updated.ToArray()),
        Deleted  = Mapper.Map<ANG_PT_DTO[], List<ANG_PT_INFO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<ANG_PT_DTO[], List<ANG_PT_INFO>>(updated.Added.ToArray()),
      }.Compose();
      return new DataEventData<ANG_PT_INFO>(this.ID, o);
    }
  }
}