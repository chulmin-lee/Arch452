using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.ClientServices
{
  public class OfficeMultiViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public List<OpdRoomConfig> OfficeRooms { get; private set; } = new List<OpdRoomConfig>();
    public OfficeMultiViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.LargeNotice;
      this.ContentTitle = s.Medical?.LargeTitle ?? string.Empty;

      this.Config = new ContentConfig
      {
        RoomPerPage = 5,
        ItemRows = s.DelayPerson > 0 ? s.DelayPerson : 5,
        UseRotation = true,
      };

      var depts = s.Medical?.DeptRooms ?? throw new ServiceException("opdroom");
      foreach (var dept in depts.Where(x => x.RoomType == "A"))
      {
        foreach (var room in dept.Rooms)
        {
          this.OfficeRooms.Add(new OpdRoomConfig
          {
            DeptCode = room.DeptCode,
            DeptName = room.DeptName,
            RoomCode = room.RoomCode,
            RoomName = room.RoomName,
            DurationTime = room.DurationTime,
            Title = room.RoomTitle
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