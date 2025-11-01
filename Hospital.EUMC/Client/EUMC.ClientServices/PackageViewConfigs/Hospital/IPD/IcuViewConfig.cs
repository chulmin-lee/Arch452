using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Collections.Generic;

namespace EUMC.ClientServices
{
  public class IcuViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public List<IcuRoomConfig> Rooms { get; private set; } = new List<IcuRoomConfig>();
    public IcuViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.LargeNormal;
      this.BottomStyle = BottomStyle.LargeNotice;

      this.Config = new ContentConfig()
      {
        UseRotation = true,
        ItemRows = s.DelayPerson > 0 ? s.DelayPerson : 8,
        //IsStaff = s.PackageName == PackageNames.ICU_STAFF,
        //IsBaby = s.PackageName == PackageNames.ICU_BABY
      };

      var icus = s.Medical?.Icus ?? throw new ServiceException("icus");
      foreach (var icu in icus)
      {
        this.Rooms.Add(new IcuRoomConfig() { IcuCode = icu.DeptCode, IcuName = icu.DeptName, Title = icu.Title });
      }
    }

    public class ContentConfig
    {
      public bool IsStaff;
      public bool IsBaby;
      public bool UseRotation { get; set; }
      public int ItemRows { get; set; }  // 부서당 표시 환자수
    }
  }
}