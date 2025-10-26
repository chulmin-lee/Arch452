using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Linq;

namespace EUMC.ClientServices
{
  public class OfficeSingleViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public OpdRoomConfig Room { get; set; } = new OpdRoomConfig();

    public OfficeSingleViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.None;
      this.BottomStyle = BottomStyle.Logo;
      this.IsWideContent = false;

      this.Config = new ContentConfig()
      {
        ItemRows = s.DelayPerson > 0 ? s.DelayPerson : 4,
        UseRotation = false,
        UseInRoomPoupup = true
      };

      var opd = this.PackageInfo.OpdRoom ?? throw new ServiceException("opdroom");
      var dept = opd.DeptRooms.First();
      this.Room = new OpdRoomConfig
      {
        DeptCode = dept.DeptCode,
        RoomCode = dept.RoomCode,
      };
    }

    public class ContentConfig
    {
      public bool UseRotation { get; set; } = false; // 페이지 혹은 Item 순환 여부
      public int ItemRows { get; set; }  // content 줄수
      public bool UseInRoomPoupup { get; set; }
    }
  }
}