namespace ServiceCommon
{
  public class CALL_PATIENT_NOTI : ServiceMessage
  {
    public string PatientName { get; set; } = string.Empty;
    public string PatientNameTTS { get; set; } = string.Empty;
    public string CallMessage { get; set; } = string.Empty;
    public PACKAGE Package { get; set; }
    public string Speech => $"{this.PatientNameTTS}님 {this.CallMessage}";
    public CALL_PATIENT_NOTI() : base(SERVICE_ID.SVR_CALL_PATIENT) { }
  }

  public class CALL_OPERATION_NOTI : ServiceMessage
  {
    public string CallMessage1 { get; set; } = string.Empty;
    public string CallMessage2 { get; set; } = string.Empty;
    public CALL_OPERATION_NOTI() : base(SERVICE_ID.SVR_CALL_OPERATION) { }
  }

  public class CALL_ANNOUNCE_NOTI : ServiceMessage
  {
    public string Message { get; set; } = string.Empty;
    public CALL_ANNOUNCE_NOTI() : base(SERVICE_ID.SVR_CALL_ANNOUNCE) { }
    public CALL_ANNOUNCE_NOTI(string m) : this()
    {
      this.Message = m;
    }
  }

  public class CALLL_BELL_NOTI : ServiceMessage
  {
    public string Message { get; set; } = string.Empty;
    public CALLL_BELL_NOTI() : base(SERVICE_ID.SVR_BELL_CALL) { }
    public CALLL_BELL_NOTI(string m) : this()
    {
      this.Message = m;
    }
  }
}