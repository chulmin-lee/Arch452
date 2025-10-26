using Common;
using ServiceCommon;
using ServiceCommon.ServerServices;

namespace EUMC.HospitalService
{
  internal static class MessageTransformerFactory
  {
    public static IMessageTransformer<DATA_ID> Create(IHospitalMemberOwner owner, SERVICE_ID id, ServiceConfigurations o)
    {
      switch (id)
      {
        #region Emergency

        case SERVICE_ID.ER_PATIENT: return new ER_PATIENT_Service(owner);
        case SERVICE_ID.ER_AREA_CONGEST: return new ER_AREA_CONGEST_Service(owner);
        case SERVICE_ID.ER_CONGESTION: return new ER_CONGESTION_Service(owner);
        case SERVICE_ID.ER_ISOLATION: return new ER_ISOLATION_Service(owner);
        case SERVICE_ID.ER_STATISTICS: return new ER_STATISTICS_Service(owner);
        #endregion Emergency

        #region IPD
        case SERVICE_ID.ICU: return new ICU_Service(owner);
        case SERVICE_ID.OPERATION: return new OPERATION_Service(owner);
        case SERVICE_ID.DELIVERY_ROOM: return new DELIVERY_Service(owner);
        case SERVICE_ID.WARD_ROOMS: return new WARD_Service(owner);
        #endregion IPD

        #region OPD

        case SERVICE_ID.OFFICE_ROOM: return new OPD_OFFICE_Service(owner);
        case SERVICE_ID.EXAM_ROOM: return new OPD_EXAM_Service(owner);
        case SERVICE_ID.INSPECTION: return new INSPECTION_Service(owner);
        case SERVICE_ID.ENDO: return new ENDO_Service(owner);
        #endregion OPD

        #region ETC
        case SERVICE_ID.DRUG: return new DRUG_Service(owner);
        case SERVICE_ID.DR_SCH: return new DR_SCH_Service(owner);
        case SERVICE_ID.DR_PHOTO:
          {
            return new DR_PHOTO_Service(owner, o.DR_PHOTO);
          }
        #endregion ETC

        default:
          {
            throw new ServiceException($"{id} not supported");
          }
      }
    }
  }
}