using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class ErPatientViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public bool IsChild { get; set; }
    public ErPatientViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.None;
      this.ContentTitle = this.IsChild ? "소아 응급 센터" : "권역 응급의료 센터";

      this.Config = new ContentConfig()
      {
        ItemRows = s.PatientPerRoom > 0 ? s.PatientPerRoom : 10,
      };

      var emergency = this.PackageInfo.Emergency ?? throw new ServiceException("ememrgency");
      this.IsChild = emergency.IsChild;
    }
    public class ContentConfig
    {
      public int ItemRows { get; set; } // 10;
    }
  }
}