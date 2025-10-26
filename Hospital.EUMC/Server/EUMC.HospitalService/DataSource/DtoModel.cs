using Framework.DataSource;
using Newtonsoft.Json;
using System.IO;

namespace EUMC.HospitalService
{
  #region OPD
  public class OFFICE_DTO : OriginDataModel
  {
    // room
    [Origin(DTO.UNIQUE)] public string deptNm { get; set; } = string.Empty; // 부서명
    [Origin(DTO.UNIQUE)] public string roomCd { get; set; } = string.Empty; // 진료실코드
    [Origin] public string roomNm { get; set; } = string.Empty; // 진료실명
    [Origin] public string smallRoomNm { get; set; } = string.Empty; // 2025/02/23 진료실명
    [Origin] public string nurseNm { get; set; } = string.Empty; // 담당직원명

    // doctor
    [Origin] public string ordDrId { get; set; } = string.Empty; // 의사번호
    [Origin] public string ordDrNm { get; set; } = string.Empty; // 의사이름
    [Origin] public string drImage { get; set; } = string.Empty; // 의사 사진 경로

    // patient
    [Origin(DTO.UNIQUE)] public string pid { get; set; } = string.Empty; // 환자번호
    [Origin] public string hnNm { get; set; } = string.Empty; // 환자이름
    [Origin] public string ttsHngNm { get; set; } = string.Empty; // 2025/02/23 환자이름
    [Origin] public string ordStat { get; set; } = string.Empty; // 진료중여부
    [Origin] public string waitSeq { get; set; } = string.Empty; // 대기순서

    //[ModelMember] public string displayYn { get; set; } = string.Empty; // 전광판 사용여부
    [Origin] public string noticeMsg { get; set; } = string.Empty; // 공지사항
                                                                   //[ModelMember] public string ttsYn { get; set; } = string.Empty; // TTS 사용여부
    [Origin] public string callYn { get; set; } = string.Empty; // 호출 사용여부
    [Origin] public string ordPart { get; set; } = string.Empty; // 의사분야

    //==================
    // 검색할때 추가할것
    [Origin] public string useRoomYn { get; set; } = string.Empty; // 호출 사용여부
    public string DeptCode { get; set; } = string.Empty;
  }

  public class PHOTO_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string STF_NO { get; set; } = string.Empty;
    [Origin] public string IMAGE_PATH { get; set; } = string.Empty;
    public string HSP_TP_CD { get; set; } = string.Empty;
    //=========================
    // 추가
    [JsonIgnore]
    public bool Success { get; set; }
    //public string Filename { get; set; } = string.Empty;
    public string GetFilename()
    {
      return Path.GetFileName(this.IMAGE_PATH);
      //if (string.IsNullOrEmpty(this.Filename))
      //{
      //  return string.IsNullOrWhiteSpace(this.IMAGE_PATH) ? string.Empty
      //    : this.IMAGE_PATH.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
      //}
      //return this.Filename;
    }
  }

  public class EXAM_DTO : OriginDataModel
  {
    // room
    [Origin(DTO.UNIQUE)] public string suppDeptCd { get; set; } = string.Empty; // 부서코드
    [Origin(DTO.UNIQUE)] public string ExcuRoomCd { get; set; } = string.Empty; // 검사실코드
    [Origin(DTO.UNIQUE)] public string EXCUROOMNM { get; set; } = string.Empty; // 검사실코드
                                                                                // patient
    [Origin(DTO.UNIQUE)] public string pid { get; set; } = string.Empty; // 환자 번호
    [Origin] public string hngNm { get; set; } = string.Empty; // 환자이름
    [Origin] public string ttsHngNm { get; set; } = string.Empty; // 환자이름
    [Origin] public string sex { get; set; } = string.Empty;
    [Origin] public string age { get; set; } = string.Empty; // 환자이름
                                                             // patient 상태
    [Origin] public string seq { get; set; } = string.Empty; // 순서
    [Origin] public string AcptPrcpYn { get; set; } = string.Empty; // 접수 여부

    // 검사실 - 1 : 접수 / 2 : 검사중 / P : 부분실시(안정중) / 4 : 완료
    // 내시경실 - 1 : 준비중 / 2 : 수술중 / 3 : 회복중 / 4 : 완료"
    [Origin] public string execStatFlag { get; set; } = string.Empty;
    [Origin] public string RsrvTm { get; set; } = string.Empty; // 예약시간
    [Origin] public string CallDt { get; set; } = string.Empty; // 호출시간
    [Origin] public string CallPat { get; set; } = string.Empty; // 호출 대상 (환자 or 보호자)
                                                                 // 나중에 제거
    [Origin] public string PrcpDd { get; set; } = string.Empty; // 처방일자
    [Origin] public string EndFlag { get; set; } = string.Empty; // 종료 구분
    [Origin] public string ExamDt { get; set; } = string.Empty; // 실시시간
    [Origin] public string AcptDt { get; set; } = string.Empty; // 접수시간
  }

  public class INSPECTION_DTO : OriginDataModel
  {
    // patient
    [Origin(DTO.UNIQUE)] public string pid { get; set; } = string.Empty; // 환자 번호
    [Origin] public string hngNm { get; set; } = string.Empty; // 환자이름
    [Origin(DTO.UNIQUE)] public string delayTm { get; set; } = string.Empty; // 지연시간
    [Origin(DTO.UNIQUE)] public string gubun { get; set; } = string.Empty; // 구분
  }

  public class NAME_PLATE_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string wardCd { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string roomCd { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public string bedCd { get; set; } = string.Empty;
    [Origin] public string wardType { get; set; } = string.Empty; //(A:일반변동, B:중환자실, C:NICU, D:분만병동, E:응급실)
    [Origin] public string pid { get; set; } = string.Empty;
    [Origin] public string hngNm { get; set; } = string.Empty;
    [Origin] public string age { get; set; } = string.Empty;
    [Origin] public string sex { get; set; } = string.Empty; // 값: F/M
    [Origin] public string ordDeptCd { get; set; } = string.Empty;
    [Origin] public string ordDrNm { get; set; } = string.Empty;
  }

  public class DELIVERY_ROOM_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string pid { get; set; } = string.Empty;
    [Origin] public string hngNm { get; set; } = string.Empty;
    [Origin] public string location { get; set; } = string.Empty;
    [Origin] public string childbirth { get; set; } = string.Empty;
    [Origin] public string babyGender { get; set; } = string.Empty;
  }

  public class ENDO_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string pid { get; set; } = string.Empty;
    [Origin] public string hngNm { get; set; } = string.Empty;
    [Origin] public int stateCd { get; set; }
  }

  public class WARD_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string area_code { get; set; } = string.Empty;
    [Origin(DTO.UNIQUE)] public int floor { get; set; }
    [Origin(DTO.UNIQUE)] public string room_code { get; set; } = string.Empty;
    [Origin] public int capacity { get; set; }
    [Origin] public string assistant { get; set; } = string.Empty;

    [Origin(DTO.UNIQUE)] public string pt_no { get; set; } = string.Empty;
    [Origin] public string pt_nm { get; set; } = string.Empty;
    [Origin] public string sex { get; set; } = string.Empty;
    [Origin] public int age { get; set; }

    [Origin] public string fall { get; set; } = "N";
    [Origin] public string fire { get; set; } = "N";
    [Origin] public string surgery { get; set; } = "N";
  }

  #endregion OPD

  #region IPD
  public class ER_PATIENT_DTO : OriginDataModel
  {
    //  위치
    [Origin] public string deptNm { get; set; } = string.Empty; // 부서명
    [Origin] public string bed { get; set; } = string.Empty; // 구역 예) 소생실, 응급환자구역5 |

    // 환자
    [Origin(DTO.UNIQUE)] public string pid { get; set; } = string.Empty; // 환자번호
    [Origin] public string hngNm { get; set; } = string.Empty; // 환자명
    [Origin] public string ktasFlag { get; set; } = string.Empty; // 중증도
    [Origin] public string childYn { get; set; } = string.Empty; // 소아여부
    [Origin] public string sexAge { get; set; } = string.Empty; //
    [Origin] public string bedCd { get; set; } = string.Empty; //
    [Origin] public string first_visit { get; set; } = "Y";
    [Origin] public string jinryoStat { get; set; } = "0"; // 0~2 진료
    [Origin] public string bloodStat { get; set; } = "0"; // 0~3 혈액검사 여부
    [Origin] public string collaboStat { get; set; } = "0"; // 0~2 협진 여부
    [Origin] public string ratStat { get; set; } = "0"; // 0~2 영상검사 여부
    [Origin] public string inoutStat { get; set; } = "0"; // 0~2 입퇴원
    [Origin] public string wardStat { get; set; } = "0"; // 0~2 병실배정
  }
  /// <summary>
  /// 응급실 혼잡도
  /// </summary>
  public class ER_CONGESTION_DTO : OriginDataModel
  {
    [Origin] public int bedCnt { get; set; }  // 총침상수
    [Origin] public int satPer { get; set; }  // 퍼센트
    [Origin] public int inCnt { get; set; }   // 재원환자
    [Origin(DTO.UNIQUE)] public string isChild { get; set; } = string.Empty; // 재원환자
  }

  /// <summary>
  /// 구역별(성인/소아) 혼잡도
  /// </summary>
  public class ER_AREA_CONGEST_DTO : OriginDataModel
  {
    [Origin] public string totalBedCnt { get; set; } = string.Empty;  // 총침상수
    /*
    구역
    O001 : 성인 일반
    O002 : 소아 일반
    O003 : 음압 격리 (안씀)
    O004 : 일반 격리 (안씀)
    O046 : 음압 격리 (사용)
    O047 : 일반 격리 (사용)
    O048 : 소아 음압 격리
    O049 : 소아 일반 격리
    */
    [Origin(DTO.UNIQUE)] public string ertrsmType { get; set; } = string.Empty; // 구역
    [Origin] public string satPer { get; set; } = string.Empty; // 퍼센트
    [Origin] public string inCnt { get; set; } = string.Empty; // 재원환자
    [Origin] public string isChild { get; set; } = string.Empty; // 재원환자
  }

  public class ER_ISOLATION_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string bedCd { get; set; } = string.Empty;
    [Origin] public string bed { get; set; } = string.Empty;  // room name
    [Origin] public string deptCd { get; set; } = string.Empty;
    [Origin] public string deptNm { get; set; } = string.Empty;
    [Origin] public string pid { get; set; } = string.Empty;
    [Origin] public string hngNm { get; set; } = string.Empty;
    [Origin] public string sexAge { get; set; } = string.Empty;
  }

  public class OPERATION_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string wardCd { get; set; } = string.Empty; // 병동코드
    [Origin(DTO.UNIQUE)] public string roomCd { get; set; } = string.Empty; // 병실코드
    [Origin(DTO.UNIQUE)] public string pid { get; set; } = string.Empty; // 환자번호
    [Origin] public string hngNm { get; set; } = string.Empty; // 환자이름
    [Origin] public string ttsHngNm { get; set; } = string.Empty; // TTS용 환자이름
    [Origin] public string sex { get; set; } = string.Empty; // 성별 (F / M)
    [Origin] public string age { get; set; } = string.Empty; // 성별 (F / M)
    [Origin] public string perfDeptCd { get; set; } = string.Empty; // 집도과 코드
    [Origin] public string perfDrNm { get; set; } = string.Empty; // 집도의
    [Origin] public string deptNm { get; set; } = string.Empty; // 부서명
    [Origin] public string stateCd { get; set; } = string.Empty; // 수술 상태 (01: 준비중, 02: 수술중, 03: 회복중)
    [Origin] public string patposplceCd { get; set; } = string.Empty; // 이동위치
    [Origin] public string callFlag { get; set; } = string.Empty; // 호출여부 (Y

    //2025/09/10 SCH 수술실 이름
    [Origin] public string opRmNm { get; set; } = string.Empty; // 수술실이름
  }

  public class ICU_DTO : OriginDataModel
  {
    [Origin] public string wardCd { get; set; } = string.Empty; // 부서코드
    [Origin] public string wardNm { get; set; } = string.Empty; // 부서명
    [Origin(DTO.UNIQUE)] public string deptCd { get; set; } = string.Empty; // 진료과코드
    [Origin] public string deptNm { get; set; } = string.Empty; // 진료과명
    [Origin] public string roomCd { get; set; } = string.Empty; // 중환자실유형 예) MICU, MECU

    //  환자
    [Origin(DTO.UNIQUE)] public string pid { get; set; } = string.Empty; // 환자번호
    [Origin] public string hngNm { get; set; } = string.Empty; // 환자이름
    [Origin] public string sexAge { get; set; } = string.Empty; // 성별/나이
                                                                //[ModelMember] public string brthDd { get; set; } = string.Empty; // 생년월일

    [Origin] public string medispclNm { get; set; } = string.Empty; // 의사이름

    [Origin] public string patStat { get; set; } = string.Empty; // 환자상태
    [Origin] public string bedCd { get; set; } = string.Empty; // 배드번호
    [Origin] public string inftInfo { get; set; } = string.Empty; // 감염정보 (R:보호, A:공기, B:혈액, C:접촉, D:비말, E:신종호흡기)
    public string Delimeter { get; set; } = "^";
  }
  #endregion IPD

  #region etc

  public class DRUG_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string dispDrugno { get; set; } = string.Empty; // 투약번호
    [Origin] public string hngNm { get; set; } = string.Empty; // 환자명
    [Origin(DTO.UNIQUE)] public string pid { get; set; } = string.Empty; // 환자번호
    [Origin] public string dispFlag { get; set; } = string.Empty; // D:조제완료, M:투약완료, S:메세지, C:호출
    [Origin] public string DrugFlag { get; set; } = string.Empty; // 투약구분 (Y:원외, O:원내, M:Admission, R:정규, A:추가, L:퇴원, E:응급실, W:병동응급, D:응급실퇴원원외, P:PRN, S:특수조제실)
    [Origin] public string orgDrugNo { get; set; } = string.Empty;
    [Origin] public string ordDeptNm { get; set; } = string.Empty; // 진료과명
    [Origin] public string ttsDrugno { get; set; } = string.Empty; //
    [Origin] public string ttsDrugFlag { get; set; } = string.Empty; //
    [Origin] public string comments { get; set; } = string.Empty; //

    // 조제중/조제완료 구분용 필드. 데이타 조회 조건에 따라 설정한다
    public bool IsDone { get; set; }
  }
  public class DR_SCH_DTO : OriginDataModel
  {
    [Origin(DTO.UNIQUE)] public string basedd { get; set; } = string.Empty; // 스케쥴일자
    [Origin] public string orddeptcd { get; set; } = string.Empty; // dept code
    [Origin] public string orddeptcdnm { get; set; } = string.Empty; // dept name
    [Origin(DTO.UNIQUE)] public string orddrid { get; set; } = string.Empty; // 의사번호
    [Origin] public string orddridnm { get; set; } = string.Empty; // 의사이름
    [Origin] public string specialdept { get; set; } = string.Empty; // 전문분야
    [Origin(DTO.UNIQUE)] public int week { get; set; } // 요일이네
    [Origin] public string amordyn { get; set; } = string.Empty; // 오전 진료 Y/N
    [Origin] public string pmordyn { get; set; } = string.Empty; // 오후 진료 Y/N
  }

  #endregion etc
}