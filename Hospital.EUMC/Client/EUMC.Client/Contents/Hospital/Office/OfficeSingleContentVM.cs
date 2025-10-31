using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class OfficeSingleContentVM : ContentViewModelBase
  {
    public OfficeSingleInformation ITEM { get; set; }
    public OfficeSingleContentVM(OfficeSingleViewConfig config) : base(config)
    {
      this.ITEM = new OfficeSingleInformation(config);
    }

    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;

      LOG.dc($"{m.ServiceId}");
      switch (m.ServiceId)
      {
        case SERVICE_ID.OFFICE_PT:
          return this.ITEM.Update(m.CastTo<OFFICE_RESP>());
        case SERVICE_ID.DR_PHOTO:
          return this.ITEM.UpdatePhoto(m.CastTo<DR_PHOTO_RESP>());
        case SERVICE_ID.DR_PHOTO_NOTI:
          return this.ITEM.PhotoNotify(m.CastTo<DR_PHOTO_UPDATED>());
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