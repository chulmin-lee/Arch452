using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class ErrorViewConfig : PackageViewConfig
  {
    public PACKAGE_ERROR Error { get; protected set; }
    public string ErrorMessage { get; protected set; } = string.Empty;

    public ErrorViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.WindowType = WindowScreenType.Maximized;
      this.TitlebarStyle = TitlebarStyle.Custom;
      this.BottomStyle = BottomStyle.None;

      var e = p.ErrorPackage;
      if (e != null)
      {
        this.Error = e.ErrorCode;
        this.ErrorMessage = e.ErrorMessage;
      }
    }

    public ErrorViewConfig(string packageName) : base(PACKAGE.ERROR_PACKAGE)
    {
      this.WindowType = WindowScreenType.Maximized;
      this.TitlebarStyle = TitlebarStyle.Custom;
      this.BottomStyle = BottomStyle.None;

      this.Error = PACKAGE_ERROR.UnknownPackage;
      this.ErrorMessage = $"{packageName} not supported";
      this.PackageInfo = new PackageInfo(packageName, this.Error, this.ErrorMessage);
    }
  }
}