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
          case SERVICE_ID.ER_CPR: return typeof(ER_CPR_REQ);
          //===============================
          // IPD
          //===============================
          case SERVICE_ID.ICU: return typeof(ICU_REQ);
          case SERVICE_ID.OPERATION: return typeof(OPERATION_REQ);
          //===============================
          // OPD
          //===============================
          case SERVICE_ID.OFFICE_PT: return typeof(OFFICE_REQ);
          case SERVICE_ID.EXAM_PT: return typeof(EXAM_REQ);
          case SERVICE_ID.ANG: return typeof(ANG_REQ);
          case SERVICE_ID.ENDO: return typeof(ENDO_REQ);
          //===============================
          // 기타
          //===============================
          case SERVICE_ID.DR_SCH: return typeof(DR_SCH_REQ);
          case SERVICE_ID.DRUG: return typeof(DRUG_REQ);
          //===============================
          // 장례식장
          //===============================
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
        }
        #endregion types
        LOG.w($"TcpMessageServer: {id} not supported");
        return null;
      }
    }
  }
}