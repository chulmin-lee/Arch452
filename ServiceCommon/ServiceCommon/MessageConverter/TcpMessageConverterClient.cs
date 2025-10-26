using Common;
using System;

namespace ServiceCommon
{
  public class TcpMessageConverterClient : TcpMessageConverterBase
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
          case SERVICE_ID.ER_PATIENT: return typeof(ER_PATIENT_RESP);
          case SERVICE_ID.ER_AREA_CONGEST: return typeof(ER_AREA_CONGEST_RESP);
          case SERVICE_ID.ER_CONGESTION: return typeof(ER_CONGESTION_RESP);
          case SERVICE_ID.ER_ISOLATION: return typeof(ER_ISOLATION_RESP);
          case SERVICE_ID.ER_STATISTICS: return typeof(ER_STATISTICS_RESP);
          //===============================
          // IPD
          //===============================
          case SERVICE_ID.DELIVERY_ROOM: return typeof(DELIVERY_ROOM_RESP);
          case SERVICE_ID.ICU: return typeof(ICU_RESP);
          case SERVICE_ID.OPERATION: return typeof(OPERATION_RESP);
          case SERVICE_ID.WARD_ROOMS: return typeof(WARD_ROOM_RESP);
          //===============================
          // OPD
          //===============================
          case SERVICE_ID.OFFICE_ROOM: return typeof(OFFICE_RESP);
          case SERVICE_ID.EXAM_ROOM: return typeof(EXAM_RESP);
          case SERVICE_ID.INSPECTION: return typeof(INSPECTION_RESP);
          case SERVICE_ID.ENDO: return typeof(ENDO_RESP);
          //===============================
          // 기타
          //===============================
          case SERVICE_ID.DR_SCH: return typeof(DR_SCH_RESP);
          case SERVICE_ID.DRUG: return typeof(DRUG_RESP);
          case SERVICE_ID.CAFETERIA: return typeof(CAFETERIA_RESP);
          case SERVICE_ID.DR_PHOTO: return typeof(DR_PHOTO_RESP);
          case SERVICE_ID.DR_PHOTO_NOTI: return typeof(DR_PHOTO_UPDATED);
          //===============================
          // 장례식장
          //===============================
          case SERVICE_ID.FUNERAL_ROOM: return typeof(FUNERAL_RESP);

          //===============================
          // ticket
          //===============================
          case SERVICE_ID.TICKET_GEN: return typeof(TICKET_GEN_RESP);
          case SERVICE_ID.TICKET_GEN_PT: return typeof(TICKET_GEN_PT_RESP);
          case SERVICE_ID.TICKET_CALL: return typeof(TICKET_CALL_RESP);
          case SERVICE_ID.TICKET_RECALL: return null;
          case SERVICE_ID.TICKET_DIV_INFO: return typeof(TICKET_DIV_INFO_RESP);

          case SERVICE_ID.CLIENT_REGISTER: return typeof(CLIENT_REGISTER_RESP);
          case SERVICE_ID.SVR_COMMAND: return typeof(SERVER_CMD_NOTI);
          case SERVICE_ID.SVR_STATUS: return typeof(SERVER_STATUS_NOTI);
          case SERVICE_ID.CLIENT_STATUS: return typeof(CLIENT_STATUS_RESP);
          case SERVICE_ID.SVR_CALL_PATIENT: return typeof(CALL_PATIENT_NOTI);
          case SERVICE_ID.SVR_CALL_ANNOUNCE: return typeof(CALL_ANNOUNCE_NOTI);
          case SERVICE_ID.SVR_CALL_OPERATION: return typeof(CALL_OPERATION_NOTI);
          case SERVICE_ID.SVR_BELL_CALL: return typeof(CALLL_BELL_NOTI);
          case SERVICE_ID.USER_MESSAGE: return null;
        }
        #endregion types

        LOG.e($"TcpMessageClient: {id} not supported");
        return null;
      }
    }
  }
}