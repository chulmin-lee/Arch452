using System.Windows;

namespace UIControls
{
  /// <summary>
  /// 기본 윈도우
  /// </summary>
  public class CustomWindow : CustomWindowBase
  {
    static CustomWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow),
          new FrameworkPropertyMetadata(typeof(CustomWindow)));
    }
  }

  //-------------------------------------------------
  // 팝업용
  //-------------------------------------------------
  // Icon 없음, 제목 왼쪽 정렬, Close button
  //
  //-------------------------------------------------
  public class CustomPopupWindow : CustomWindowBase
  {
    static CustomPopupWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomPopupWindow),
          new FrameworkPropertyMetadata(typeof(CustomPopupWindow)));
    }
  }

  //-------------------------------------------------
  // 팝업용
  //-------------------------------------------------
  // Icon 없음, 제목 가운데 정렬, Close button 없음
  //-------------------------------------------------
  public class CustomSplashWindow : CustomWindowBase
  {
    static CustomSplashWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomSplashWindow),
          new FrameworkPropertyMetadata(typeof(CustomSplashWindow)));
    }
  }

  //-------------------------------------------------
  // 플러그인 용
  //-------------------------------------------------
  // Menu, ContextMenu
  // Close button
  // Title : 왼쪽 정렬
  //-------------------------------------------------
  public class CustomDialogWindow : CustomWindowBase
  {
    // TitleWidth 추가

    static CustomDialogWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDialogWindow),
          new FrameworkPropertyMetadata(typeof(CustomDialogWindow)));
    }
  }
  //-------------------------------------------------
  // 플러그인중 Process 용 ( 화면폭 조절)
  //-------------------------------------------------
  // Menu, ContextMenu
  // Close button
  // Title : 왼쪽 정렬, Title Width 가변
  //-------------------------------------------------
  public class ProcessDialogWindow : CustomWindowBase
  {
    // TitleWidth 추가

    static ProcessDialogWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessDialogWindow),
          new FrameworkPropertyMetadata(typeof(ProcessDialogWindow)));
    }
  }
}
