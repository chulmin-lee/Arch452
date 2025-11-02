using System.Collections.Generic;

namespace ServiceCommon.ClientServices
{
  public enum WindowScreenType
  {
    Predefined = 0,
    Custom = 1,
    Maximized = 2,
  }

  public interface IPackageViewConfig
  {
    PACKAGE Package { get; }
    PackageInfo PackageInfo { get; }

    //==================================
    // Content 표시
    //==================================
    /// <summary>
    /// content 표시 여부
    /// </summary>
    bool IsScreenOn { get; }

    //==================================
    // Window Style
    //==================================
    /// <summary>
    /// 1. window 타입 결정
    /// 2. debug 모드일때 윈도우 resize 결정
    /// </summary>
    WindowScreenType WindowType { get; }

    bool IsWideContent { get; }

    //==================================
    // Titlebar
    //==================================
    bool ShowTitleBar { get; }
    string ContentTitle { get; }

    //==================================
    // Bottom Area
    //==================================
    bool ShowBottomArea { get; }
    NoticeConfig Notice { get; }

    //==================================
    // ETC
    //==================================
    bool CanReboot { get; }
    List<CONTENT_FILE> ContentFiles { get; }
  }

  public abstract class PackageViewConfigBase : IPackageViewConfig
  {
    public PackageInfo PackageInfo { get; protected set; } = new PackageInfo();
    public PACKAGE Package { get; protected set; } = PACKAGE.NONE;

    //==================================
    // Content 표시
    //==================================
    public bool IsScreenOn { get; protected set; } = true;

    //==================================
    // Window Style
    //==================================
    public WindowScreenType WindowType { get; protected set; } = WindowScreenType.Predefined;
    public bool IsWideContent { get; protected set; } = true;

    //==================================
    // Titlebar
    //==================================
    public virtual bool ShowTitleBar { get; } = true;
    public string ContentTitle { get; protected set; } = string.Empty;

    //==================================
    // Bottom Area
    //==================================
    public virtual bool ShowBottomArea { get; } = true;
    public NoticeConfig Notice { get; protected set; } = new NoticeConfig();

    //==================================
    // ETC
    //==================================
    public bool CanReboot { get; protected set; } = true;
    public List<CONTENT_FILE> ContentFiles { get; set; } = new List<CONTENT_FILE>();

    public PackageViewConfigBase(PACKAGE p)
    {
      this.Package = p;
      this.PackageInfo.Package = p;
    }

    public PackageViewConfigBase(PackageInfo p, PlaylistSchedule s)
    {
      this.PackageInfo = p;
      this.Package = p.Package;
      this.Notice = s.NoticeConfig;
      this.ContentFiles = s.ContentFiles;
    }
  }

  public class IcuRoomConfig
  {
    public string IcuCode { get; set; } = string.Empty;
    public string IcuName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
  }

  public class OpdRoomConfig
  {
    public string DeptCode { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string DurationTime { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public string GetKey()
    {
      return $"{DeptCode}:{RoomCode}";
    }
  }
}