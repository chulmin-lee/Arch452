﻿using System.Collections.Generic;

namespace EUMC.HospitalService
{
  public interface IHospitalRepository
  {
    List<OFFICE_DTO> OFFICE_DATA();
    List<PHOTO_DTO> PHOTO_DATA();
    List<EXAM_DTO> EXAM_DATA();

    List<ER_PATIENT_DTO> EMERGENCY_DATA();
    List<ER_AREA_CONGEST_DTO> ER_AREA_CONGEST_DATA();
    List<ER_CONGESTION_DTO> ER_CONGESTION_DATA();
    List<ER_ISOLATION_DTO> ER_ISOLATION_DATA();

    List<OPERATION_DTO> OPERATION_DATA();
    List<ICU_DTO> ICU_DATA();

    List<DRUG_DTO> DRUG_DATA();
    List<INSPECTION_DTO> INSPECTION_DATA();
    List<NAME_PLATE_DTO> NAME_PLATE_DATA();
    List<DR_SCH_DTO> DR_SCH_DATA();

    List<ENDO_DTO> ENDO_DATA();
    List<WARD_DTO> WARD_DATA();
    List<DELIVERY_ROOM_DTO> DELIVERY_ROOM_DATA();
  }
}