using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public enum TitlebarStyle
  {
    None,
    LargeNormal,
    MiddleNormal,
    Custom,
  }
  public enum BottomStyle
  {
    None,
    LargeNotice,
    MiddleNotice,
  }

  public abstract class PackageViewConfig : PackageViewConfigBase
  {
    public bool IsSeoul { get; set; } = true;
    public TitlebarStyle TitlebarStyle = TitlebarStyle.None;
    public BottomStyle BottomStyle = BottomStyle.None;
    public override bool ShowTitleBar { get => this.TitlebarStyle != TitlebarStyle.None; }
    public override bool ShowBottomArea { get => this.BottomStyle != BottomStyle.None && this.Notice.UseNotice; }
    public PackageViewConfig(PACKAGE p) : base(p) { }
    public PackageViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s)
    {
      this.IsSeoul = s.HospitalCode == "01";
    }
  }
}