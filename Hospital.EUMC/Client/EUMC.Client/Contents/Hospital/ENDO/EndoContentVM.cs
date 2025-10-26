using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class EndoContentVM : ContentViewModelBase
  {
    public EndoInformation ITEM { get; set; }
    public EndoContentVM(EndoViewConfig config) : base(config)
    {
      this.ITEM = new EndoInformation(config);
    }
    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;
      switch (m.ServiceId)
      {
        case SERVICE_ID.ENDO:
          {
            return this.ITEM.Update(m.CastTo<ENDO_RESP>().Patients);
          }
          //case MESSAGE_ID.SVR_CALL_PATIENT:
          //  {
          //    return this.ITEM.CallPatient(m.CastTo<CALL_PATIENT_RESP>().Response);
          //  }
      }
      return false;
    }
    protected override void ContentClose()
    {
    }
  }
}