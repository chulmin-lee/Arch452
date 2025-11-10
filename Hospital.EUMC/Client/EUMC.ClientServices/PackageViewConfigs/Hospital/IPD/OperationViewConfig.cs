using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class OperationViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public OperationViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.LargeNotice;
      this.ContentTitle = "수술 현황";

      this.Config = new ContentConfig()
      {
        ItemRows = 6,
        UseRotation = true,
      };
    }
    public class ContentConfig
    {
      public int ItemRows { get; set; }
      public bool UseRotation { get; set; } = true;
    }
  }
}