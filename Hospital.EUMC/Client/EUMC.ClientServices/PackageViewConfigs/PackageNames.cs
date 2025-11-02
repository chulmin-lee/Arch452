namespace EUMC.ClientServices
{
  internal static class PackageNames
  {
    //IPD
    public const string ER_PATIENT  = "h3"; // winInfoEmergencyRoom 응급실
    public const string OPERATION   = "h1"; // winInfoOperationRoom 수술실 안내
    public const string ICU_RM      = "h2"; // winInfoICU 중환자실
    public const string ICU2_RM     = "h7"; // winInfoICU2 뇌졸중집중치료실
    public const string ICU_BABY    = "h11"; // winInfoICUBaby 신생아 중환자실

    // OPD
    public const string SINGLE_OPD  = "w1"; // 진료실/검사실 중대합 (A, B)
    public const string MULTI_OPD   = "w2"; // 진료실/검사실 대대합 (A, B, C,..)

    // OPD Exam
    // 조영실
    public const string ANG           = "h4"; // winBloodVessel 혈관조영실, PC_ANG_PT_INFO
    public const string ANG_GUARD     = "h5"; // winBVGuardian 혈관조영실 보호자용
    public const string ANG2          = "h14"; // winBloodVessel2 혈관조영실(3F), PC_ANG_PT_INFO2
    public const string ANG2_GUARD    = "h15"; // winBVGuardianToggle 혈관조영실/심뇌혈관조영실 전환형 보호자용
    public const string ANG_IMC       = "h8"; // winBloodVessel 심뇌혈관조영실 (PC_ANG_PT_INFO_IMC)
    public const string ANG_IMC_GURAD = "h9"; // winBloodVessel 심뇌혈관조영실 (PC_ANG_PT_INFO_IMC)
    // ENDO
    public const string ENDO          = "h6"; // winEndoscopy 내시경실 보호자용 대대합
    // RAD
    public const string RAD           = "r1"; // winInfoMiddleR 영상의학과 MRI/CT/초음파 중대합
    public const string RAD_LARGE     = "w5"; // winInfoRadLarge 영상의학과 검사실 대대합

    // ETC
    public const string DR_SCH  ="w4"; // winKioskMedsSchedule 진료과별 시간표 키오스크
    public const string DRUG ="h0"; // winInfoDrug 약제실 안내

    // 홍보
    public const string FULL_HD_PLAYER  = "a0"; // winFullHDPlayer FULL HD 플레이어
    public const string TOTALINFO       = "e1"; // winKioskTotalInfo 병원안내 (환자조회/시설안내)
    public const string BLDNG_WEATHER   = "e2"; // winWeather 병원홍보 날씨
    public const string BLDNG_CONTENTS  = "e3"; // winContents 병원홍보 49

    // 순번 호출기
    public const string TICKET_CALLER       = "o0"; // winCaller 순번 호출기
    public const string TICKET_CALLER_MULTI = "o1"; // MultiCallerView 수번 호출기 (다중)
    // 순번 발권기
    public const string TICKET_GEN          = "g3"; // winComboKiosk 발권기 (입·퇴원/산재·자보)
    public const string TICKET_GEN2         = "g6"; // winComboKioskForeign 발권기 (외래진료실)
    public const string TICKET_GEN_MULTI    = "g8"; // MultiTicketView 발권기 멀티

    // 순번 표시기
    public const string TICKET_DISPLAY  = "g1"; // winInfoCallMonitor 순번 표시기 (싱글)
    public const string TICKET_DISPLAY2 = "g9"; // MultiDisplayView 순번표시기 (멀티)
    // 순번 대대합
    public const string RCPT_LARGE2     = "g5"; // winInfoReceiptLarge2 순번 대대합
    public const string ONE_CALL_LARGE  = "g7"; // winInfoReceiptOneLarge 순번 대대합 (영상일반촬영)
    // test
    public const string NO_SCH      = "no"; // no schedule
    public const string ERROR       = "er"; // error
    public const string INFORMATION = "x2";
    public const string UPDATE      = "x3";
  }
}