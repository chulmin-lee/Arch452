using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class ErrorContentVM : ContentViewModelBase
  {
    public PACKAGE_ERROR Error { get; set; }
    public string ErrorMessage { get; set; }
    public ErrorContentVM(ErrorViewConfig config) : base(config)
    {
      this.Error = config.Error;
      this.ErrorMessage = config.ErrorMessage;
    }
    protected override void ContentClose()
    {
    }
  }
}