using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{

  internal class ErPatientContentVM : ContentViewModelBase
  {
    public ErPatientInformation ITEM { get; set; }

    public ErPatientContentVM(ErPatientViewConfig config) : base(config)
    {
      this.ITEM = new ErPatientInformation(config);
    }
    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;

      LOG.ic($"{m.ServiceId}");
      switch (m.ServiceId)
      {
        case SERVICE_ID.ER_PATIENT:
          {
            return this.ITEM.Update(m.CastTo<ER_PATIENT_RESP>());
          }
        case SERVICE_ID.ER_CONGESTION:
          {
            return this.ITEM.UpdateCongestion(m.CastTo<ER_CONGESTION_RESP>());
          }
        case SERVICE_ID.ER_STATISTICS:
          {
            return this.ITEM.UpdateStatistics(m.CastTo<ER_STATISTICS_RESP>());
          }
        case SERVICE_ID.SVR_CALL_PATIENT:
          {
            return this.ITEM.CallPatient(m.CastTo<CALL_PATIENT_NOTI>());
          }
      }
      return false;
    }
    protected override void ContentClose()
    {
      this.ITEM.Close();
    }
  }
}