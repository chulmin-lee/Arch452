namespace EUMC.HospitalService
{
  public enum DATA_ID
  {
    None = 0,
    DEPT_MASTER,
    //==========================
    // Emergency
    //==========================
    ER_PATIENT,
    ER_CONGESTION,
    ER_CPR,
    ER_CT,
    //==========================
    // IPD
    //==========================
    ICU,
    OPERATION,

    //==========================
    // OPD Office
    //==========================
    OFFICE_ROOM,
    OFFICE_PT,
    DR_PHOTO,
    //==========================
    // OPD 검사실
    //==========================
    EXAM_DEPT,
    EXAM_ROOM,
    EXAM_STAFF,
    EXAM_PT,

    //==========================
    // OPD 특수 검사실
    //==========================
    ANG,   // 혈관조영실
    ANG2,  // 혈관조영실(3F)
    ANG_IMC, // 심뇌혈과조영실
    ENDO,  // 내시경
    RAD,
    RAD_TR,
    ENDO_WGO,



    //==========================
    // ETC
    //==========================
    DRUG,

    DR_SCH,
  }
}