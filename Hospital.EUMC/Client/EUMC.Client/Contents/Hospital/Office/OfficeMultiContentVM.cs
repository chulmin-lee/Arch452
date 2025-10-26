using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class OfficeMultiContentVM : ContentViewModelBase
  {
    public OfficeMultiInformation ITEM { get; set; }
    public OfficeMultiContentVM(OfficeMultiViewConfig config) : base(config)
    {
      this.ITEM = new OfficeMultiInformation(config);
    }

    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;

      LOG.dc($"{m.ServiceId}");
      switch (m.ServiceId)
      {
        case SERVICE_ID.OFFICE_ROOM:
          return this.ITEM.Update(m.CastTo<OFFICE_RESP>());
        case SERVICE_ID.SVR_CALL_PATIENT:
          {
            return this.ITEM.CallPatient(m.CastTo<CALL_PATIENT_NOTI>());
          }
        case SERVICE_ID.SVR_CALL_ANNOUNCE:
          {
            return this.ITEM.CallAnnounce(m.CastTo<CALL_ANNOUNCE_NOTI>());
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