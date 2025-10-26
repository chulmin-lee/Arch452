using ServiceCommon;
using ServiceCommon.ClientServices;
using EUMC.ClientServices;
using System.Windows.Input;
using UIControls;
using System.Collections.Generic;
using System;

namespace EUMC.Client
{
  public interface IContentViewModel : IDisposable
  {
    PACKAGE ClientPackage { get; }
    bool IsScreenOn { get; set; }
    //===================================
    // Window Style
    //===================================
    /// <summary>
    /// 1. content 를 표시할 윈도우 선택 (Code behinde)
    /// 2. Debug 모드시 창 크기 조절에 사용 (Code behinde)
    /// </summary>
    WindowScreenType WindowType { get; }
    /// <summary>
    /// Wide Content 여부
    /// - debug 모드에서 창 resize에 사용 (Code behinde)
    /// - notice 크기, 폰트 크기 결정
    /// - popup 출력시 크기/위치 결정
    /// </summary>
    bool IsWideContent { get; }

    //==================================
    // Titlebar
    //==================================
    bool ShowTitleBar { get; }
    /// <summary>
    /// content category
    /// </summary>
    TitlebarStyle TitlebarStyle { get; }
    string ContentTitle { get; set; }
    ClockSetting ClockSetting { get; }

    //==================================
    // Bottom Area
    //==================================
    bool ShowBottomArea { get; }
    BottomStyle BottomStyle { get; }
    NoticeConfig Notice { get; }

    /// <summary>
    /// Custom Window
    /// </summary>
    int ContentWidth { get; }
    int ContentHeight { get; }

    bool MessageReceived(ServiceMessage m);
    List<KeyBinding> KeyBindings();

    void Close();
  }
}