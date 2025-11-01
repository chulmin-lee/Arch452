
using Common;
using Framework.DataSource.Oracle;
using System;
using System.Linq;
using System.Collections.Generic;
using ServiceCommon;

namespace EUMC.HospitalService
{
  public class EumcRepository : OracleRepository, IHospitalRepository
  {
    const string XMED = "XMED";
    const string XEDP = "XEDP";
    const string XSUP = "XSUP";
    string HspCode;

    //Dictionary<DATA_ID, string> _procedure;

    public EumcRepository(string hspCode, string xmed, string xedp, string xsup)
    {
      this.HspCode = hspCode;
      AddConnection(XMED, xmed);
      AddConnection(XEDP, xedp);
      AddConnection(XSUP, xsup);
      //_procedure = this.intialize();
    }
    //Dictionary<DATA_ID, string> intialize()
    //{
    //  var o = new Dictionary<DATA_ID, string>();
    //  o.Add(DATA_ID.ER_CONGESTION, "PKG_ELPD_DB.PC_ER_STA_AVG_TM_INFO");
    //  o.Add(DATA_ID.ER_CPR, "PKG_ELPD_DB.PC_ELPD_ER_CPR");
    //  o.Add(DATA_ID.ER_CT, "PKG_ELPD_DB.PC_ELPD_ER_CPR");
    //  o.Add(DATA_ID.ICU, "PKG_ELPD_DB.PC_ICU_PT_INFO");

    //  o.Add(DATA_ID., "PKG_ELPD_DB.PC_ICU_PT_INFO");
    //  o.Add(DATA_ID.OPERATION, "PKG_ELPD_DB.PC_ICU_PT_INFO");
    //  return o;
    //}



    #region base
    public List<DEPT_MASTER_DTO> DEPT_MASTER()
    {
      var query = "SELECT A.DEPT_CD, A.HSP_TP_CD, " +
                  "NVL((SELECT B.LWR_CTG_CNTE FROM BPGEBCCC B WHERE B.HSP_TP_CD = A.HSP_TP_CD AND B.LWR_CTG_NM = A.DEPT_CD AND rownum = 1),A.DEPT_NM) AS DEPT_NM " +
                  $"FROM PDEDBMSM A WHERE A.HSP_TP_CD='{HspCode}' ORDER BY DEPT_CD";
      return Query<DEPT_MASTER_DTO>(XEDP, query);
    }
    #endregion

    #region Emergency
    /// <summary>
    /// 응급실 환자 조회
    /// </summary>
    /// <returns></returns>
    public List<ER_PATIENT_DTO> ER_PATIENT()
    {
      // 응급실 구역 정보
      var query = "SELECT DISTINCT C.HSP_TP_CD, C.LWR_CTG_NM, C.LWR_CTG_CNTE FROM BPGEBCCC C "
                   + $"WHERE C.HSP_TP_CD = '{HspCode}' AND C.ELDP_COMN_CD = 'ERZONE' AND C.USE_YN = 'Y'";
      var area_codes = Query<ER_AREA_DTO>(XEDP, query).Select(x => x.LWR_CTG_NM);

      // 응급실 환자
      var proc = "PKG_ELPD_DB.PC_ER_PT_INFO";
      var patients = new List<ER_PATIENT_DTO>();
      foreach (var area in area_codes)
      {
        var param = new HospitalParamBuilder().ER_AREA_CD(area).HSP_TP_CD(HspCode).Build();
        patients.AddRange(this.QueryProcedure<ER_PATIENT_DTO>(XEDP, proc, param));
      }
      return patients;
    }
    /// <summary>
    /// 응급실 혼잡도 및 평균대기시간 조회
    /// </summary>
    public List<ER_CONGESTION_DTO> ER_CONGESTION()
    {
      var proc = "PKG_ELPD_DB.PC_ER_STA_AVG_TM_INFO";
      var param = new HospitalParamBuilder().ER_AREA_CD().HSP_TP_CD(HspCode).Build();
      var list = QueryProcedure<ER_CONGESTION_DTO>(XEDP, proc, param);
      list.ForEach(x => x.HSP_TP_CD = HspCode);
      return list;
    }
    /// <summary>
    /// 응급실 CPR 상황 여부
    /// </summary>
    /// <returns></returns>
    public List<ER_CPR_DTO> ER_CPR()
    {
      var proc = "PKG_ELPD_DB.PC_ELPD_ER_CPR";
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
      return QueryProcedure<ER_CPR_DTO>(XEDP, proc, param);
    }
    /// <summary>
    /// 응급실 CT 실 조회
    /// </summary>
    public List<ER_CT_DTO> ER_CT()
    {
      var proc = "PKG_ELPD_DB.PC_ER_PT_INFO_C";
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
      return QueryProcedure<ER_CT_DTO>(XEDP, proc, param);
    }
    #endregion

    #region IPD
    /// <summary>
    /// 중환자실 환자 조회
    /// </summary>
    /// <param name="icu_dept_codes">중화자실이 있는 부서 목록</param>
    public List<ICU_DTO> ICU(List<string> icu_dept_codes)
    {
      var proc = "PKG_ELPD_DB.PC_ICU_PT_INFO";
      var icu = new List<ICU_DTO>();

      foreach (var dept_code in icu_dept_codes)
      {
        var param = new HospitalParamBuilder().DEPT_CD(dept_code)
                                            .HSP_TP_CD(HspCode)
                                            .Build();

        var list = this.QueryProcedure<ICU_DTO>(XEDP, proc, param).ToList();
        // DEPT_CD에 Icu Code가 들어있으므로 여기서 교체한다
        foreach(var p in list)
        {
          var icu_code = p.DEPT_CD;
          p.DEPT_CD = dept_code;
          p.ICU_CD = icu_code;
        }
        icu.AddRange(list);
      }
      return icu;
    }

    /// <summary>
    /// 수술환자 조회
    /// 상태
    /// -  1: 대기중, 2: 수술중, [수술종료] 3: 회복실, 4: 병실, 5: 중환자실
    /// </summary>
    /// <returns></returns>
    public List<OPERATION_DTO> OPERATION()
    {
      var patients = new List<OPERATION_DTO>();

      // 1: 대기중
      {
        var proc = "PKG_ELPD_DB.PC_OP_WAIT_PT_INFO";
        var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
        var waiting = this.QueryProcedure<OP_WAIT_PT_DTO>(XEDP, proc, param);
        waiting.ForEach(x => patients.Add(new OPERATION_DTO { PT_NO = x.PT_NO, PT_NM = x.PT_NM, StateCode = "1" }));
      }
      //, 2: 수술중
      {
        var proc = "PKG_ELPD_DB.PC_OP_PT_INFO";
        var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
        var op = this.QueryProcedure<OPERATION_DTO>(XEDP, proc, param);
        patients.ForEach(x => x.StateCode = "2");
      }
      // [수술종료] 3:회복실, 4:병실, 5:중환자실
      {
        var proc = "PKG_ELPD_DB.PC_OP_END_PT_INFO";
        var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
        var end = this.QueryProcedure<OP_END_PT_DTO>(XEDP, proc, param);

        foreach(var p in end)
        {
          var o = new OPERATION_DTO { PT_NO = p.PT_NO, PT_NM = p.PT_NM, };
          switch (p.PT_PSTN_CD)
          {
            case "회복실" : o.StateCode = "3"; break;
            case "병실" : o.StateCode = "4"; break;
            case "중환자실" : o.StateCode = "5"; break;
          }
          patients.Add(o);
        }
      }
      return patients;
    }
    #endregion

    #region Office
    /// <summary>
    /// 기본 진료실 정보
    /// </summary>
    public List<OFFICE_ROOM_MASTER_DTO> OFFICE_ROOM_MASTER()
    {
      var query = $"SELECT HSP_TP_CD, MED_DEPT_CD, MTM_NO, MTM_NM, OEDP_MRK_YN from BPGEBROD where OEDP_MRK_YN='Y' and HSP_TP_CD = '{HspCode}'";
      return Query<OFFICE_ROOM_MASTER_DTO>(XEDP, query);
    }
    /// <summary>
    /// 운영중인 진료실 정보 조회
    /// </summary>
    /// <param name="room_dept_codes">조회할 부서</param>
    public List<OFFICE_ROOM_DTO> OFFICE_ROOM()
    {
      // OFFICE_ROOM_MASTER() 간략화 버전
      var query = $"SELECT distinct MED_DEPT_CD from BPGEBROD where OEDP_MRK_YN='Y' AND HSP_TP_CD='{HspCode}'";
      var office_dept_codes = this.Query<OFFICE_ROOM_MASTER_DTO>(XEDP, query).Select(x => x.MED_DEPT_CD);

      // 부서별 진료실 정보 조회
      // - OEDP_MRK_YN='Y'인 진료실만 검색된다.
      var proc = "PKG_ELPD_DB.PC_MED_ALL_RM_INFO";
      var all_rooms = new List<OFFICE_ROOM_DTO>();
      foreach (var dept_code in office_dept_codes)
      {
        var param = new HospitalParamBuilder().DEPT_CD(dept_code)
                                          .HSP_TP_CD(HspCode)
                                          .Build();
        var list = this.QueryProcedure<OFFICE_ROOM_DTO>(XEDP, proc, param);
        list.ForEach(x => { x.DEPT_CD = dept_code; });
        all_rooms.AddRange(list);
      }
      return all_rooms;
    }
    /// <summary>
    /// 부서/진료실별 환자 정보
    /// </summary>
    /// <param name="dept_room_no">부서명, 진료실번호</param>
    public List<OFFICE_PT_DTO> OFFICE_PT(Dictionary<string, List<string>> dept_room_no)
    {
      var proc = "PKG_ELPD_DB.PC_MED_RM_PT_INFO";
      var room_pt = new List<OFFICE_PT_DTO>();

      foreach (var dept in dept_room_no)
      {
        string dept_code = dept.Key;
        foreach (var room in dept.Value)
        {
          var param = new HospitalParamBuilder().DEPT_CD(dept_code)
                                            .RM_NO(room)
                                            .HSP_TP_CD(HspCode)
                                            .IP_ADDR()
                                            .Build();
          // 2025/10/27 조회 부서가 다른경우 필터링
          var list = this.QueryProcedure<OFFICE_PT_DTO>(XEDP, proc, param)
                         .Where(x => x.DEPT_CD == dept_code &&
                                     x.PT_NM == x.ELDP_PT_NM &&
                                     !string.IsNullOrEmpty(x.PACT_ID));
          room_pt.AddRange(list);
        }
      }
      room_pt.ForEach(x => x.HSP_TP_CD = HspCode);

      #region PACT_ID 중복 검사 (2025/10/27)
      var count = room_pt.Select(x => x.PACT_ID).Distinct().Count();
      if (count != room_pt.Count)
      {
        var dupe = room_pt.GroupBy(x => x.PACT_ID)
                          .Where(g => g.Count() > 1)
                          .SelectMany(y => y.ToList())
                          .ToList();
        foreach (var p in dupe)
        {
          LOG.ec($"dup PACT_ID: {p}");
          //room_pt.Remove(p);
        }
      }
      #endregion
      return room_pt;
    }



    #endregion

    #region EXAM_Common
    /// <summary>
    /// 검사실이 있는 부서 목록 조회
    /// </summary>se
    public List<EXAM_DEPT_DTO> EXAM_DEPT()
    {
      var proc = "PKG_ELPD_DB.PC_ELPD_EXAM_ROOM";
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
      var list = QueryProcedure<EXAM_DEPT_DTO>(XEDP, proc, param);
      // DEPT_CD 중복 필터링
      list = list.GroupBy(x => x.DEPT_CD).Select(x => x.First()).ToList();
      return list;
    }
    /// <summary>
    /// 검사실 정보
    /// </summary>
    /// <param name="exam_dept_cd">검사실이 있는 부서 코드</param>
    public List<EXAM_ROOM_DTO> EXAM_ROOM(List<string> exam_dept_cd)
    {
      // 검사실이 있는 부서코드 조회
      //var proc = "PKG_ELPD_DB.PC_ELPD_EXAM_ROOM";
      //var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
      //var exam_dept_cd = QueryProcedure<EXAM_DEPT_DTO>(XEDP, proc, param).Select(x => x.DEPT_CD).Distinct();
      var proc = "PKG_ELPD_DB.PC_EXAM_RM_INFO";
      var exam_rooms = new List<EXAM_ROOM_DTO>();

      foreach (var dept_code in exam_dept_cd)
      {
        var param = new HospitalParamBuilder().HSP_TP_CD(HspCode)
                                          .DEPT_CD(dept_code)
                                          .EXAM_RM_CD()
                                          .Build();
        var list = this.QueryProcedure<EXAM_ROOM_DTO>(XEDP, proc, param);
        list.ForEach(x => { x.DEPT_CD = dept_code; });
        exam_rooms.AddRange(list);
      }
      return exam_rooms;
    }
    public List<EXAM_STAFF_DTO> EXAM_STAFF(List<string> exam_dept_cd)
    {
      var proc = "PKG_ELPD_DB.PC_EXAM_STAFF_INFO";
      var staffs = new List<EXAM_STAFF_DTO>();

      foreach (var dept_code in exam_dept_cd)
      {
        var param = new HospitalParamBuilder().HSP_TP_CD(HspCode)
                                          .DEPT_CD(dept_code)
                                          .EXAM_RM_CD()
                                          .Build();
        var list = this.QueryProcedure<EXAM_STAFF_DTO>(XEDP, proc, param);
        list.ForEach(x => { x.DEPT_CD = dept_code; });
        staffs.AddRange(list);
      }
      return staffs;
    }
    public List<EXAM_PT_DTO> EXAM_PT(List<string> exam_dept_cd)
    {
      var proc = "PKG_ELPD_DB.PC_EXAM_STATE_PT_LIST";
      var patients = new List<EXAM_PT_DTO>();
      foreach (var dept_code in exam_dept_cd)
      {
        var param = new HospitalParamBuilder().HSP_TP_CD(HspCode)
                                          .STATE("W")
                                          .EXAM_RM_CD()
                                          .DEPT_CD(dept_code)
                                          .Build();
        var list = this.QueryProcedure<EXAM_PT_DTO>(XEDP, proc, param);
        patients.AddRange(list);
      }
      return patients;
    }
    #endregion

    #region OPD_EXAM_SPECIAL
    public List<ANG_PT_DTO> ANG_IMC_PT()
    {
      var proc = "PKG_ELPD_DB.PC_ANG_PT_INFO_IMC";
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
      var list = this.QueryProcedure<ANG_PT_DTO>(XEDP, proc, param).ToList();
      return list;
    }
    public List<ANG_PT_DTO> ANG_PT()
    {
      var proc = "PKG_ELPD_DB.PC_ANG_PT_INFO";
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
      var list = this.QueryProcedure<ANG_PT_DTO>(XEDP, proc, param).ToList();
      return list;
    }
    public List<ANG_PT_DTO> ANG2_PT()
    {
      var proc = "PKG_ELPD_DB.PC_ANG_PT_INFO2";
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
      var list = this.QueryProcedure<ANG_PT_DTO>(XEDP, proc, param).ToList();
      return list;
    }
    /// <summary>
    /// 내시경(Endoscope) 환자 조회
    /// </summary>
    public List<ENDO_PT_DTO> ENDO_PT()
    {
      var proc = "PKG_ELPD_DB_SUP.PC_RAD_EXAM_PT_INFO_PENDO";
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
      var list = this.QueryProcedure<ENDO_PT_DTO>(XSUP, proc, param);
      return list;
    }
    /// <summary>
    /// 영상의학과 일반촬영실 환자조회
    /// </summary>
    /// <param name="exam_room_codes">검사실이 있는 부서의 검사실 코드</param>
    public List<RAD_PT_DTO> RAD_PT(List<string> exam_room_codes)
    {
      var proc = "PKG_ELPD_DB_SUP.PC_RAD_EXAM_PT_INFO_SEOUL";
      var room_pt = new List<RAD_PT_DTO>();

      foreach (var exam_room_code in exam_room_codes)
      {
        var param = new HospitalParamBuilder().HSP_TP_CD(HspCode)
                                          .EXRM_TP_CD(exam_room_code)
                                          .Build();
        var list = this.QueryProcedure<RAD_PT_DTO>(XSUP, proc, param);
        room_pt.AddRange(list);
      }
      return room_pt;
    }
    /// <summary>
    /// 방사선종양학과 환자 조회
    /// </summary>
    public List<RAD_TR_PT_DTO> RAD_TR_PT()
    {
      var proc = "PKG_ELPD_DB.PC_EXAM_PT_INFO_TR";
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).TRP_RSV_DT().Build();
      return this.QueryProcedure<RAD_TR_PT_DTO>(XEDP, proc, param).ToList();
    }
    /// <summary>
    /// 여성 암병원 초음파 실 대기/검사 인원 조회
    /// </summary>
    /// <returns></returns>
    public List<ENDO_PT_DTO> ENDO_WGO()
    {
      var proc = "PKG_ELPD_DB.PC_WGO_PT_INFO_US";
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).TRP_RSV_DT().Build();
      return this.QueryProcedure<ENDO_PT_DTO>(XEDP, proc, param).ToList();
    }
    #endregion


    #region ETC
    public List<DRUG_DTO> DRUG()
    {
      var proc = "PKG_ELPD_DB.PC_DRAG_CALL_INFO";
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
      var list = this.QueryProcedure<DRUG_DTO>(XEDP, proc, param);
      list.ForEach(x => x.HSP_TP_CD = HspCode);
      return list;
    }
    public List<DR_PHOTO_DTO> DR_PHOTO()
    {
      var proc = "PKG_ELPD_DB.PC_MED_ALL_DRIMG_INFO";
      var all_rooms = new List<DR_PHOTO_DTO>();
      var param = new HospitalParamBuilder().HSP_TP_CD(HspCode).Build();
      return this.QueryProcedure<DR_PHOTO_DTO>(XEDP, proc, param);
    }

    /// <summary>
    /// 진료 스케쥴
    /// </summary>
    public List<DR_SCH_DTO> DR_SCH()
    {
      var proc = "PKG_ELPD_DB_MED.PC_MED_SCH_INFO_A";
      var param = new HospitalParamBuilder().MED_DEPT_CD().HSP_TP_CD(HspCode).Build();
      var list = this.QueryProcedure<DR_SCH_DTO>(XEDP, proc, param);
      list.ForEach(x => x.HSP_TP_CD = HspCode);
      return list;
    }
    #endregion


  }
}
