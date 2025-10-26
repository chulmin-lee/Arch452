using Common;
using Framework.DataSource;
using System.Collections.Generic;
using System.IO;

namespace EUMC.HospitalService
{
  public class HospitalRepositorySim : IHospitalRepository
  {
    string _sim_dir;
    public HospitalRepositorySim(string dir_name = "SIM")
    {
      _sim_dir = dir_name;
    }

    List<T> LoadData<T>(string name = null) where T : OriginDataModel
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

    public List<DRUG_DTO> DRUG_DATA() => LoadData<DRUG_DTO>();
    public List<ER_PATIENT_DTO> EMERGENCY_DATA() => LoadData<ER_PATIENT_DTO>();
    public List<ER_CONGESTION_DTO> ER_CONGESTION_DATA() => LoadData<ER_CONGESTION_DTO>();
    public List<ER_AREA_CONGEST_DTO> ER_AREA_CONGEST_DATA() => LoadData<ER_AREA_CONGEST_DTO>();
    public List<ER_ISOLATION_DTO> ER_ISOLATION_DATA() => LoadData<ER_ISOLATION_DTO>();
    public List<EXAM_DTO> EXAM_DATA() => LoadData<EXAM_DTO>();
    public List<ICU_DTO> ICU_DATA() => LoadData<ICU_DTO>();
    public List<OFFICE_DTO> OFFICE_DATA() => LoadData<OFFICE_DTO>();
    public List<PHOTO_DTO> PHOTO_DATA() => LoadData<PHOTO_DTO>();
    public List<OPERATION_DTO> OPERATION_DATA() => LoadData<OPERATION_DTO>();
    public List<INSPECTION_DTO> INSPECTION_DATA() => LoadData<INSPECTION_DTO>();
    public List<NAME_PLATE_DTO> NAME_PLATE_DATA() => LoadData<NAME_PLATE_DTO>();
    public List<DR_SCH_DTO> DR_SCH_DATA() => LoadData<DR_SCH_DTO>();
    public List<ENDO_DTO> ENDO_DATA() => LoadData<ENDO_DTO>();
    public List<WARD_DTO> WARD_DATA() => LoadData<WARD_DTO>();
    public List<DELIVERY_ROOM_DTO> DELIVERY_ROOM_DATA() => LoadData<DELIVERY_ROOM_DTO>();
  }
}