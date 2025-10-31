using Common;
using ServiceCommon;
using ServiceCommon.HospitalService;
using ServiceCommon.ServerServices;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  public class HospitalMessageServiceHandler : MessageServiceHandlerBase
  {
    public HospitalMessageServiceHandler(IMessageGenerator impl) : base(impl)
    {
    }

    protected override IMessageLoader CreateMessageLoaderImpl(SERVICE_ID id)
    {
      switch (id)
      {
        //===================
        // Emergency
        //===================
        case SERVICE_ID.ER_PATIENT: return new ER_PATIENT_Loader();
        case SERVICE_ID.ER_CONGESTION: return new ER_CONGESTION_Loader();
        case SERVICE_ID.ER_STATISTICS: return new ER_STATISTICS_Loader();
        //===================
        // IPD
        //===================
        case SERVICE_ID.DELIVERY_ROOM: return new DELIVERY_Loader();
        case SERVICE_ID.ICU: return new ICU_Loader();
        case SERVICE_ID.OPERATION: return new OPERATION_Loader();
        case SERVICE_ID.WARD_ROOMS: return new WARD_Loader();
        //===================
        // OPD
        //===================
        case SERVICE_ID.OFFICE_PT: return new OPD_OFFICE_Loader();
        case SERVICE_ID.EXAM_PT: return new OPD_EXAM_Loader();
        case SERVICE_ID.ENDO: return new ENDO_Loader();
        //===================
        // ETC
        //===================
        case SERVICE_ID.DRUG: return new DRUG_Loader();
        case SERVICE_ID.DR_PHOTO: return new DR_PHOTO_Loader();
      }
      LOG.ec($"{id} not supported");
      return null;
    }

    protected override Dictionary<PACKAGE, List<SERVICE_ID>> InitalizePackageMapImpl()
    {
      var map = new Dictionary<PACKAGE, List<SERVICE_ID>>();
      //===================
      // Emergency
      //===================
      map.Add(PACKAGE.ER_PATIENT, new List<SERVICE_ID> { SERVICE_ID.ER_PATIENT, SERVICE_ID.ER_CONGESTION, SERVICE_ID.ER_CPR });
      //===================
      // IPD
      //===================
      map.Add(PACKAGE.ICU, new List<SERVICE_ID> { SERVICE_ID.ICU });
      map.Add(PACKAGE.OPERATION, new List<SERVICE_ID> { SERVICE_ID.OPERATION });
      //===================
      // OPD
      //===================
      map.Add(PACKAGE.OFFICE_SINGLE, new List<SERVICE_ID> { SERVICE_ID.OFFICE_PT });
      map.Add(PACKAGE.OFFICE_MULTI, new List<SERVICE_ID> { SERVICE_ID.OFFICE_PT });
      map.Add(PACKAGE.EXAM_SINGLE, new List<SERVICE_ID> { SERVICE_ID.EXAM_PT });
      map.Add(PACKAGE.EXAM_MULTI, new List<SERVICE_ID> { SERVICE_ID.EXAM_PT });
      map.Add(PACKAGE.ENDO, new List<SERVICE_ID> { SERVICE_ID.ENDO });
      //===================
      // ETC
      //===================
      map.Add(PACKAGE.DRUG, new List<SERVICE_ID> { SERVICE_ID.DRUG });
      return map;
    }
  }
}