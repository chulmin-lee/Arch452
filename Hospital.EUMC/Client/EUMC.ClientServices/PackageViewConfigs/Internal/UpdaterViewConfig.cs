using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class UpdaterViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; }
    public UpdaterViewConfig(string version) : base(PACKAGE.UPDATER)
    {
      this.WindowType = WindowScreenType.Custom;
      this.TitlebarStyle = TitlebarStyle.None;
      this.BottomStyle = BottomStyle.None;

      this.Config = new ContentConfig
      {
        Version = version,
        ItemRows = 5, // 기본 줄수
        ContentWidth = 800,
        ContentHeight = 600,
      };
    }

    public class ContentConfig
    {
      public string Version { get; set; } = string.Empty;
      public int ItemRows { get; set; } // 컨텐츠 줄수
      public int ContentWidth { get; set; } = 640;
      public int ContentHeight { get; set; } = 480;
    }
  }
}