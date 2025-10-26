namespace EUMC.ClientServices
{
  internal static class PackageNames
  {
    public const string OFFICE_SINGLE     = "h0"; // 진료실 중대합
    public const string OFFICE_MULTI      = "h1"; // 진료실 대대합
    public const string EXAM_SINGLE       = "h2"; // 검사실 중대합
    public const string EXAM_MULTI        = "h3"; // 검사실 대대합
    public const string ENDO              = "h4"; // 내시경
    public const string DELIVERY_ROOM     = "h5"; // 분만실

    public const string ICU_STAFF         = "i0"; // 중환자실
    public const string OPERATION         = "o0"; // 수술실

    public const string ER_PATIENT_ADULT  = "e0"; // 응급실 (성인)
    public const string ER_PATIENT_CHILD  = "e1"; // 응급실 (소아)

    public const string DRUG              = "w0"; // 약제과
    public const string PROMOTION         = "a0"; // 55인치 가로

    public const string WARD_ROOMS        = "s3"; // 병실현황
                                                  // 순번 시스템
    public const string TICKET_GEN        = "t1"; // 발권기
    public const string TICKET_DISPLAY    = "t2"; // 순번 표시기
    public const string TICKET_CALLER     = "t3"; // 호출기
    public const string TICKET_LARGE      = "t4"; // 순번 대대합
                                                  // 홍보
    public const string PROMOTION_V       = "a1"; // 세로형

    // test
    public const string NO_SCH      = "no"; // no schedule
    public const string ERROR       = "er"; // error
    public const string INFORMATION = "x2";
    public const string UPDATE      = "x3";
  }
}