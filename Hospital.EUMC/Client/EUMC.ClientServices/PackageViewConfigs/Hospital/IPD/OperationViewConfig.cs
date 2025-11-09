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
      this.ContentTitle = "수술 진행 현황 안내";

      this.Config = new ContentConfig()
      {
        ItemRows = s.PatientPerRoom > 0 ? s.PatientPerRoom : 5,
      };
    }
    public class ContentConfig
    {
      public int ItemRows { get; set; }
    }
  }
}