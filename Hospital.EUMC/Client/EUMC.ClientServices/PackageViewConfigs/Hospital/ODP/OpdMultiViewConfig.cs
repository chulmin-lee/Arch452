using Common;
using ServiceCommon;
using ServiceCommon.ClientServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.ClientServices
{
  public class OpdMultiViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public RoomPanelConfig PanelConfig { get; set; } = new RoomPanelConfig();
    public List<PackageRoomConfig> Rooms { get; set; } = new List<PackageRoomConfig>();

    public OpdMultiViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.LargeNotice;
      this.ContentTitle = "종합진료안내";

      this.Config = new ContentConfig
      {
        ShowDelayTime = s.ShowDelayTime,
        RoomPerPage = 4,
        ItemRows = s.PatientPerRoom > 0 ? s.PatientPerRoom : 4,
      };

      this.PanelConfig = new RoomPanelConfig
      {
        ItemRows = this.Config.ItemRows,
        RoomTitle = "진료실",
        DoctorTitle = "담당의사",
      };

      this.PanelConfig.WaitMesages = this.IsSeoul ? new List<string> { "들어오실 분", "다음 순서 입니다", "잠시만 기다려 주십시요" }
                                                  : new List<string> { "들어오실 분", "다음 순서", "진료 대기" };
      this.Rooms = p.RoomConfigs;
    }

    public class ContentConfig
    {
      public int RoomPerPage { get; set; }
      public int ItemRows { get; set; }
      public bool ShowDelayTime { get; set; }
    }
  }

  public class RoomPanelConfig
  {
    public bool IsOffice { get; set; }
    public int ItemRows { get; set; }
    public string RoomTitle { get; set; } = string.Empty;
    public string DoctorTitle { get; set; } = string.Empty;
    public List<string> WaitMesages { get; set; } = new List<string>();
  }

  //==========================================================
  // Office
  //==========================================================

  public class OfficeMultiViewConfig : OpdMultiViewConfig
  {
    public OfficeMultiViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.PanelConfig.IsOffice = true;
    }
  }
  //==========================================================
  // Office
  //==========================================================

  public class ExamMultiViewConfig : OpdMultiViewConfig
  {
    public ExamMultiViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.PanelConfig.IsOffice = false;
    }
  }
}