using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.ClientServices
{
  public class OpdSingleViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; }
    public PackageRoomConfig Room { get; set; } = new PackageRoomConfig();
    public List<string> WaitMesages { get; set; } = new List<string>();

    public OpdSingleViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.MiddleNormal;
      this.BottomStyle = BottomStyle.MiddleNotice;
      this.IsWideContent = false;

      this.Config = new ContentConfig()
      {
        ShowDelayTime = s.ShowDelayTime,
        ItemRows = s.PatientPerRoom > 0 ? s.PatientPerRoom : 4
      };
      this.Room = p.RoomConfigs.FirstOrDefault() ?? throw new ServiceException("opdroom");
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
      this.WaitMesages = this.IsSeoul ? new List<string> { "들어오실 분", "다음 순서 입니다", "잠시만 기다려 주십시요" }
                                      : new List<string> { "들어오실 분", "다음 순서", };
    }
  }
}