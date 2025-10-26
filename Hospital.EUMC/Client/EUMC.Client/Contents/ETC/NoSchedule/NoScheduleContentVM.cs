using EUMC.ClientServices;

namespace EUMC.Client
{
  internal class NoScheduleContentVM : ContentViewModelBase
  {
    public NoScheduleContentVM(NoScheduleViewConfig config) : base(config)
    {
    }

    protected override void ContentClose()
    {
    }
  }
}