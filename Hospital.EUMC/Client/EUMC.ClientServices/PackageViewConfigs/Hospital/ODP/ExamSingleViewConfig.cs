using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.ClientServices
{
  public class ExamSingleViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public OpdRoomConfig Room { get; set; }
    public List<string> WaitMesages { get; set; } = new List<string>();
    public ExamSingleViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.MiddleNormal;
      this.BottomStyle = BottomStyle.MiddleNotice;
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

      switch (s.HospitalCode)
      {
        case "01": this.WaitMesages = new List<string> { "들어오실 분", "다음 순서 입니다", "잠시만 기다려 주십시요" }; break;
        case "02": this.WaitMesages = new List<string> { "들어오실 분", "다음 순서", "진료 대기" }; break;
      }
    }

    public class ContentConfig
    {
      public int ItemRows { get; set; }  // content 줄수
    }
  }
}