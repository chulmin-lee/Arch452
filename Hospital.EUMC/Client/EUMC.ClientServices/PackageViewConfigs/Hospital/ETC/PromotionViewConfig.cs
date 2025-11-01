using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Collections.Generic;

namespace EUMC.ClientServices
{
  public class PromotionViewConfig : PackageViewConfig
  {
    public ContentConfig Config { get; set; } = new ContentConfig();
    public int Duration { get; set; }
    public bool UseTV { get; set; } = false;
    public int ChannelNo { get; set; } = 0;
    public string ChannelName { get; set; } = string.Empty;
    public PromotionViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.TitlebarStyle = TitlebarStyle.None;
      this.BottomStyle = BottomStyle.None;

      this.PackageInfo.UseService = false;
      this.Duration = s.Duration;
      if (s.TVSetting.UseTV)
      {
        this.UseTV = true;
        this.ChannelNo = s.TVSetting.TvChannelNo;
        this.ChannelName = s.TVSetting.TvChannelName;
      }
      this.Config = new ContentConfig
      {
        MediaVolumn = s.MediaVolume, // 0 ~ 1.0
      };
    }
    public class ContentConfig
    {
      public double MediaVolumn { get; set; } = 0.5;
    }
  }
}