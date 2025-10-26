using EUMC.ClientServices;

namespace EUMC.Client
{
  internal class PromotionContentVM : ContentViewModelBase
  {
    public PromotionInformation ITEM { get; set; }
    public PromotionContentVM(PromotionViewConfig config) : base(config)
    {
      this.ITEM = new PromotionInformation(config);
    }
    protected override void ScreenOnOff(bool on)
    {
      this.ITEM.ScreenOnOff(on);
    }
    protected override void ContentClose()
    {
      this.ITEM.Close();
    }
  }
}