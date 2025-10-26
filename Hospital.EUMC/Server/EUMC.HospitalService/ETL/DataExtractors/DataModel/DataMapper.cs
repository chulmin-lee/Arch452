using AutoMapper;
using Common;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.HospitalService
{
  internal static class DataMapper
  {
    static IMapper _mapper;
    public static IMapper Mapper => _mapper ?? (_mapper = create_mapper());

    static IMapper create_mapper()
    {
      var config = new MapperConfiguration(cfg =>
    {
      // common
      cfg.CreateMap<string, int>().ConvertUsing(s => Convert.ToInt32(s));
      cfg.CreateMap<string, bool>().ConvertUsing(s => StringToBoolean(s));

      //===================================================
      // raw data  to POCO
      //===================================================
      // office
      cfg.CreateMap<OFFICE_DTO, OFFICE_POCO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.DeptCode))  // 임의로 추가했음. 조회할때 추가할것
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.deptNm))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.roomCd))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.roomNm))
            .ForMember(dest => dest.ShortRoomName, opt => opt.MapFrom(src => src.smallRoomNm))
            .ForMember(dest => dest.AssistantName, opt => opt.MapFrom(src => src.nurseNm))
            .ForMember(dest => dest.DoctorNo, opt => opt.MapFrom(src => src.ordDrId))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.ordDrNm))
            .ForMember(dest => dest.DoctorPart, opt => opt.MapFrom(src => src.ordPart))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => PatientNo(src.pid)))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.hnNm))
            .ForMember(dest => dest.PatientNameTTS, opt => opt.MapFrom(src => src.ttsHngNm))
            .ForMember(dest => dest.WaitNo, opt => opt.MapFrom(src => src.waitSeq))
            .ForMember(dest => dest.InRoom, opt => opt.MapFrom(src => src.ordStat))
            .ForMember(dest => dest.IsCall, opt => opt.MapFrom(src => src.callYn))
            .ForMember(dest => dest.UseRoom, opt => opt.MapFrom(src => src.useRoomYn))
            .ForMember(dest => dest.CallMessage, opt => opt.MapFrom(src => src.noticeMsg));

      // dr photo
      cfg.CreateMap<PHOTO_DTO, PHOTO_POCO>()
            .ForMember(dest => dest.DoctorNo, opt => opt.MapFrom(src => src.STF_NO))  // 임의로 추가했음. 조회할때 추가할것
            .ForMember(dest => dest.Filename, opt => opt.MapFrom(src => src.GetFilename()));

      // exam
      cfg.CreateMap<EXAM_DTO, EXAM_POCO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.suppDeptCd))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.ExcuRoomCd))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.EXCUROOMNM))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => PatientNo(src.pid)))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.hngNm))
            .ForMember(dest => dest.PatientNameTTS, opt => opt.MapFrom(src => src.ttsHngNm))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.sex))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.age))
            .ForMember(dest => dest.WaitNo, opt => opt.MapFrom(src => src.seq))
            .ForMember(dest => dest.InRoom, opt => opt.MapFrom(src => src.seq == "0"))
            .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => src.execStatFlag))
            .ForMember(dest => dest.CallType, opt => opt.MapFrom(src => PatientCallType(src.CallPat)))
            .ForMember(dest => dest.ReserveTime, opt => opt.MapFrom(src => ReserveTime(src.RsrvTm)))
            .ForMember(dest => dest.CallTime, opt => opt.MapFrom(src => src.CallDt));

      // 응급진단검사
      cfg.CreateMap<INSPECTION_DTO, INSPECTION_INFO>()
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => PatientNo(src.pid)))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.hngNm))
            .ForMember(dest => dest.Gubun, opt => opt.MapFrom(src => src.gubun))
            .ForMember(dest => dest.DelayTime, opt => opt.MapFrom(src => src.delayTm));

      // 응급실
      cfg.CreateMap<ER_PATIENT_DTO, EMERGENCY_INFO>()
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.deptNm))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.bedCd))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.bed))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.hngNm))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => PatientNo(src.pid)))
            .ForMember(dest => dest.KtasCode, opt => opt.MapFrom(src => EmergencyKtas(src.ktasFlag)))
            .ForMember(dest => dest.KtasName, opt => opt.MapFrom(src => src.ktasFlag))
            .ForMember(dest => dest.IsChild, opt => opt.MapFrom(src => StringToBoolean(src.childYn)))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => EmergencyGender(src.sexAge)))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => EmergencyAge(src.sexAge)))

            .ForMember(dest => dest.IsFirstVisit, opt => opt.MapFrom(src => src.first_visit))
            .ForMember(dest => dest.RadioStateCode, opt => opt.MapFrom(src => ErRadioState(src.ratStat)))
            .ForMember(dest => dest.BloodStateCode, opt => opt.MapFrom(src => ErBloodState(src.bloodStat)))
            .ForMember(dest => dest.CollaboStateCode, opt => opt.MapFrom(src => ErCollaboState(src.collaboStat)))
            .ForMember(dest => dest.MedicalStateCode, opt => opt.MapFrom(src => ErMedicalCareState(src.jinryoStat)))
            .ForMember(dest => dest.HospitalStateCode, opt => opt.MapFrom(src => ErHospitalState(src.inoutStat)))
            .ForMember(dest => dest.WardStateCode, opt => opt.MapFrom(src => ErWardState(src.wardStat)))
            ;

      // 응급격리실
      cfg.CreateMap<ER_ISOLATION_DTO, ER_ISOLATION_POCO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.deptCd))
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.deptNm))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.bedCd))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.bed))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.hngNm))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => PatientNo(src.pid)))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => EmergencyGender(src.sexAge)))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => EmergencyAge(src.sexAge)));

      cfg.CreateMap<ER_ISOLATION_POCO, EMERGENCY_INFO>();

        // 응급실 혼잡도
      cfg.CreateMap<ER_CONGESTION_DTO, ER_CONGESTION_INFO>()
            .ForMember(dest => dest.TotalBedCount, opt => opt.MapFrom(src => src.bedCnt))
            .ForMember(dest => dest.TotalPercent, opt => opt.MapFrom(src => src.satPer))
            .ForMember(dest => dest.TotalInBedCount, opt => opt.MapFrom(src => src.inCnt))
            .ForMember(dest => dest.IsChild, opt => opt.MapFrom(src => src.isChild));

      // 응급실 구역별 혼잡도
      cfg.CreateMap<ER_AREA_CONGEST_DTO, ER_AREA_CONGEST_INFO>()
            .ForMember(dest => dest.AreaCode, opt => opt.MapFrom(src => ErCongestAreaCode(src.ertrsmType)))
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => ErCongestAreaName(src.ertrsmType)))
            .ForMember(dest => dest.BedCount, opt => opt.MapFrom(src => src.totalBedCnt))
            .ForMember(dest => dest.InBedCount, opt => opt.MapFrom(src => src.inCnt))
            .ForMember(dest => dest.Percent, opt => opt.MapFrom(src => src.satPer))
            .ForMember(dest => dest.IsChild, opt => opt.MapFrom(src => src.isChild));

      // 수술실
      cfg.CreateMap<OPERATION_DTO, OPERATION_INFO>()
            .ForMember(dest => dest.BuildingNo, opt => opt.MapFrom(src => src.wardCd))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.roomCd))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.pid))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.hngNm))
            .ForMember(dest => dest.PatientNameTTS, opt => opt.MapFrom(src => src.ttsHngNm))
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.deptNm))
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.perfDeptCd))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.perfDrNm))
            .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => src.stateCd))
            .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => OperationStateName(src.stateCd)))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => OperationGender(src.sex)))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.age))
            .ForMember(dest => dest.LocationCode, opt => opt.MapFrom(src => src.patposplceCd))
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => OperationLocation(src.patposplceCd)))
            .ForMember(dest => dest.CallPatient, opt => opt.MapFrom(src => src.callFlag))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.opRmNm));

      // 중환자실
      cfg.CreateMap<ICU_DTO, ICU_INFO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.deptCd))
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.deptNm))
            .ForMember(dest => dest.WardCode, opt => opt.MapFrom(src => src.wardCd))
            .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.wardNm))
            .ForMember(dest => dest.IcuCode, opt => opt.MapFrom(src => src.roomCd))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => PatientNo(src.pid)))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.hngNm))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => IcuGender(src.sexAge)))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => IcuAge(src.sexAge)))
            .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => src.patStat))
            .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.patStat))
            .ForMember(dest => dest.BedNo, opt => opt.MapFrom(src => src.bedCd))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.medispclNm))
            .ForMember(dest => dest.Infections, opt => opt.MapFrom(src => src.inftInfo))
            .ForMember(dest => dest.Delimeter, opt => opt.MapFrom(src => src.Delimeter));

           // 약제과
      cfg.CreateMap<DRUG_DTO, DRUG_INFO>()
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.ordDeptNm))
            .ForMember(dest => dest.DrugNo, opt => opt.MapFrom(src => src.dispDrugno))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => PatientNo(src.pid)))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.hngNm))
            .ForMember(dest => dest.IsDone, opt => opt.MapFrom(src => src.IsDone));

           // 진료 스케쥴
      cfg.CreateMap<DR_SCH_DTO, DR_SCH_POCO>()
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.orddeptcdnm))
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.orddeptcd))
            .ForMember(dest => dest.DoctorNo, opt => opt.MapFrom(src => src.orddrid))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.orddridnm))
            .ForMember(dest => dest.SpecialPart, opt => opt.MapFrom(src => src.specialdept))
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src =>  ToDayOfWeek(src.week)))
            .ForMember(dest => dest.AM, opt => opt.MapFrom(src => src.amordyn))
            .ForMember(dest => dest.PM, opt => opt.MapFrom(src => src.pmordyn));

      cfg.CreateMap<ENDO_DTO, ENDO_INFO>()
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.pid))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.hngNm))
            .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => EndoStateCode(src.stateCd)))
            .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => EndoStateName(src.stateCd)))
            ;

      cfg.CreateMap<DELIVERY_ROOM_DTO, DELIVERY_INFO>()
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.pid))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.hngNm))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.location))
            .ForMember(dest => dest.IsChildBirth, opt => opt.MapFrom(src => src.childbirth))
            .ForMember(dest => dest.IsMale, opt => opt.MapFrom(src => IsMale(src.babyGender)));

      cfg.CreateMap<WARD_DTO, WARD_POCO>()
            .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.floor))
            .ForMember(dest => dest.AreaCode, opt => opt.MapFrom(src => src.area_code))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.room_code))
            .ForMember(dest => dest.Assistant, opt => opt.MapFrom(src => src.assistant))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.pt_no))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.pt_nm))
            .ForMember(dest => dest.IsMale, opt => opt.MapFrom(src => IsMale(src.sex)))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.age))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.age))
            .ForMember(dest => dest.Fall, opt => opt.MapFrom(src => src.fall))
            .ForMember(dest => dest.Fire, opt => opt.MapFrom(src => src.fire))
            .ForMember(dest => dest.Surgery, opt => opt.MapFrom(src => src.surgery));

      //===================================================
      // POCO to Message Data
      //===================================================
      // office
      cfg.CreateMap<OFFICE_POCO, ROOM_INFO>();
      cfg.CreateMap<OFFICE_POCO, DOCTOR_INFO>()
         .ForMember(dest => dest.SpecialPart, opt => opt.MapFrom(src => CommaStringToNewLineString(src.DoctorPart)))
         ;
      cfg.CreateMap<OFFICE_POCO, PATIENT_INFO>();
      // exam
      cfg.CreateMap<EXAM_POCO, ROOM_INFO>();
      cfg.CreateMap<EXAM_POCO, PATIENT_INFO>();

      // ward
      cfg.CreateMap<WARD_POCO, AREA_WARD_INFO.AREA_WARD>();
      cfg.CreateMap<WARD_POCO, AREA_WARD_INFO.WARD_PATIENT>();
    });
      return config.CreateMapper();
    }
    static ENDO_INFO.STATE EndoStateCode(int code)
    {
      switch (code)
      {
        case 1: return ENDO_INFO.STATE.Waiting;
        case 2: return ENDO_INFO.STATE.Inspecting;
        case 3: return ENDO_INFO.STATE.Recovering;
      }
      return ENDO_INFO.STATE.None;
    }
    static string EndoStateName(int code)
    {
      switch (code)
      {
        case 1: return "대기중";
        case 2: return "검사중";
        case 3: return "회복중";
      }
      return "";
    }
    static string CommaStringToNewLineString(string s)
    {
      return s.Replace(",", Environment.NewLine);
    }
    static string EmergencyGender(string s)
    {
      if (s.IndexOf("/") > 0)
      {
        return s.Split('/')[0];
      }
      return s;
    }
    static string EmergencyAge(string s)
    {
      if (s.IndexOf("/") > 0)
      {
        return s.Split('/')[1];
      }
      return s;
    }
    static ER_KTAS EmergencyKtas(string s)
    {
      if (int.TryParse(s, out var ktas))
      {
        switch (ktas)
        {
          case 1:
          case 2: return ER_KTAS.High;
          case 3: return ER_KTAS.Medium;
          case 4:
          case 5: return ER_KTAS.Low;
        }
      }
      return ER_KTAS.None;
    }
    static ER_RADIO_STATE ErRadioState(string s)
    {
      switch (s)
      {
        case "0": return ER_RADIO_STATE.None;
        case "1": return ER_RADIO_STATE.Wait;
        case "2": return ER_RADIO_STATE.Finished;
      }
      return ER_RADIO_STATE.None;
    }
    static ER_BLOOD_STATE ErBloodState(string s)
    {
      switch (s)
      {
        case "0": return ER_BLOOD_STATE.None;
        case "1": return ER_BLOOD_STATE.Progress;
        case "2": return ER_BLOOD_STATE.Inspecting;
        case "3": return ER_BLOOD_STATE.Finished;
      }
      return ER_BLOOD_STATE.None;
    }
    static ER_MEDICAL_STATE ErMedicalCareState(string s)
    {
      switch (s)
      {
        case "0": return ER_MEDICAL_STATE.None;
        case "1": return ER_MEDICAL_STATE.Progress;
        case "2": return ER_MEDICAL_STATE.Visited;
      }
      return ER_MEDICAL_STATE.None;
    }
    static ER_COLLABO_STATE ErCollaboState(string s)
    {
      switch (s)
      {
        case "0": return ER_COLLABO_STATE.None;
        case "1": return ER_COLLABO_STATE.Progress;
        case "2": return ER_COLLABO_STATE.Visited;
      }
      return ER_COLLABO_STATE.None;
    }
    static ER_HOSPITALIZED_STATE ErHospitalState(string s)
    {
      switch (s)
      {
        case "0": return ER_HOSPITALIZED_STATE.None;
        case "1": return ER_HOSPITALIZED_STATE.Hospitalization;
        case "2": return ER_HOSPITALIZED_STATE.Discharged;
      }
      return ER_HOSPITALIZED_STATE.None;
    }
    static ER_WARD_STATE ErWardState(string s)
    {
      switch (s)
      {
        case "0": return ER_WARD_STATE.None;
        case "1": return ER_WARD_STATE.Waiting;
        case "2": return ER_WARD_STATE.Assigned;
      }
      return ER_WARD_STATE.None;
    }

    static int ErCongestAreaCode(string code)
    {
      switch (code)
      {
        case "O001": return 1;
        case "O002": return 2;
        case "O003": return 3;
        case "O004": return 4;
        case "O046": return 46;
        case "O047": return 47;
        case "O048": return 48;
        case "O049": return 49;
        default: return 0;
      }
    }
    static string ErCongestAreaName(string code)
    {
      switch (code)
      {
        case "O001": return "성인 일반";
        case "O002": return "소아 일반";
        case "O003": return "음압 격리";// (안씀)
        case "O004": return "일반 격리"; // (안씀)
        case "O046": return "음압 격리"; // (사용)
        case "O047": return "일반 격리"; //(사용)
        case "O048": return "소아 음압 격리";
        case "O049": return "소아 일반 격리";
        default: return code;
      }
    }
    static string IcuGender(string s)
    {
      var arr = s.Split('/');  // "남/99"
      if (arr.Length == 2)
      {
        return arr[0];
      }
      return s;
    }
    static string OperationGender(string s)
    {
      // F/M
      if (s.ToLower() == "f") return "여";
      else if (s.ToLower() == "m") return "남";
      else return s;
    }
    static string IcuAge(string s)
    {
      var arr = s.Split('/');  // "남/99"
      if (arr.Length == 2)
      {
        int.TryParse(arr[1], out int val);
        return val.ToString();
      }
      return s;
    }
    static int ReserveTime(string s)
    {
      if (int.TryParse(s, out int val))
      {
        return val;
      }
      return 0;
    }
    static bool StringToBoolean(string source)
    {
      return source == "y" || source == "Y" | source == "1";
    }
    static bool IsMale(string source)
    {
      return source == "남" || source == "M";
    }
    static string PatientNo(string n)
    {
      int len = 4;
      if (n.Length > len)
      {
        n = n.Substring(0, len);
      }
      return n;
    }
    static string WardTypeName(string code)
    {
      switch (code)
      {
        case "A": return "일반병동";
        case "B": return "중환자실";
        case "C": return "NICU";
        case "D": return "분만병동";
        case "E": return "응급실";
        default: return "";
      }
    }
    static string[] _endoState = new string [] { "", "준비중", "수술중", "회복중", "퇴실" };
    static string EndoState(string s)
    {
      int.TryParse(s, out int v);
      if (v >= _endoState.Length) v = 0;
      return _endoState[v];
    }
    //(01 : 준비중 02 : 수술중 : 03 : 회복중, 4: 종료)
    static string[] _operationState = new string [] { "", "준비", "수술", "회복", "종료" };
    static string OperationStateName(string s)
    {
      int.TryParse(s, out int v);
      if (v >= _operationState.Length) v = 0;
      return _operationState[v];
    }
    static string[] _operationLocation = new string [] { "", "준비실", "수술실", "회복실" };
    static string OperationLocation(string s)
    {
      return "수술실";
    }
    static PATIENT_CALL_TYPE PatientCallType(string source)
    {
      int.TryParse(source, out int v);
      switch (v)
      {
        case 1: return PATIENT_CALL_TYPE.Patient;
        case 2: return PATIENT_CALL_TYPE.Guard;
        case 0:
        default:
          return PATIENT_CALL_TYPE.NoCall;
      }
    }
    static List<string> IcuInfections(string delimeter, string source)
    {
      return source.Split(',').ToList();
    }
    static DayOfWeek ToDayOfWeek(int i)
    {
      switch (i)
      {
        case 0: return DayOfWeek.Sunday;
        case 1: return DayOfWeek.Monday;
        case 2: return DayOfWeek.Tuesday;
        case 3: return DayOfWeek.Wednesday;
        case 4: return DayOfWeek.Thursday;
        case 5: return DayOfWeek.Friday;
        case 6: return DayOfWeek.Saturday;
        default:
          throw new ArgumentOutOfRangeException(nameof(i), "Invalid day of week value");
      }
    }
  }
}