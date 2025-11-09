using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.ClientServices
{
  public class OpdSingleViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; }
    public OpdRoomConfig Room { get; set; } = new OpdRoomConfig();
    public List<string> WaitMesages { get; set; } = new List<string>();

    public OpdSingleViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.MiddleNormal;
      this.BottomStyle = BottomStyle.MiddleNotice;
      this.IsWideContent = false;

      this.Config = new ContentConfig()
      {
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
    }

    public class ContentConfig
    {
      public int ItemRows { get; set; }  // content 줄수
      public bool ShowDelayTime { get; set; }
      public bool ShowDelayPopup { get; set; }
      public int DelayPopupInterval { get; set; } = 30 * 60; // 지연 알림 팝업 지속 시간(초)
    }
  }

  //==========================================================
  // Office
  //==========================================================
  public class OfficeSingleViewConfig : OpdSingleViewConfig
  {
    public OfficeSingleViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.Config.ItemRows = s.PatientPerRoom > 0 ? s.PatientPerRoom : 4;
      this.Config.ShowDelayPopup = true;
      this.Config.DelayPopupInterval = 30 * 60;

      this.WaitMesages = this.IsSeoul ? new List<string> { "들어오실 분", "다음 순서 입니다", "잠시만 기다려 주십시요" }
                                      : new List<string> { "들어오실 분", "다음 순서", "진료 대기" };
    }
  }
  //==========================================================
  // Exam
  //==========================================================
  public class ExamSingleViewConfig : OpdSingleViewConfig
  {
    public ExamSingleViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.Config.ItemRows = s.PatientPerRoom > 0 ? s.PatientPerRoom : 5;

      this.WaitMesages = this.IsSeoul ? new List<string> { "들어오실 분", "다음 순서 입니다", "잠시만 기다려 주십시요" }
                                      : new List<string> { "들어오실 분", "다음 순서", };
    }
  }
}