using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.ClientServices
{
  public class OfficeSingleViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public OpdRoomConfig Room { get; set; } = new OpdRoomConfig();
    public List<string> WaitMesages { get; set; } = new List<string>();
    public OfficeSingleViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.MiddleNormal;
      this.BottomStyle = BottomStyle.MiddleNotice;
      this.IsWideContent = false;

      this.Config = new ContentConfig()
      {
        ItemRows = s.DelayPerson > 0 ? s.DelayPerson : 4,
        UseRotation = false,
        ShowDelayPopup = true,
        ShowDelayTime = s.ShowDelayTime,
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
      public bool ShowDelayTime { get; set; }
      public bool UseRotation { get; set; } = false; // 페이지 혹은 Item 순환 여부
      public int ItemRows { get; set; }  // content 줄수
      public bool ShowDelayPopup { get; set; }
      public int DelayPopupDuration { get; set; } = 30 * 60; // 지연 알림 팝업 지속 시간(초)
    }
  }
}