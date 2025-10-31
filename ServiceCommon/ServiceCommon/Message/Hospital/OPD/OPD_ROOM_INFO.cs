using Common;
using System.Collections.Generic;

namespace ServiceCommon
{
  public class OPD_ROOM_INFO : IGroupKeyData<string>
  {
    public string GroupKey => this.Room.GroupKey;
    public string DeptCode => this.Room.DeptCode;
    public string RoomCode => this.Room.RoomCode;
    public string DoctorNo => this.Doctor.DoctorNo;

    public ROOM_INFO Room { get; set; } = new ROOM_INFO();
    public DOCTOR_INFO Doctor { get; set; } = new DOCTOR_INFO();
    public PATIENT_INFO RoomPatient { get; set; }
    public List<PATIENT_INFO> WaitPatients { get; set; } = new List<PATIENT_INFO>();

    public void Clear()
    {
      this.Room.UseRoom = false;
      this.Doctor = new DOCTOR_INFO();
      this.RoomPatient = null;
      this.WaitPatients.Clear();
    }
  }

  /// <summary>
  /// 진료실/검사실 정보
  /// </summary>
  public class ROOM_INFO
  {
    public string DeptCode { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string DurationTime { get; set; } = string.Empty;
    public string DelayReason { get; set; } = string.Empty;
    public string DelayTime { get; set; } = string.Empty;

    public string AssistantName { get; set; } = string.Empty;
    public bool UseRoom { get; set; } = true;
    public string GroupKey => $"{this.DeptCode}:{this.RoomCode}";
  }
  /// <summary>
  /// 진료실 의사 정보
  /// </summary>
  public class DOCTOR_INFO
  {
    public string DoctorNo { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorDeptName { get; set; } = string.Empty;
    public string SpecialPart { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
  }
  /// <summary>
  /// 환자 호출
  /// </summary>
  public enum PATIENT_CALL_TYPE
  {
    NoCall  = 0, // 호출하지 않음
    Patient = 1, // 환자 호출
    Guard   = 2, // 보호자 호출
    All     = 3,
  }
  /// <summary>
  /// 환자 정보
  /// </summary>
  public class PATIENT_INFO
  {
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    // 환자상태
    public int WaitNo { get; set; }
    public string StateCode { get; set; } = string.Empty;
    public int ReserveTime { get; set; }  // 예약 시간  "0930" : 930, "1015": 1015, 없으면 0
    public bool InRoom { get; set; }
  }
}