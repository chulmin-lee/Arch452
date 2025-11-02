using Framework.DataSource;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace EUMC.HospitalService
{
  public class DEPT_MASTER_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string DEPT_CD { get; set; }
    [Origin] public string HSP_TP_CD { get; set; }
    [Origin] public string DEPT_NM { get; set; }
  }


  #region Emergency
  /// <summary>
  /// 응급실 구역 정보 조회
  /// </summary>
  public class ER_AREA_DTO : OriginDataModel
  {
    [Origin] public string HSP_TP_CD { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string LWR_CTG_NM { get; set; } = string.Empty;   // "A"
    [Origin] public string LWR_CTG_CNTE { get; set; } = string.Empty;      // "중증응급환자진료구역"

    public override string ToString()
    {
      return $"[{HashCode}] {LWR_CTG_NM}/{LWR_CTG_CNTE}";
    }
  }
  public class ER_PATIENT_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string ER_AREA_CD { get; set; } = string.Empty;  // "A" ER_ARED_DTO 에서 조회
    [Origin] public string ER_AREA_NM { get; set; } = string.Empty; // "A 중증응급환자진료구역"
    [Origin(DTO.UNIQUE)] public string PT_NO { get; set; } = string.Empty;
    [Origin] public string PT_NM { get; set; } = string.Empty;
    [Origin] public string AGE { get; set; } = string.Empty;  // 64세
    [Origin] public string DOC_STATE { get; set; } = string.Empty;
    [Origin] public string BLO_STATE { get; set; } = string.Empty;
    [Origin] public string RAD_STATE { get; set; } = string.Empty; // 요청/진행/미해당/완료
    [Origin] public string CON_STATE { get; set; } = string.Empty;
    [Origin] public string INOUT_STATE { get; set; } = string.Empty;

    //----------------------------
    [Origin(DTO.COMPUTED)] public string ER_AREA_GBN { get; set; } = string.Empty;

    public override string ToString()
    {
      return $"{PT_NO}/{PT_NM}/{ER_AREA_CD}/{ER_AREA_NM}/{DOC_STATE}/{BLO_STATE}/{RAD_STATE}/{CON_STATE}/{INOUT_STATE}";
    }
  }
  /// <summary>
  /// 응급실 혼잡도
  /// </summary>
  public class ER_CONGESTION_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string 구역 { get; set; } = string.Empty;  // 성인/소아
    [Origin] public int CALC_VALUE { get; set; }  // 혼잡 퍼센트
    [Origin] public int ER_SATY_TM { get; set; } // 응급환자 평균대기시간
    [Origin] public int ER_MED_TM { get; set; } // 응급진료 평균대기시간
    [Origin] public int CT_EXAM_TM { get; set; } // CT검사
    [Origin] public int XRAY_EXAM_TM { get; set; } // X-ray 검사
    //----------
    [Origin] public string HSP_TP_CD { get; set; } = string.Empty;

    public override string ToString()
    {
      return $"{구역}/{CALC_VALUE}/{ER_SATY_TM}/{ER_MED_TM}/{CT_EXAM_TM}/{XRAY_EXAM_TM}";
    }
  }
  /// <summary>
  /// 응급실 CPR 상태 조회
  /// </summary>
  public class ER_CPR_DTO : OriginDataModel
  {
    [Origin] public string ER_CPR_STATE { get; set; } = "N";
    //--------------
    public override string ToString()
    {
      return $"{ER_CPR_STATE}";
    }
  }
  /// <summary>
  /// 응급실 CT실
  /// </summary>
  public class ER_CT_DTO : OriginDataModel
  {
    [Origin] public int SEQ { get; set; }
    [Origin(DTO.UNIQUE)] public string PT_NO { get; set; } = string.Empty;
    [Origin] public string PT_NM { get; set; } = string.Empty;
    [Origin] public string NUM { get; set; } = string.Empty;      // 위치정보
    [Origin] public string ACPT_DTM { get; set; } = string.Empty;  // 날짜 (A는 날짜시간, B는 날짜만)
    [Origin(DTO.UNIQUE)] public string ORD_CD { get; set; } = string.Empty;  // 처방항목
    [Origin(DTO.UNIQUE)] public string CODE { get; set; } = string.Empty;   // A: 검사중, B: 처방받음

    //[Origin] public string EXM_PRGR_STS_CD { get; set; } // 사용안함
    public override string ToString()
    {
      return $"{PT_NO}/{PT_NM}/{SEQ}/{CODE}";
    }
  }
  #endregion

  #region IPD
  public class OP_WAIT_PT_DTO
  {
    public string PT_NO { get; set; } = string.Empty;
    public string SEQ { get; set; } = string.Empty;
    public string PT_NM { get; set; } = string.Empty;
  }

  public class OPERATION_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string PT_NO { get; set; } = string.Empty;
    [Origin] public string PT_NM { get; set; } = string.Empty;
    [Origin] public string StateCode{ get; set; } = string.Empty; //1: 대기중, 2: 수술중, [수술종료] 3: 회복실, 4: 병실, 5: 중환자실
  }
  public class OP_END_PT_DTO
  {
    public string PT_NO { get; set; } = string.Empty;
    public string PT_NM { get; set; } = string.Empty;
    public string PT_PSTN_CD { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string OPRM_NM { get; set; } = string.Empty;
    [Origin] public string OP_END_TM { get; set; } = string.Empty;
    //------------------------
    [Origin] public string HSP_TP_CD { get; set; } = string.Empty;

    public override string ToString()
    {
      return $"{PT_NO}/{PT_NM}/{PT_PSTN_CD}/{OPRM_NM}/{OP_END_TM}";
    }
  }

  public class ICU_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string PT_NO { get; set; } = string.Empty;
    [Origin] public string PT_NM { get; set; } = string.Empty; // [10]구본욱
    [Origin] public string DEPT_CD { get; set; } = string.Empty;
    [Origin] public string DEPT_NM { get; set; } = string.Empty; // 심장혈관외과(대동맥혈관병원)
    [Origin] public string BED_NO { get; set; } = string.Empty;
    //--------------------
    [Origin] public string ICU_CD { get; set; } = string.Empty; // 조회용 DEPT_CD가 들어있다.

    public override string ToString()
    {
      return $"{ICU_CD}, {PT_NO}, {PT_NM}";
    }
  }
  #endregion IPD

  #region OPD_OFFICE
  /// <summary>
  /// 전체 진료실 마스터 정보
  /// </summary>
  public class OFFICE_ROOM_MASTER_DTO : OriginDataModel
  {
    [Origin] public string HSP_TP_CD { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string MED_DEPT_CD { get; set; } = string.Empty; // DEPT_CD
    [Origin(DTO.UNIQUE)] public string MTM_NO { get; set; } = string.Empty;      // 진료실번호
    [Origin] public string MTM_NM { get; set; } = string.Empty;                  // 진료실이름
    [Origin] public string OEDP_MRK_YN { get; set; } = string.Empty;             // 사용여부

    public override string ToString()
    {
      return $"[{HashCode}] {MED_DEPT_CD}/{MTM_NO}/{MTM_NM}/USE:{OEDP_MRK_YN}";
    }
    public bool UseYN() => this.OEDP_MRK_YN == "Y";
  }
  /// <summary>
  /// 진료실 환자 목록
  /// 같은 사람이 동시에 여러 부서를 방문할 수 있다. PT_NO 는 중복 가능
  /// </summary>
  public class OFFICE_PT_DTO : OriginDataModel
  {
    [Origin] public string HSP_TP_CD { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string DEPT_CD { get; set; } = string.Empty; // 여러 부서에 방문 가능
    [Origin] public string DEPT_NM { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string RM_NO { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string PT_NO { get; set; } = string.Empty;
    [Origin] public string PT_NM { get; set; } = string.Empty;
    [Origin] public int RANK { get; set; }
    [Origin] public string ELDP_PT_NM { get; set; } = string.Empty;
    [Origin] public string PHONE_NO { get; set; } = string.Empty;
    [Origin] public string DEPT_FLR_TP_CD { get; set; } = string.Empty;
    [Origin] public string OEDP_MRK_YN { get; set; } = string.Empty;
    [Origin] public string PACT_ID { get; set; } = string.Empty;

    public string GetDeptName() => this.DEPT_NM.Split('(', ')')[0].Trim();

    public override string ToString()
    {
      return $"[{DEPT_CD}({this.DEPT_NM}).{RM_NO}/{DEPT_FLR_TP_CD}/{OEDP_MRK_YN}] {PT_NM}/{ELDP_PT_NM}/{PT_NO} [{RANK}]";
    }
    public bool UseYN() => OEDP_MRK_YN == "Y";
  }

  /// <summary>
  /// 현재 운영중인 진료실 정보
  /// </summary>
  public class OFFICE_ROOM_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string RM_NO { get; set; } = string.Empty;
    [Origin] public string RM_NM { get; set; } = string.Empty;
    [Origin] public string DR_NO { get; set; } = string.Empty;
    [Origin] public string DR_NM { get; set; } = string.Empty;// 예. "심장혈관흉부외과※성숙환",
    [Origin] public string DELEY_TM { get; set; } = string.Empty;
    [Origin] public string DELEY_TXT { get; set; } = string.Empty;
    //----------------
    [Origin(DTO.UNIQUE)] public string DEPT_CD { get; set; } = string.Empty;

    public string DoctorName()
    {
      var arr = this.DR_NM.Split('※');
      return arr.Length > 1 ? arr[1] : arr[0];
    }
    public string DoctorDeptName()
    {
      var arr = this.DR_NM.Split('※');
      return arr.Length > 1 ? arr[0] : string.Empty;
    }

    public override string ToString()
    {
      return $"{DEPT_CD}/{RM_NO}/{RM_NM}/{RM_NO}/{DR_NM}/{DELEY_TXT}/{DELEY_TM}";
    }
  }
  #endregion

  #region OPD_Exam
  /// <summary>
  /// 검사실이 있는 진료과 정보
  /// DEPT_CD 만 필요 (중복이 있다)
  /// </summary>
  public class EXAM_DEPT_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string DEPT_CD { get; set; } = string.Empty;
    [Origin] public string LWR_CTG_CNTE { get; set; } = string.Empty;
    public override string ToString()
    {
      return $"[{HashCode}] {DEPT_CD}/{LWR_CTG_CNTE}";
    }
  }
  /// <summary>
  /// 검사실 환자 정보
  /// </summary>
  public class EXAM_PT_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string DEPTCD { get; set; } = string.Empty;
    [Origin] public string DEPTNM { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string EXAMRMCD { get; set; } = string.Empty; // 검사실코드
    [Origin] public string EXAMRMNM { get; set; } = string.Empty; // 검사실명
    [Origin(DTO.UNIQUE)] public string PTNO { get; set; } = string.Empty;
    [Origin] public string PTNM { get; set; } = string.Empty;

    public int REGINDEX { get; set; }  // key
    public int ELPDSEQ { get; set; }      // ELPD 순서?
    public string ACPT_DTM { get; set; } = string.Empty; // 날짜
    public string RSV_DTM { get; set; } = string.Empty;  // 예약일
    //public string TEXTCOLOR { get; set; } = string.Empty;
    public override string ToString()
    {
      return $"{DEPTCD}/{EXAMRMCD}/{PTNM}/{PTNO}/{ELPDSEQ}";
    }
  }
  /// <summary>
  /// 검사실 상태 (검사실이 있는 부서별로 조회한다)
  /// </summary>
  public class EXAM_ROOM_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string EXAM_RM_CD { get; set; } = string.Empty;
    [Origin] public string EXAM_RM_NM { get; set; } = string.Empty;
    [Origin] public string DELAY_TEXT { get; set; } = string.Empty;
    [Origin] public string DELAY_TM { get; set; } = string.Empty;
    //---------
    [Origin(DTO.UNIQUE)] public string DEPT_CD { get; set; } = string.Empty;

    public override string ToString()
    {
      return $"{DEPT_CD}.{EXAM_RM_CD} : {EXAM_RM_NM}/{DELAY_TEXT}/{DELAY_TM}";
    }
  }
  /// <summary>
  /// 검사실 의료진
  /// </summary>
  public class EXAM_STAFF_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string EXAMRMCD { get; set; } = string.Empty;
    [Origin] public string EXAMRMNM { get; set; } = string.Empty;
    [Origin] public string STFNO { get; set; } = string.Empty;// 의료진 ID
    [Origin] public string STFNM { get; set; } = string.Empty; // 의료진 이름
    [Origin] public string DELEYTM { get; set; } = string.Empty;
    [Origin] public string DELEYTXT { get; set; } = string.Empty;
    [Origin] public string USE_GRP_CD { get; set; } = string.Empty;
    //-------------------------
    [Origin] public string DEPT_CD { get; set; } = string.Empty;
  }

  #endregion

  #region OPD_EXAM_SPECIAL
  /// <summary>
  /// 혈관조영술(angiography) 환자 정보
  /// </summary>
  public class ANG_PT_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string DEPT_CD { get; set; } = string.Empty;
    [Origin] public string DEPT_NM { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string PT_NO { get; set; } = string.Empty; // 환자번호(등록번호)
    [Origin] public string PT_NM { get; set; } = string.Empty;  // 환자이름

    [Origin(DTO.UNIQUE)] public string OP_NM { get; set; } = string.Empty;
    [Origin] public string OP_RM { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public int STATE_CD { get; set; }
    [Origin] public string STATE_NM { get; set; } = string.Empty;
    [Origin] public string OP_EXPT_DT { get; set; } = string.Empty;  // 시간
    [Origin] public string ETC { get; set; } = string.Empty;

    public override string ToString()
    {
      return $"{DEPT_CD}/{DEPT_NM},{PT_NO}/{PT_NM}/{OP_RM}/{STATE_CD}/{STATE_NM}/{ETC}";
    }
  }
  /// <summary>
  /// 내시경실 환자 조회
  /// </summary>
  public class ENDO_PT_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string GUBN { get; set; } = string.Empty; // 준비중
    [Origin(DTO.UNIQUE)] public string PT_NO { get; set; } = string.Empty;
    [Origin] public string PT_NM { get; set; } = string.Empty;
    //--------------------------
    [Origin] public string HSP_TP_CD { get; set; } = string.Empty;

    public override string ToString()
    {
      return $"{GUBN}/{PT_NO}/{PT_NM}";
    }
  }
  /// <summary>
  /// 여성암병원 초음파실 대기/검사 현황
  /// </summary>
  //public class ENDO_WGO_PT_DTO : OriginDataModel
  //{
  //  [Origin(DTO.UNIQUE)] public string GUBUN { get; set; }
  //  [Origin(DTO.UNIQUE)] public string PT_NO { get; set; }
  //  [Origin] public string PT_NM { get; set; }
  //}
  /// <summary>
  /// 영상의학과 일반촬영실 대기/완료 환자
  /// </summary>
  public class RAD_PT_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string RAD_RM_NO { get; set; } = string.Empty;
    [Origin] public string RAD_RM_NM { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string PT_NO { get; set; } = string.Empty;
    [Origin] public string PT_NM { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public int SEQ { get; set; }
    [Origin] public string RAD_YN { get; set; } = string.Empty;
    public override string ToString()
    {
      return $"{RAD_RM_NO}/{PT_NO}/{PT_NM}/{RAD_RM_NM}";
    }
  }
  /// <summary>
  /// 방사선종양학과 검사실 환자
  /// 시간순으로 조회된다
  /// </summary>
  public class RAD_TR_PT_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string TYRM_CD { get; set; } = string.Empty;  // RoomCode
    [Origin(DTO.UNIQUE)] public string PT_NO { get; set; } = string.Empty;
    [Origin] public string PT_NM { get; set; } = string.Empty;
    [Origin] public string RSV_DTM { get; set; } = string.Empty;
    [Origin] public string ACPT_DTM { get; set; } = string.Empty;
    [Origin] public string FMT_DTM { get; set; } = string.Empty;// 값이 있으면 출력에서 제외
    [Origin] public string HSP_TP_CD { get; set; } = string.Empty;

    public override string ToString()
    {
      return $"{PT_NO}/{PT_NM}";
    }
  }

  #endregion


  #region etc
  /// <summary>
  /// 의사 사진
  /// </summary>
  public class DR_PHOTO_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string STF_NO { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string IMAGE_PATH { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string GetFileName() => Path.GetFileName(this.IMAGE_PATH);
    public override string ToString()
    {
      return $"{STF_NO}/{IMAGE_PATH}";
    }
  }
  public class DRUG_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string RAG_NO { get; set; } = string.Empty; // 투약번호
    [Origin] public string PT_NO { get; set; } = string.Empty;
    [Origin] public string PT_NM { get; set; } = string.Empty;
    [Origin] public string DRAG_TM { get; set; } = string.Empty;
    [Origin] public string FMT_DRST_DEPT_CD { get; set; } = string.Empty;
    [Origin] public string ORD_DT { get; set; } = string.Empty;
    //--------------------
    [Origin] public string HSP_TP_CD { get; set; } = string.Empty;
    public override string ToString()
    {
      return $"{PT_NO}/{PT_NM}/{RAG_NO}";
    }
  }
  /// <summary>
  /// 진료 스케쥴
  /// </summary>
  public class DR_SCH_DTO : OriginDataModel
  {
    // center 정보
    [Origin(DTO.UNIQUE)] public string UPR_DEPT_CD { get; set; } = string.Empty;   // AVE
    //[Origin] public string UPR_DEPT_NM { get; set; } = string.Empty;               // 이대대동맥혈관병원
    [Origin] public string TRANS_CENTER_NM { get; set; } = string.Empty;           // 대동맥혈관병원
                                                                                   // 부서 정보
    [Origin(DTO.UNIQUE)] public string MED_DEPT_CD { get; set; } = string.Empty;   // VECVS
    //[Origin] public string MED_DEPT_NM { get; set; } = string.Empty;               // 심장혈관외과(대동맥혈관병원)
    [Origin] public string TRANS_DEPT_NM { get; set; } = string.Empty;             // 심장혈관외과
    [Origin] public string MED_DEPT_NO_FLR { get; set; } = string.Empty;           // 방번호, 층수  "[11] 3층"

    // 의사 정보
    [Origin(DTO.UNIQUE)] public string DR_STF_NO { get; set; } = string.Empty;// 의사번호
    [Origin] public string DR_NM_REAL { get; set; } = string.Empty;// 의사 이름
    [Origin(DTO.UNIQUE)] public string SPLT_MTEL_CNTE { get; set; } = string.Empty;// 전문분야, 임시로 key로 만들었음

    // 스케쥴
    [Origin] public string MED_SCH_AM { get; set; } = string.Empty;// 화,목
    [Origin] public string MED_SCH_PM { get; set; } = string.Empty;// 수,금

    // 휴진
    [Origin] public string MED_SCH_ABL_AM { get; set; } = string.Empty; // 휴진
    [Origin] public string MED_SCH_ABL_PM { get; set; } = string.Empty;
    // 휴일
    [Origin] public string MED_SCH_HDY_AM { get; set; } = string.Empty; // 휴일
    [Origin] public string MED_SCH_HDY_PM { get; set; } = string.Empty;
    // 휴가
    [Origin] public string MED_SCH_VCT_AM { get; set; } = string.Empty;  // 휴가
    [Origin] public string MED_SCH_VCT_PM { get; set; } = string.Empty;

    //------------------------
    [Origin] public string HSP_TP_CD { get; set; } = string.Empty;

    public override string ToString()
    {
      return $"{MED_DEPT_CD},의사 {DR_STF_NO}/{DR_NM_REAL}[{SPLT_MTEL_CNTE}]";
    }
  }

  #endregion etc
}