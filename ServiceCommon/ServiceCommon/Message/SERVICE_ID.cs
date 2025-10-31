using Framework.Network;

namespace ServiceCommon
{
  /// <summary>
  /// client 서비스를 처리할 주체
  /// </summary>
  enum SERVICE_TYPE
  {
    MANAGE    = 10000 * 0, // client 관리를 위한 필수 서비스
    SERVER    = 10000 * 1, // server가 client에게 명령
    CLIENT    = 10000 * 2, // client 상태
    HOSPITAL  = 10000 * 3, // 병원 서비스 요청/통지
    TICKET    = 10000 * 4,
    MESSAGING = 10000 * 5,
    FUNERAL   = 10000 * 6,
    USER      = 10000 * 9, // 사용자 요청/통지
  }

  public enum SERVICE_ID
  {
    NONE = 0,

    SVR_COMMAND = SERVICE_TYPE.MANAGE + 1,

    #region Server command
    SVR_STATUS             = SERVICE_TYPE.SERVER + 1,
    SVR_CALL_PATIENT       = SERVICE_TYPE.SERVER + 2,
    SVR_CALL_OPERATION     = SERVICE_TYPE.SERVER + 3,
    SVR_CALL_ANNOUNCE      = SERVICE_TYPE.SERVER + 4,
    SVR_BELL_CALL          = SERVICE_TYPE.SERVER + 5,

    //SVR_CMD_CONFIG_UPDATED      = SERVICE_TYPE.SERVER + 11,
    //SVR_CMD_CLIENT_UPDATE       = SERVICE_TYPE.SERVER + 12,
    //SVR_CMD_CLIENT_UPDATE_FORCE = SERVICE_TYPE.SERVER + 13,
    //SVR_CMD_ROLLBACK            = SERVICE_TYPE.SERVER + 14, // 서버 롤백 명령 (서버에서 클라이언트로 전달됨)
    //SVR_CMD_RESTART             = SERVICE_TYPE.SERVER + 15,
    //SVR_CMD_REBOOT              = SERVICE_TYPE.SERVER + 16,
    //SVR_CMD_SHUTDOWN            = SERVICE_TYPE.SERVER + 17,
    //SVR_CMD_RELOAD_DATA         = SERVICE_TYPE.SERVER + 18,

    #endregion Server command

    #region client
    // client
    CLIENT_REGISTER        = SERVICE_TYPE.CLIENT + 1,
    CLIENT_STATUS          = SERVICE_TYPE.CLIENT + 2,
    CLIENT_MEDIA_NOTI      = SERVICE_TYPE.CLIENT + 3,
    #endregion client

    #region hospital
    //===============================
    // 응급실
    //===============================
    ER_PATIENT      = SERVICE_TYPE.HOSPITAL + 10, // 응급실 환자
    ER_AREA_CONGEST = SERVICE_TYPE.HOSPITAL + 12, // 응급실 구역별 혼잡도
    ER_CONGESTION   = SERVICE_TYPE.HOSPITAL + 14, // 응급실 혼잡도
    ER_STATISTICS   = SERVICE_TYPE.HOSPITAL + 16, // 응급실 통계
    ER_CT           = SERVICE_TYPE.HOSPITAL + 18, // 응급실 CT
    ER_CPR          = SERVICE_TYPE.HOSPITAL + 20, // CPR
    ER_ISOLATION    = SERVICE_TYPE.HOSPITAL + 22, // 응급실 진료실
    //===============================
    // IPD
    //===============================
    OPERATION       = SERVICE_TYPE.HOSPITAL + 50, // 수술실
    ICU             = SERVICE_TYPE.HOSPITAL + 60, // 중환자실
    DELIVERY_ROOM   = SERVICE_TYPE.HOSPITAL + 80,
    WARD_ROOMS            = SERVICE_TYPE.HOSPITAL + 81, // 병실 현황 조회

    //=====================================
    // 외래
    //=====================================
    OFFICE_PT     = SERVICE_TYPE.HOSPITAL + 100, // 진료실
    EXAM_PT       = SERVICE_TYPE.HOSPITAL + 110, // 검사실
    INSPECTION      = SERVICE_TYPE.HOSPITAL + 120, // 검사실
    ANG             = SERVICE_TYPE.HOSPITAL + 130,  // 혈관 조영술 (Angiography)
    ENDO            = SERVICE_TYPE.HOSPITAL + 140, // 내시경(Endoscope)
    ENDO_WGO       = SERVICE_TYPE.HOSPITAL + 150, // 여성암병원 초음파실(Ultrasound, Sonography)
    RAD_SEOUL       = SERVICE_TYPE.HOSPITAL + 160, // 여성암병원 초음파실(Ultrasound, Sonography)
    RAD_TR          = SERVICE_TYPE.HOSPITAL + 161, // 방사선종양학과 (Radiation Ultrasound, Sonography)

    //=====================================
    // 기타
    //=====================================
    DR_SCH         = SERVICE_TYPE.HOSPITAL + 900, // 진료 스케쥴
    DRUG           = SERVICE_TYPE.HOSPITAL + 910, // 약제과
    CAFETERIA      = SERVICE_TYPE.HOSPITAL + 920,
    DR_PHOTO       = SERVICE_TYPE.HOSPITAL + 930, // 의료진 사진
    DR_PHOTO_NOTI  = SERVICE_TYPE.HOSPITAL + 940, // 의료진 사진 변경 알림
    #endregion hospital

    #region funeral
    FUNERAL_ROOM = SERVICE_TYPE.FUNERAL + 1, // 분향소
    #endregion funeral

    #region ticket
    TICKET_GEN         = SERVICE_TYPE.TICKET + 10,
    TICKET_GEN_PT      = SERVICE_TYPE.TICKET + 11,
    TICKET_CALL        = SERVICE_TYPE.TICKET + 20,
    TICKET_RECALL      = SERVICE_TYPE.TICKET + 30,
    TICKET_DIV_INFO    = SERVICE_TYPE.TICKET + 40,
    #endregion ticket

    #region user
    USER_MESSAGE = SERVICE_TYPE.USER + 1, // 사용자 정의 메시지 진행
    #endregion user
  }
}