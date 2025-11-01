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
          case SERVICE_ID.ER_CONGESTION: return typeof(ER_CONGESTION_RESP);
          case SERVICE_ID.ER_CPR: return typeof(ER_CPR_RESP);
          //===============================
          // IPD
          //===============================
          case SERVICE_ID.ICU: return typeof(ICU_RESP);
          case SERVICE_ID.OPERATION: return typeof(OPERATION_RESP);
          //===============================
          // OPD
          //===============================
          case SERVICE_ID.OFFICE_PT: return typeof(OFFICE_RESP);
          case SERVICE_ID.EXAM_PT: return typeof(EXAM_RESP);
          case SERVICE_ID.ANG: return typeof(ANG_RESP);
          case SERVICE_ID.ENDO: return typeof(ENDO_RESP);
          //===============================
          // 기타
          //===============================
          case SERVICE_ID.DR_SCH: return typeof(DR_SCH_RESP);
          case SERVICE_ID.DRUG: return typeof(DRUG_RESP);
          //===============================
          // 장례식장
          //===============================

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
          //case SERVICE_ID.SVR_CALL_PATIENT: return typeof(CALL_PATIENT_NOTI);
          //case SERVICE_ID.SVR_CALL_ANNOUNCE: return typeof(CALL_ANNOUNCE_NOTI);
          //case SERVICE_ID.SVR_CALL_OPERATION: return typeof(CALL_OPERATION_NOTI);
          //case SERVICE_ID.SVR_BELL_CALL: return typeof(CALLL_BELL_NOTI);
          //case SERVICE_ID.USER_MESSAGE: return null;
        }
        #endregion types

        LOG.w($"TcpMessageClient: {id} not supported");
        return null;
      }
    }
  }
}