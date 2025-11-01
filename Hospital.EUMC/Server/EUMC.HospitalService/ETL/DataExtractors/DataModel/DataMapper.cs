using AutoMapper;
using Common;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EUMC.HospitalService
{
  internal static partial class DataMapper
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
      // Base
      //===================================================
      // dept master
      cfg.CreateMap<DEPT_MASTER_DTO, DEPT_MASTER_POCO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.DEPT_CD))
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.DEPT_NM))
            ;

      //===================================================
      // Emergency
      //===================================================
      #region Emergency


      // 응급실
      cfg.CreateMap<ER_PATIENT_DTO, ER_PATIENT_INFO>()
            .ForMember(dest => dest.AreaCode, opt => opt.MapFrom(src => src.ER_AREA_CD))
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.ER_AREA_NM))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PT_NO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PT_NM))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.AGE))
            .ForMember(dest => dest.DoctorState, opt => opt.MapFrom(src => src.DOC_STATE))
            .ForMember(dest => dest.BloodState, opt => opt.MapFrom(src => src.BLO_STATE))
            .ForMember(dest => dest.ConState, opt => opt.MapFrom(src => src.CON_STATE))
            .ForMember(dest => dest.RadioState, opt => opt.MapFrom(src => src.RAD_STATE))
            .ForMember(dest => dest.InOutState, opt => opt.MapFrom(src => src.INOUT_STATE))
            ;

        // 응급실 혼잡도
      cfg.CreateMap<ER_CONGESTION_DTO, ER_CONGESTION_INFO>()
            .ForMember(dest => dest.IsChild, opt => opt.MapFrom(src => ErIsChild(src.구역)))
            .ForMember(dest => dest.CongestPercent, opt => opt.MapFrom(src => src.CALC_VALUE))
            .ForMember(dest => dest.PatientWaitTime, opt => opt.MapFrom(src => src.ER_SATY_TM))
            .ForMember(dest => dest.TreatmentWaitTime, opt => opt.MapFrom(src => src.ER_MED_TM))
            .ForMember(dest => dest.CTWaitTime, opt => opt.MapFrom(src => src.CT_EXAM_TM))
            .ForMember(dest => dest.XrayWaitTime, opt => opt.MapFrom(src => src.XRAY_EXAM_TM))
            ;

      // 응급실 구역별 혼잡도
      cfg.CreateMap<ER_CT_DTO, ER_CT_INFO>()
            .ForMember(dest => dest.IsExam, opt => opt.MapFrom(src => ErIsChild(src.CODE)))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PT_NO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PT_NM))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.NUM))
            .ForMember(dest => dest.WaitNo, opt => opt.MapFrom(src => src.SEQ))
            ;
      #endregion
      //===================================================
      // IPD
      //===================================================
      #region IPD
      // 수술실
      cfg.CreateMap<OPERATION_DTO, OPERATION_INFO>()
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PT_NO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PT_NM))
            .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => OperationState(src.StateCode)))
            ;

      // 중환자실
      cfg.CreateMap<ICU_DTO, ICU_INFO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.DEPT_CD))
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.DEPT_NM))
            .ForMember(dest => dest.IcuCode, opt => opt.MapFrom(src => src.ICU_CD))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PT_NO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PT_NM))
            .ForMember(dest => dest.BedNo, opt => opt.MapFrom(src => src.BED_NO))
            ;
      #endregion

      //===================================================
      // OPD: office
      //===================================================
      #region OPD - Office
      // room & doctor
      cfg.CreateMap<OFFICE_ROOM_DTO, OFFICE_ROOM_POCO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.DEPT_CD))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.RM_NO))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RM_NM))
            .ForMember(dest => dest.DoctorNo, opt => opt.MapFrom(src => src.DR_NO))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.DR_NM))
            .ForMember(dest => dest.DelayReason, opt => opt.MapFrom(src => src.DELEY_TXT))
            .ForMember(dest => dest.DelayTime, opt => opt.MapFrom(src => src.DELEY_TM));
      cfg.CreateMap<OFFICE_ROOM_POCO, ROOM_INFO>();
      cfg.CreateMap<OFFICE_ROOM_POCO, DOCTOR_INFO>();
      // 환자
      cfg.CreateMap<OFFICE_PT_DTO, OFFICE_PT_POCO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.DEPT_CD))
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.DEPT_NM))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.RM_NO))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PT_NO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PT_NM))
            .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.RANK))
            .ForMember(dest => dest.PhoneNo, opt => opt.MapFrom(src => src.PHONE_NO))
            .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => OfficeFloor(src.DEPT_FLR_TP_CD)))
            .ForMember(dest => dest.PactId, opt => opt.MapFrom(src => src.PACT_ID))
            ;
      cfg.CreateMap<OFFICE_PT_POCO, PATIENT_INFO>()
            .ForMember(dest => dest.WaitNo, opt => opt.MapFrom(src => src.Rank))
            ;

      cfg.CreateMap<DR_PHOTO_DTO, DR_PHOTO_POCO>()
            .ForMember(dest => dest.DoctorNo, opt => opt.MapFrom(src => src.STF_NO))  // 임의로 추가했음. 조회할때 추가할것
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => Path.GetFileName(src.IMAGE_PATH)));
      #endregion
      //===================================================
      // OPD: Exam common
      //===================================================
      #region OPD Exam Common
      cfg.CreateMap<EXAM_DEPT_DTO, EXAM_DEPT_POCO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.DEPT_CD))
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.LWR_CTG_CNTE))
            ;
      cfg.CreateMap<EXAM_ROOM_DTO, EXAM_ROOM_POCO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.DEPT_CD))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.EXAM_RM_CD))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.EXAM_RM_NM))
            .ForMember(dest => dest.DelayReason, opt => opt.MapFrom(src => src.DELAY_TEXT))
            .ForMember(dest => dest.DelayTime, opt => opt.MapFrom(src => src.DELAY_TM));
      cfg.CreateMap<EXAM_ROOM_POCO, ROOM_INFO>();

      cfg.CreateMap<EXAM_STAFF_DTO, EXAM_STAFF_POCO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.DEPT_CD))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.EXAMRMCD))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.EXAMRMNM))
            .ForMember(dest => dest.DoctorNo, opt => opt.MapFrom(src => src.STFNO))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.STFNM))
            //.ForMember(dest => dest.DelayReason, opt => opt.MapFrom(src => src.DELEYTXT))
            //.ForMember(dest => dest.DelayTime, opt => opt.MapFrom(src => src.DELEYTM));
            ;
      //cfg.CreateMap<EXAM_STAFF_POCO, DOCTOR_INFO>();

      cfg.CreateMap<EXAM_PT_DTO, EXAM_PT_POCO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.DEPTCD))
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.DEPTNM))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.EXAMRMCD))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.EXAMRMNM))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PTNO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PTNM ))
            .ForMember(dest => dest.WaitNo, opt => opt.MapFrom(src => src.ELPDSEQ ));
      cfg.CreateMap<EXAM_PT_POCO, PATIENT_INFO>();

      #endregion

      //===================================================
      // OPD: Exam special
      //===================================================
      #region OPD Exam Special

      // 조영술 (an
      cfg.CreateMap<ANG_PT_DTO, ANG_PT_INFO>()
            .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.DEPT_CD))
            .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.DEPT_NM))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PT_NO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PT_NM))
            .ForMember(dest => dest.OperationName, opt => opt.MapFrom(src => src.OP_NM))
            .ForMember(dest => dest.OperationRoom, opt => opt.MapFrom(src => src.OP_RM))
            .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => AngState(src.STATE_CD)))
            ;
      // 초음파
      cfg.CreateMap<ENDO_PT_DTO, ENDO_PT_INFO>()
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PT_NO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PT_NM))
            .ForMember(dest => dest.Gubn, opt => opt.MapFrom(src => src.GUBN))
            ;
      // 방사선종양학과 일반촬영실
      cfg.CreateMap<RAD_PT_DTO, RAD_PT_INFO>()
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.RAD_RM_NO))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RAD_RM_NM))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PT_NO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PT_NM))
            .ForMember(dest => dest.WaitNo, opt => opt.MapFrom(src => src.SEQ))
            ;
      // 방사선종양학과 검사실 환자
      cfg.CreateMap<RAD_TR_PT_DTO, RAD_TR_PT_INFO>()
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PT_NO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PT_NM))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.TYRM_CD))  // TR001, TR002
            ;

      #endregion


      //===================================================
      // ETC
      //===================================================
      #region ETC
      // 약제과
      cfg.CreateMap<DRUG_DTO, DRUG_INFO>()
            .ForMember(dest => dest.DrugNo, opt => opt.MapFrom(src => src.RAG_NO))
            .ForMember(dest => dest.PatientNo, opt => opt.MapFrom(src => src.PT_NO))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PT_NM))
            ;

           // 진료 스케쥴
      //cfg.CreateMap<DR_SCH_DTO, DR_SCH_POCO>()
      //      .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.orddeptcdnm))
      //      .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(src => src.orddeptcd))
      //      .ForMember(dest => dest.DoctorNo, opt => opt.MapFrom(src => src.orddrid))
      //      .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.orddridnm))
      //      .ForMember(dest => dest.SpecialPart, opt => opt.MapFrom(src => src.specialdept))
      //      .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src =>  ToDayOfWeek(src.week)))
      //      .ForMember(dest => dest.AM, opt => opt.MapFrom(src => src.amordyn))
      //      .ForMember(dest => dest.PM, opt => opt.MapFrom(src => src.pmordyn));



      #endregion


      //===================================================
      // POCO to Message Data
      //===================================================
      // office
      //cfg.CreateMap<OFFICE_PT_POCO, ROOM_INFO>();
      //cfg.CreateMap<OFFICE_PT_POCO, DOCTOR_INFO>()
      //   .ForMember(dest => dest.SpecialPart, opt => opt.MapFrom(src => CommaStringToNewLineString(src.DoctorPart)))
      //   ;
      //cfg.CreateMap<OFFICE_PT_POCO, PATIENT_INFO>();
      //// exam
      //cfg.CreateMap<EXAM_POCO, ROOM_INFO>();
      //cfg.CreateMap<EXAM_POCO, PATIENT_INFO>();


    });
      return config.CreateMapper();
    }


  }
}