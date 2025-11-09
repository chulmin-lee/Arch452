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
    public List<OpdRoomConfig> ExamRooms { get; private set; } = new List<OpdRoomConfig>();
    public List<OpdRoomConfig> OfficeRooms { get; private set; } = new List<OpdRoomConfig>();

    public OpdMultiViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.LargeNotice;

      this.Config = new ContentConfig
      {
        RoomPerPage = 4,
        ItemRows = s.PatientPerRoom > 0 ? s.PatientPerRoom : 4,
      };

      this.PanelConfig = new RoomPanelConfig
      {
        ItemRows = this.Config.ItemRows,
        RoomTitle = "진료실·진료센터",
        DoctorTitle = "담당의사",
        WaitMesages = new List<string> { "들어오실분", "다음 순서 입니다", "잠시만 기다려 주십시오" }
      };

      var depts = s.Medical?.DeptRooms ?? throw new ServiceException("opdroom");

      foreach (var dept in depts)
      {
        foreach (var room in dept.Rooms)
        {
          var x = new OpdRoomConfig
          {
            DeptCode = room.DeptCode,
            RoomCode = room.RoomCode,
            RoomName = room.RoomName,
          };

          switch (dept.RoomType)
          {
            case "A": this.OfficeRooms.Add(x); break;
            case "B": this.ExamRooms.Add(x); break;
            default:
              LOG.ec($"Unknown RoomType {dept.RoomType}");
              break;
          }
        }
      }
    }

    public class ContentConfig
    {
      public int RoomPerPage { get; set; }
      public int ItemRows { get; set; }
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