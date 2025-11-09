using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class EndoViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public EndoViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.LargeNotice;
      this.ContentTitle = "내시경실";

      this.Config = new ContentConfig()
      {
        ItemRows = s.PatientPerRoom > 0 ? s.PatientPerRoom : 5,
      };
    }

    public class ContentConfig
    {
      public int ItemRows { get; set; }  // content 줄수
    }
  }
}