using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public enum TitlebarStyle
  {
    None,

    LargeNormal,       // 병원 기본 대대합 layout (보통 titlebar, notice 포함이지만, 사이트마다 달라질수있다)
    LargeNormalBlack,
    MiddleNormal,      // 병원 기본 중대합 (No titlebar, No Notice, 하단 로고)
    MiddleExamRoom,    // 이미지 타이틀바
    Custom,            // updater 처럼 특정 크기를 갖는 경우
  }
  public enum BottomStyle
  {
    None,
    LargeNotice,
    MiddleNotice,
    Logo,
  }

  public abstract class PackageViewConfig : PackageViewConfigBase
  {
    public TitlebarStyle TitlebarStyle = TitlebarStyle.None;
    public BottomStyle BottomStyle = BottomStyle.None;
    public override bool ShowTitleBar { get => this.TitlebarStyle != TitlebarStyle.None; }
    public override bool ShowBottomArea { get => this.BottomStyle != BottomStyle.None; }
    public PackageViewConfig(PACKAGE p) : base(p) { }
    public PackageViewConfig(PackageInfo p, PlaylistSchedule s) : base(p, s) { }
  }
}