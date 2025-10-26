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
        //==========================
        // Emergency
        //==========================
        case DATA_ID.ER_PATIENT: return new ER_PATIENT_Extractor(owner);
        case DATA_ID.ER_AREA_CONGEST: return new ER_AREA_CONGEST_Extractor(owner);
        case DATA_ID.ER_CONGESTION: return new ER_CONGESTION_Extractor(owner);
        case DATA_ID.ER_ISOLATION: return new ER_ISOLATION_Extractor(owner);
        //==========================
        // IPD
        //==========================
        case DATA_ID.ICU: return new ICU_Extractor(owner);
        case DATA_ID.DELIVERY_ROOM: return new DELIVERY_Extractor(owner);
        case DATA_ID.OPERATION: return new OPERATION_Extractor(owner);
        case DATA_ID.WARD_ROOMS: return new WARD_Extractor(owner);
        //==========================
        // OPD
        //==========================
        case DATA_ID.OFFICE_ROOM: return new OPD_OFFICE_Extractor(owner);
        case DATA_ID.EXAM_ROOM: return new OPD_EXAM_Extractor(owner);
        case DATA_ID.INSPECTION: return new INSPECTION_Extractor(owner);
        case DATA_ID.ENDO: return new ENDO_Extractor(owner);
        //==========================
        // ETC
        //==========================
        case DATA_ID.DR_PHOTO: return new DR_PHOTO_Extractor(owner, o.DR_PHOTO);
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