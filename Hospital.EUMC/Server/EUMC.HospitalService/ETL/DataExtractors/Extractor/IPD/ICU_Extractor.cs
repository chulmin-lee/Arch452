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
    Config _config;
    List<string> icu_codes = new List<string>();
    public ICU_Extractor(IHospitalMemberOwner owner, Config config) : base(owner, DATA_ID.ICU)
    {
      _config = config;
      this.subscribe(DATA_ID.DEPT_MASTER);
    }
    protected override List<ICU_DTO> query() => this.Repository.ICU(icu_codes);
    protected override bool data_notified(INotifyData<DATA_ID> o)
    {
      switch(o.ID)
      {
        case DATA_ID.DEPT_MASTER:
          {
            this.icu_codes.Clear();
            var dept_codes = o.All<DEPT_MASTER_POCO>()
                              .Where(x => _config.IcuDeptNames.Any(c => x.DeptName.Contains(c)))
                              .Select(x => x.DeptCode);
            this.icu_codes.AddRange(dept_codes);
            return true;
          }
      }
      return false;
    }

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

    internal class Config : DataConfig
    {
      // 중환자실이 있는 부서명
      public List<string> IcuDeptNames { get; set; }
      public Config() : base(DATA_ID.ICU)
      {
      }
    }
  }
}