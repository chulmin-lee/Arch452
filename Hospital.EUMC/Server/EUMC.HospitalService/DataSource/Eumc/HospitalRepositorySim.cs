using Common;
using Framework.DataSource;
using ServiceCommon;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;

namespace EUMC.HospitalService
{
  public class HospitalRepositorySim : IEumcRepository
  {
    string _sim_dir;
    public HospitalRepositorySim(string dir_name = "SIM")
    {
      _sim_dir = dir_name;
    }

    List<T> LoadData<T>(string name = null) where T : class // OriginDataModel
    {
      if (string.IsNullOrEmpty(name))
        name = typeof(T).Name;

      var path = Path.Combine(_sim_dir, $"{name}.json");
      if (File.Exists(path))
      {
        var list = NewtonJson.Load<List<T>>(path);
        if (list != null)
        {
          return list;
        }
      }
      LOG.w($"Simulation> {name}.json not found");
      return new List<T>();
    }
    #region base
    public List<DEPT_MASTER_DTO> DEPT_MASTER() => this.LoadData<DEPT_MASTER_DTO>();

    #endregion

    #region Emergency
    public List<ER_PATIENT_DTO> ER_PATIENT()
    {
      //var area_codes = this.LoadData<ER_AREA_DTO>().Select(x => x.LWR_CTG_NM);
      return LoadData<ER_PATIENT_DTO>();
    }
    public List<ER_CONGESTION_DTO> ER_CONGESTION() => LoadData<ER_CONGESTION_DTO>();
    public List<ER_CPR_DTO> ER_CPR() => LoadData<ER_CPR_DTO>();
    public List<ER_CT_DTO> ER_CT() => this.LoadData<ER_CT_DTO>();
    #endregion

    #region IPD
    public List<ICU_DTO> ICU(List<string> icu_dept_codes) => LoadData<ICU_DTO>();
    public List<OPERATION_DTO> OPERATION()
    {
      var patients = new List<OPERATION_DTO>();
      // 1: 대기중
      {
        var waiting = this.LoadData<OP_WAIT_PT_DTO>();
        waiting.ForEach(x => patients.Add(new OPERATION_DTO { PT_NO = x.PT_NO, PT_NM = x.PT_NM, StateCode = "1" }));
      }
      //, 2: 수술중
      {
        var op = LoadData<OPERATION_DTO>();
        patients.ForEach(x => x.StateCode = "2");
      }

      // [수술종료] 3:회복실, 4:병실, 5:중환자실
      {
        var end = LoadData<OP_END_PT_DTO>();

        foreach (var p in end)
        {
          var o = new OPERATION_DTO { PT_NO = p.PT_NO, PT_NM = p.PT_NM, };
          switch (p.PT_PSTN_CD)
          {
            case "회복실": o.StateCode = "3"; break;
            case "병실": o.StateCode = "4"; break;
            case "중환자실": o.StateCode = "5"; break;
          }
          patients.Add(o);
        }
      }
      return patients;
    }
    #endregion

    #region Office
    public List<OFFICE_ROOM_MASTER_DTO> OFFICE_ROOM_MASTER() => this.LoadData<OFFICE_ROOM_MASTER_DTO>();
    public List<OFFICE_ROOM_DTO> OFFICE_ROOM() => this.LoadData<OFFICE_ROOM_DTO>();
    public List<OFFICE_PT_DTO> OFFICE_PT(Dictionary<string, List<string>> dept_room_no) => this.LoadData<OFFICE_PT_DTO>();
    public List<DR_PHOTO_DTO> DR_PHOTO() => this.LoadData<DR_PHOTO_DTO>();
    #endregion

    #region EXAM_Common
    public List<EXAM_DEPT_DTO> EXAM_DEPT() => this.LoadData<EXAM_DEPT_DTO>();
    public List<EXAM_ROOM_DTO> EXAM_ROOM(List<string> exam_dept_cd) => this.LoadData<EXAM_ROOM_DTO>();
    public List<EXAM_STAFF_DTO> EXAM_STAFF(List<string> exam_dept_cd) => this.LoadData<EXAM_STAFF_DTO>();
    public List<EXAM_PT_DTO> EXAM_PT(List<string> exam_dept_cd) => this.LoadData<EXAM_PT_DTO>();
    #endregion

    #region EXAM Special
    public List<ANG_PT_DTO> ANG_PT() => this.LoadData<ANG_PT_DTO>();
    public List<ANG_PT_DTO> ANG2_PT() => this.LoadData<ANG_PT_DTO>("ANG_PT_DTO2");
    public List<ANG_PT_DTO> ANG_IMC_PT() => this.LoadData<ANG_PT_DTO>("ANG_IMC_DTO");
    public List<ENDO_PT_DTO> ENDO_PT() => this.LoadData<ENDO_PT_DTO>();
    public List<RAD_PT_DTO> RAD_PT(List<string> exam_room_codes) => this.LoadData<RAD_PT_DTO>();
    public List<RAD_TR_PT_DTO> RAD_TR_PT() => this.LoadData<RAD_TR_PT_DTO>();
    public List<ENDO_PT_DTO> ENDO_WGO() => this.LoadData<ENDO_PT_DTO>("ENDO_WGO_PT_INFO");
    #endregion

    #region ETC
    public List<DRUG_DTO> DRUG() => LoadData<DRUG_DTO>();
    public List<DR_SCH_DTO> DR_SCH() => LoadData<DR_SCH_DTO>();
    #endregion

  }
}