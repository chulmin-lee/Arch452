namespace EUMC.HospitalService
{
  public enum DATA_ID
  {
    None = 0,
    //==========================
    // Emergency
    //==========================
    ER_PATIENT,
    ER_AREA_CONGEST,
    ER_CONGESTION,
    ER_ISOLATION,
    //==========================
    // IPD
    //==========================
    ICU,
    DELIVERY_ROOM, // 분만실
    OPERATION,
    WARD_ROOMS,  // 병실정보
                 //==========================
                 // IPD
                 //==========================
    OFFICE_ROOM,
    EXAM_ROOM,
    INSPECTION,
    ENDO,  // 내시경
           //==========================
           // ETC
           //==========================
    DRUG,
    DR_PHOTO,
    DR_SCH,
  }
}