using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Collections.Generic;

namespace EUMC.ClientServices
{
  public class MixRoomViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public List<OpdRoomConfig> ExamRooms { get; private set; } = new List<OpdRoomConfig>();
    public List<OpdRoomConfig> OfficeRooms { get; private set; } = new List<OpdRoomConfig>();
    public MixRoomViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.LargeNotice;
      this.ContentTitle = s.Medical?.LargeTitle ?? string.Empty;

      this.Config = new ContentConfig()
      {
        RoomPerPage = 5,
        ItemRows = s.DelayPerson > 0 ? s.DelayPerson : 7,
        UseRotation = true,
      };

      var opd = this.PackageInfo.OpdRoom ?? throw new ServiceException("OpdRoom  null");
      foreach (var dept in opd.DeptRooms)
      {
        if (dept.RoomType == "A")
        {
          this.OfficeRooms.Add(new OpdRoomConfig
          {
            DeptCode = dept.DeptCode,
            RoomCode = dept.RoomCode,
          });
        }
        else if (dept.RoomType == "B")
        {
          this.ExamRooms.Add(new OpdRoomConfig
          {
            DeptCode = dept.DeptCode,
            RoomCode = dept.RoomCode,
          });
        }
      }
    }

    public class ContentConfig
    {
      //============================
      // page 설정
      //============================
      public int RoomPerPage { get; set; }
      public int ItemRows { get; set; }
      /// <summary>
      /// 항상 보여줄 환자수, 이외의 환자는 롤링
      /// </summary>
      //public int AlwaysPatientRows { get; set; }  // 모두 롤링
      public bool UseRotation { get; set; }
    }
  }
}