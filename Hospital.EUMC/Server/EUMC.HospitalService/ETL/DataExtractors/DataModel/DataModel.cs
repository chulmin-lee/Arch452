using ServiceCommon;
using System;

namespace EUMC.HospitalService
{
  #region Base
  internal class DEPT_MASTER_POCO
  {
    public string DeptCode { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
  }
  #endregion


  internal class OFFICE_ROOM_POCO
  {
    public string DeptCode { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string DoctorNo { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string DelayReason { get; set; } = string.Empty;
    public string DelayTime { get; set; } = string.Empty;
    public string GroupKey => $"{this.DeptCode}:{this.RoomCode}";
  }
  internal class OFFICE_PT_POCO
  {
    public string DeptCode { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public int Rank { get; set; }
    public string Floor { get; set; } = string.Empty;
    public string PactId { get; set; } = string.Empty;
    public bool InRoom { get; set; }
    public string GroupKey => $"{this.DeptCode}:{this.RoomCode}";
  }
  internal class DR_PHOTO_POCO
  {
    public string DoctorNo { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
  }
  internal class EXAM_DEPT_POCO
  {
    public string DeptName { get; set; } = string.Empty;
    public string DeptCode { get; set; } = string.Empty;
  }
  internal class EXAM_ROOM_POCO
  {
    public string DeptCode { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string DelayReason { get; set; } = string.Empty;
    public string DelayTime { get; set; } = string.Empty;
    public string GroupKey => $"{this.DeptCode}:{this.RoomCode}";
  }
  internal class EXAM_STAFF_POCO
  {
    public string DeptCode { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string DoctorNo{ get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;

    //public string DelayReason { get; set; } = string.Empty;  // 사용안함
    //public string DelayTime { get; set; } = string.Empty;  // 사용안함
    public string GroupKey => $"{this.DeptCode}:{this.RoomCode}";
  }
  internal class EXAM_PT_POCO
  {
    public string DeptName { get; set; } = string.Empty; // 없다
    public string DeptCode { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty; // 없다
    public string PatientNo { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public int WaitNo { get; set; }
    public string GroupKey => $"{this.DeptCode}:{this.RoomCode}";
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