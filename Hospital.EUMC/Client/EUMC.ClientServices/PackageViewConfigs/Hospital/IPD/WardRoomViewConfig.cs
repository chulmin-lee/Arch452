using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class WardRoomViewConfig : PackageViewConfig
  {
    public int Floor { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string Key => $"{this.Floor}:{this.AreaCode}";

    public WardRoomViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.None;
      this.BottomStyle = BottomStyle.None;
      this.ContentTitle = "병실 현황 안내";

      var  o = this.PackageInfo.WardRoom ?? throw new ServiceException("wardroom");
      this.Floor = o.Floor;
      this.AreaCode = o.AreaCode;
    }
  }
}