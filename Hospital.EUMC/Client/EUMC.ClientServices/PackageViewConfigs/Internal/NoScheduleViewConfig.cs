using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class NoScheduleViewConfig : PackageViewConfig
  {
    public NoScheduleViewConfig() : base(PACKAGE.NO_SCHEDULE)
    {
      this.WindowType = WindowScreenType.Maximized;
      this.TitlebarStyle = TitlebarStyle.None;
      this.BottomStyle = BottomStyle.None;
      this.IsScreenOn = false;
      this.PackageInfo = new PackageInfo("no", PACKAGE.NO_SCHEDULE, false);
    }
  }
}