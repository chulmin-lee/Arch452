namespace ServiceCommon
{
  public enum PACKAGE
  {
    NONE = 0,

    //---------------------------------------
    // 응급실
    //---------------------------------------
    ER_PATIENT = 100,
    ER_OFFICE = 101,
    ER_ISOLATION = 102,

    //---------------------------------------
    // IPD
    //---------------------------------------
    DELIVERY_ROOM = 201,
    ICU = 202,
    OPERATION = 203,
    WARD_ROOMS = 204,

    //---------------------------------------
    // OPD
    //---------------------------------------
    OFFICE_SINGLE = 301,  // 진료실
    OFFICE_MULTI = 302,
    EXAM_SINGLE = 303,    // 검사실
    EXAM_MULTI = 304,
    EXAM_MULTI_PET = 305,
    EXAM_OFFICE_MIX = 306,
    INSPECTION = 307,
    ENDO = 308,

    //---------------------------------------
    // ETC
    //---------------------------------------
    DRUG = 401,
    DR_SCH = 402,

    PROMOTION = 450,
    CAFETERIA = 460,
    //---------------------------------------
    // 장례식장
    //---------------------------------------
    FUNERAL_SINGLE = 501,
    FUNERAL_MULTI = 502,
    //---------------------------------------
    // Ticket
    //---------------------------------------
    TICKET_GEN = 601,
    TICKET_DISPLAY = 602,
    TICKET_CALLER = 603,
    TICKET_LARGE = 604,

    // internal
    NO_SCHEDULE,
    ERROR,         // 순천향
    ERROR_PACKAGE,
    UPDATER,

    INFORMATION = 700,
  }
}