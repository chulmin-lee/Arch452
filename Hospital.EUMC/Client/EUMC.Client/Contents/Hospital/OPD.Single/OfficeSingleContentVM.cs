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
      }
      return false;
    }
  }
}