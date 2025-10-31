using System.Collections.Generic;

namespace EUMC.HospitalService
{
  public interface IEumcRepository
  {
    #region Base
    List<DEPT_MASTER_DTO> DEPT_MASTER();
    #endregion


    #region Emergency
    List<ER_PATIENT_DTO> ER_PATIENT();
    List<ER_CONGESTION_DTO> ER_CONGESTION();
    List<ER_CPR_DTO> ER_CPR();
    List<ER_CT_DTO> ER_CT();
    #endregion

    #region IPD
    List<OPERATION_DTO> OPERATION();
    List<ICU_DTO> ICU(List<string> icu_dept_codes);
    #endregion

    #region OPD_Office
    List<OFFICE_ROOM_MASTER_DTO> OFFICE_ROOM_MASTER();
    List<OFFICE_ROOM_DTO> OFFICE_ROOM();
    List<OFFICE_PT_DTO> OFFICE_PT(Dictionary<string, List<string>> dept_room_no);
    List<DR_PHOTO_DTO> DR_PHOTO();
    #endregion


    #region OPD_EXAM
    List<EXAM_DEPT_DTO> EXAM_DEPT();
    List<EXAM_ROOM_DTO> EXAM_ROOM(List<string> exam_dept_cd);
    List<EXAM_STAFF_DTO> EXAM_STAFF(List<string> exam_dept_cd);
    List<EXAM_PT_DTO> EXAM_PT(List<string> exam_dept_cd);
    #endregion

    #region OPD_EXAM_SPECIAL
    List<ANG_PT_DTO> ANG_PT();
    List<ANG_PT_DTO> ANG2_PT();
    List<ANG_PT_DTO> ANG_IMC_PT();
    List<ENDO_PT_DTO> ENDO_PT();
    List<RAD_PT_DTO> RAD_PT(List<string> exam_room_codes);
    List<RAD_TR_PT_DTO> RAD_TR_PT();
    List<ENDO_WGO_PT_DTO> ENDO_WGO();
    #endregion

    //=====================================
    // ETC
    //=====================================
    List<DRUG_DTO> DRUG();
    List<DR_SCH_DTO> DR_SCH();
  }
}