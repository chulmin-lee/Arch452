using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class DrugViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public DrugViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.LargeNotice;
      this.ContentTitle = "약제과";

      this.Config = new ContentConfig()
      {
        ContentPerPage = 1,
        ItemRows = 6,
        ItemColumns = 6
      };
    }

    public class ContentConfig
    {
      public int ContentPerPage { get; set; }
      public int ItemRows { get; set; }
      public int ItemColumns { get; set; }
    }
  }
}