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
        case SERVICE_ID.ER_CONGESTION: return new ER_CONGESTION_Service(owner);
        case SERVICE_ID.ER_CPR: return new ER_CPR_Service(owner);
        #endregion Emergency

        #region IPD
        case SERVICE_ID.ICU: return new ICU_Service(owner);
        case SERVICE_ID.OPERATION: return new OPERATION_Service(owner);
        #endregion IPD

        #region OPD
        case SERVICE_ID.OFFICE_PT: return new OFFICE_PT_Service(owner, o.OFFICE_PT);
        case SERVICE_ID.EXAM_PT: return new EXAM_PT_Service(owner);
        case SERVICE_ID.ANG: return new ANG_Service(owner);
        case SERVICE_ID.ENDO: return new ENDO_Service(owner);
        #endregion OPD

        #region ETC
        case SERVICE_ID.DRUG: return new DRUG_Service(owner);
        //case SERVICE_ID.DR_SCH: return new DR_SCH_Service(owner);
        #endregion ETC
        default:
          {
            throw new ServiceException($"{id} not supported");
          }
      }
    }
  }
}