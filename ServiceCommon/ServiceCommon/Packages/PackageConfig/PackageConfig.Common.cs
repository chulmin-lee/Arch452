namespace ServiceCommon
{
  public class ErrorPackage
  {
    public PACKAGE_ERROR ErrorCode { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
  }

  public enum PACKAGE_ERROR
  {
    Success = 0,
    //======================
    // file error
    //======================
    FileNotFound = 1,
    MissingDisplayConfig = 2,
    MissingScheduleConfig = 3,
    //======================
    // elemnt error
    //======================
    /// <summary>
    /// 필요 요소가 없음
    /// </summary>
    ElementNotFound = 4,
    /// <summary>
    /// 값 설정이 잘못됨
    /// </summary>
    InvalidValue = 5,
    UnknownPackage = 6,
  }
}