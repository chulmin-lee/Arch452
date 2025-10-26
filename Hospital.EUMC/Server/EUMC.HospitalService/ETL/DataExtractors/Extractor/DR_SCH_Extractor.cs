using Common;
using Framework.DataSource;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal class DR_SCH_Extractor : DataExtractor<DR_SCH_DTO>
  {
    public DR_SCH_Extractor(IHospitalMemberOwner owner) : base(owner, DATA_ID.DR_SCH)
    {
    }
    protected override List<DR_SCH_DTO> query() => this.Repository.DR_SCH_DATA();

    protected override INotifyData<DATA_ID> data_mapping(UpdateData<DR_SCH_DTO> updated)
    {
      var doctors = new List<DR_SCH_INFO>();
      // poco로 변환

      var list = Mapper.Map<DR_SCH_DTO[], List<DR_SCH_POCO>>(updated.All.ToArray());

      foreach (var group in list.GroupBy(x => x.DoctorNo).ToList())
      {
        var drno = group.Key;
        var doctor = group.FirstOrDefault();

        if (doctor != null)
        {
          var d = new DR_SCH_INFO()
          {
            DeptCode = doctor.DeptCode,
            DeptName = doctor.DeptName,
            DoctorNo = drno,
            DoctorName = doctor.DoctorName,
            SpecialPart = doctor.SpecialPart,
          };

          foreach (var w in group.ToList())
          {
            switch (w.DayOfWeek)
            {
              case DayOfWeek.Monday:
                if (w.AM) d.MonAM = true;
                if (w.PM) d.MonPM = true;
                break;
              case DayOfWeek.Tuesday:
                if (w.AM) d.TueAM = true;
                if (w.PM) d.TuePM = true;
                break;
              case DayOfWeek.Wednesday:
                if (w.AM) d.WedAM = true;
                if (w.PM) d.WedPM = true;
                break;
              case DayOfWeek.Thursday:
                if (w.AM) d.ThuAM = true;
                if (w.PM) d.ThuPM = true;
                break;
              case DayOfWeek.Friday:
                if (w.AM) d.FriAM = true;
                if (w.PM) d.FriPM = true;
                break;
              case DayOfWeek.Saturday:
                if (w.AM) d.SatAM = true;
                if (w.PM) d.SatPM = true;
                break;
            }
          }

          doctors.Add(d);
        }
      }
      // test
      int dept_order = 0;
      foreach (var dept_group in doctors.GroupBy(x => x.DeptName))
      {
        dept_order++;
        // 순서 설정
        var deptCode = dept_group.Key;
        var doctor_list = dept_group.ToList();

        // test: dept 순서 임의 할당

        doctor_list.ForEach(x => x.DeptOrder = dept_order);

        int doctor_order = 1;
        foreach (var d in doctor_list)
        {
          d.DoctorOrder = doctor_order++;
        }
      }

      var o = new UpdateData<DR_SCH_INFO>()
      {
        Added    = doctors,
      }.Compose();
      return new DataEventData<DR_SCH_INFO>(this.ID, o);
    }
  }
}