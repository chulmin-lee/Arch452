using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class DrSchViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public DrSchViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.LargeNotice;
      this.ContentTitle = "외래 진료 일정 안내";
    }
    public class ContentConfig
    {
    }
  }
}