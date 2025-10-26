using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Linq;

namespace EUMC.ClientServices
{
  public class ExamSingleViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public OpdRoomConfig Room { get; set; }
    public ExamSingleViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.MiddleExamRoom;
      this.BottomStyle = BottomStyle.Logo;
      this.IsWideContent = false;

      this.Config = new ContentConfig
      {
        ItemRows = s.DelayPerson > 0 ? s.DelayPerson : 5,
      };

      var rooms = s.Medical?.DeptRooms ?? throw new ServiceException("opdroom");
      var room = rooms.First().Rooms.First();

      this.Room = new OpdRoomConfig
      {
        DeptCode = room.DeptCode,
        DeptName = room.DeptName,
        RoomCode = room.RoomCode,
        RoomName = room.RoomName,
        DurationTime = room.DurationTime,
        Title = room.RoomTitle
      };
    }

    public class ContentConfig
    {
      public int ItemRows { get; set; }  // content 줄수
    }
  }
}