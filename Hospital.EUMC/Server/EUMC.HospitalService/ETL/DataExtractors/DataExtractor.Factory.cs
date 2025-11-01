using Common;
using ServiceCommon;
using ServiceCommon.ServerServices;
using System;

namespace EUMC.HospitalService
{
  internal static class DataExtractorFactory
  {
    public static IDataExtractor<DATA_ID> Create(IHospitalMemberOwner owner, DATA_ID id, DataConfigurations o)
    {
      if (owner == null) throw new ArgumentNullException(nameof(owner));
      if (o == null) throw new ArgumentNullException("DataConfig");

      switch (id)
      {
        case DATA_ID.DEPT_MASTER: return new DEPT_MASTER_Extractor(owner);
        //==========================
        // Emergency
        //==========================
        case DATA_ID.ER_PATIENT: return new ER_PATIENT_Extractor(owner);
        case DATA_ID.ER_CONGESTION: return new ER_CONGESTION_Extractor(owner);
        case DATA_ID.ER_CPR: return new ER_CPR_Extractor(owner);
        case DATA_ID.ER_CT: return new ER_CT_Extractor(owner);
        //==========================
        // IPD
        //==========================
        case DATA_ID.ICU: return new ICU_Extractor(owner, o.ICU);
        case DATA_ID.OPERATION: return new OPERATION_Extractor(owner);
        //==========================
        // OPD Office
        //==========================
        case DATA_ID.OFFICE_ROOM: return new OFFICE_ROOM_Extractor(owner);
        case DATA_ID.OFFICE_PT: return new OFFICE_PT_Extractor(owner);
        case DATA_ID.DR_PHOTO: return new DR_PHOTO_Extractor(owner, o.DR_PHOTO);
        //==========================
        // OPD exam
        //==========================
        case DATA_ID.EXAM_DEPT: return new EXAM_DEPT_Extractor(owner);
        case DATA_ID.EXAM_ROOM: return new EXAM_ROOM_Extractor(owner);
        case DATA_ID.EXAM_STAFF: return new EXAM_STAFF_Extractor(owner);
        case DATA_ID.EXAM_PT: return new EXAM_PT_Extractor(owner);
        //==========================
        // OPD exam special
        //==========================
        case DATA_ID.ANG: return new ANG_Extractor(owner);
        case DATA_ID.ANG2: return new ANG2_Extractor(owner);
        case DATA_ID.ANG_IMC: return new ANG_IMC_Extractor(owner);
        case DATA_ID.ENDO: return new ENDO_Extractor(owner);
        case DATA_ID.ENDO_WGO: return new ENDO_WGO_Extractor(owner);
        case DATA_ID.RAD: return new RAD_Extractor(owner);
        case DATA_ID.RAD_TR: return new RAD_TR_Extractor(owner);
        //==========================
        // ETC
        //==========================
        case DATA_ID.DRUG: return new DRUG_Extractor(owner);
        case DATA_ID.DR_SCH: return new DR_SCH_Extractor(owner);

        default:
          {
            throw new ServiceException($"{id} not supported");
          }
      }
    }
  }
}