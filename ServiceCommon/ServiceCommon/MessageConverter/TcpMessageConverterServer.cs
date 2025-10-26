using Common;
using System;

namespace ServiceCommon
{
  public class TcpMessageConverterServer : TcpMessageConverterBase
  {
    protected override void Initialize()
    {
      foreach (SERVICE_ID id in Enum.GetValues(typeof(SERVICE_ID)))
      {
        if (id == SERVICE_ID.NONE) continue;

        var type = get_type(id);
        if (type != null)
        {
          _message_types.Add((int)id, type);
        }
      }

      Type get_type(SERVICE_ID id)
      {
        #region types
        switch (id)
        {
          //===============================
          // 응급실
          //===============================
          case SERVICE_ID.ER_PATIENT: return typeof(ER_PATIENT_REQ);
          case SERVICE_ID.ER_CONGESTION: return typeof(ER_CONGESTION_REQ);
          case SERVICE_ID.ER_AREA_CONGEST: return typeof(ER_AREA_CONGEST_REQ);
          case SERVICE_ID.ER_ISOLATION: return typeof(ER_ISOLATION_REQ);
          case SERVICE_ID.ER_STATISTICS: return typeof(ER_STATISTICS_REQ);
          //===============================
          // IPD
          //===============================
          case SERVICE_ID.ICU: return typeof(ICU_REQ);
          case SERVICE_ID.DELIVERY_ROOM: return typeof(DELIVERY_ROOM_REQ);
          case SERVICE_ID.OPERATION: return typeof(OPERATION_REQ);
          case SERVICE_ID.WARD_ROOMS: return typeof(WARD_ROOM_REQ);
          //===============================
          // OPD
          //===============================
          case SERVICE_ID.OFFICE_ROOM: return typeof(OFFICE_REQ);
          case SERVICE_ID.EXAM_ROOM: return typeof(EXAM_REQ);
          case SERVICE_ID.ENDO: return typeof(ENDO_REQ);
          case SERVICE_ID.INSPECTION: return typeof(INSPECTION_REQ);
          //===============================
          // 기타
          //===============================
          case SERVICE_ID.DR_SCH: return typeof(DR_SCH_REQ);
          case SERVICE_ID.DRUG: return typeof(DRUG_REQ);
          case SERVICE_ID.CAFETERIA: return typeof(CAFETERIA_REQ);
          case SERVICE_ID.DR_PHOTO: return typeof(DR_PHOTO_REQ);
          case SERVICE_ID.DR_PHOTO_NOTI: return null;
          //===============================
          // 장례식장
          //===============================
          case SERVICE_ID.FUNERAL_ROOM: return typeof(FUNERAL_REQ);
          //===============================
          // ticket
          //===============================
          case SERVICE_ID.TICKET_GEN: return typeof(TICKET_GEN_REQ);
          case SERVICE_ID.TICKET_GEN_PT: return typeof(TICKET_GEN_PT_REQ);
          case SERVICE_ID.TICKET_CALL: return typeof(TICKET_CALL_REQ);
          case SERVICE_ID.TICKET_RECALL: return typeof(TICKET_RECALL_REQ);
          case SERVICE_ID.TICKET_DIV_INFO: return typeof(TICKET_DIV_INFO_REQ);

          case SERVICE_ID.CLIENT_REGISTER: return typeof(CLIENT_REGISTER_REQ);
          case SERVICE_ID.CLIENT_STATUS: return typeof(CLIENT_STATUS_REQ);
          // server -> client
          case SERVICE_ID.SVR_COMMAND:
          case SERVICE_ID.SVR_STATUS:
          case SERVICE_ID.SVR_CALL_OPERATION:
          case SERVICE_ID.SVR_BELL_CALL:
          case SERVICE_ID.SVR_CALL_ANNOUNCE:
          case SERVICE_ID.SVR_CALL_PATIENT:
          // client only
          case SERVICE_ID.USER_MESSAGE:
            return null;
        }
        #endregion types
        LOG.e($"TcpMessageServer: {id} not supported");
        return null;
      }
    }
  }
}