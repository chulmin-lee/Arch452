using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class OperationContentVM : ContentViewModelBase
  {
    public OperationInformation ITEM { get; set; }
    public OperationContentVM(OperationViewConfig config) : base(config)
    {
      this.ITEM = new OperationInformation(config);
    }
    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;

      LOG.dc($"{m.ServiceId}");

      switch (m.ServiceId)
      {
        case SERVICE_ID.OPERATION:
          {
            return this.ITEM.Update(m.CastTo<OPERATION_RESP>());
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