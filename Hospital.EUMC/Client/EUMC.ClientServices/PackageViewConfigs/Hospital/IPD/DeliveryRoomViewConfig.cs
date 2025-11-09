using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class DeliveryRoomViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public DeliveryRoomViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.LargeNotice;
      this.ContentTitle = "분만실";

      this.Config = new ContentConfig()
      {
        ItemRows = s.PatientPerRoom > 0 ? s.PatientPerRoom : 8,
      };
    }
    public class ContentConfig
    {
      public int ItemRows { get; set; }
    }
  }
}