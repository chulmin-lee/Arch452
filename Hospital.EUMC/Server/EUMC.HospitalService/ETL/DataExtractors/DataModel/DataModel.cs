using ServiceCommon;
using System;

namespace EUMC.HospitalService
{
  internal class OFFICE_POCO
  {
    public string DeptCode { get; set; } = string.Empty;  // 원본에는 없다
    public string DeptName { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string ShortRoomName { get; set; } = string.Empty;
    public string AssistantName { get; set; } = string.Empty;
    // doctor
    public string DoctorNo { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorPart { get; set; } = string.Empty;
    // patient
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string PatientNameTTS { get; set; } = string.Empty;
    public int WaitNo { get; set; }
    public bool InRoom { get; set; }
    public bool IsCall { get; set; }
    public string CallMessage { get; set; } = string.Empty;
    public bool UseRoom { get; set; }
  }
  internal class PHOTO_POCO
  {
    public string DoctorNo { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string RoomKey { get; set; } = string.Empty;
  }
  internal class EXAM_POCO
  {
    // room
    public string DeptName { get; set; } = string.Empty; // 없다
    public string DeptCode { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty; // 없다
    public string AssistantName { get; set; } = string.Empty; // 없다
    public string Gender { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    // patient
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string PatientNameTTS { get; set; } = string.Empty;

    // patient 상태
    public int WaitNo { get; set; }
    public bool InRoom { get; set; }
    public PATIENT_CALL_TYPE CallType { get; set; }
    public string StateCode { get; set; } = string.Empty;
    public string StateName { get; set; } = string.Empty;
    public int ReserveTime { get; set; }
    public string CallTime { get; set; } = string.Empty;
  }
  public class ER_ISOLATION_POCO
  {
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;

    public string DeptCode { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
  }
  internal class DR_SCH_POCO
  {
    public string DeptCode { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string DoctorNo { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string SpecialPart { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday;
    public bool AM { get; set; }
    public bool PM { get; set; }

    public override string ToString()
    {
      return $"[{DeptName}]{DoctorName}:{DoctorNo} - {DayOfWeek} AM: {PM},  AM: {PM}";
    }
  }

  internal class WARD_POCO
  {
    public int Floor { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public int Capacity { get; set; } // 수용인원. 1 = 1인실
    public string Assistant { get; set; } = string.Empty; // 담당 간호사
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string PatientMaskedName { get; set; } = string.Empty;
    public bool IsMale { get; set; }  // true: 남, false: 여
    public int Age { get; set; }

    // 병실 환자 구분
    public bool Fall { get; set; }  // 낙상
    public bool Fire { get; set; } // 화재
    public bool Surgery { get; set; }  // 수술

    public bool IsMatch(int floor, string area, string roomCode)
    {
      return this.Floor == floor && this.AreaCode == area && this.RoomCode == roomCode;
    }

    public override string ToString()
    {
      return $"{this.Floor}:{this.AreaCode} : {RoomCode}";
    }
  }
}