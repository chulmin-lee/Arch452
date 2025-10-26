using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows;
using WpfScreenHelper;

namespace UIControls
{


  public class NeoVisionWindowBase : Window
  {
    public Grid TitleBar { get; private set; }


    public NeoVisionWindowBase()
    {
      this.Loaded += Window_Loaded;
      this.SizeChanged += Window_SizeChanged;
      this.StateChanged += Window_StateChanged;
      SystemEvents.DisplaySettingsChanged += Window_DisplaySettingsChanged;
    }

    #region OVERRIDE
    public override void OnApplyTemplate()
    {
      this.TitleBar = this.GetRequiredTemplateChild<Grid>("PART_TitleBar");

      if (this.TitleBar != null)
      {
        this.TitleBar.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
        this.TitleBar.MouseMove += TitleBar_MouseMove;
        this.TitleBar.MouseLeftButtonUp += TitleBar_MouseLeftButtonUp;
      }
      base.OnApplyTemplate();
    }
    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      //HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
      //source.AddHook(WindowProc);
      //IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
      //HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
    }
    IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      switch (msg)
      {
        case 0x0024:  // Window Size or Position change
          WmGetMinMaxInfo(hwnd, lParam);
          break;
      }
      return IntPtr.Zero;
    }
    static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
    {
      var lPrimaryScreen = Screen.FromPoint(new Point(0, 0));  // primary는 (0,0)에서 시작
      var pt = MouseHelper.MousePosition;  // virtual screen 에서 좌표
      var lCurrentScreen = Screen.FromPoint(pt);
    }
    #endregion

    #region USER_EVENT_HANDLER

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      //  this.SizeToContent = SizeToContent.WidthAndHeight;
      //  this.Loaded -= CustomWindowBase_Loaded;
    }
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
    }
    private void Window_StateChanged(object sender, EventArgs e)
    {
    }
    private void Window_DisplaySettingsChanged(object sender, EventArgs e)
    {
      this.ToggleWindowState();
      this.ToggleWindowState();
    }
    #endregion

    #region WINDOW_SIZE_AND_DRAG
    void ToggleWindowState()
    {
      this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
    }
    protected bool isMaximizedDrag;  // 최대화 상태에서 드래그
    protected void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var position = e.GetPosition(this);
      if (position.X <= 50)
      {
        if (e.ClickCount != 2)
        {
          this.OpenSystemContextMenu(e);
        }
        else
        {
          base.Close();
        }
        e.Handled = true;
        return;
      }

      e.Handled = true;
      if (e.ClickCount == 2)
      {
        if (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip)
        {
          this.ToggleWindowState();
        }
        return;
      }
      else if (WindowState == WindowState.Maximized)
      {
        this.isMaximizedDrag = true;
        return;
      }
      DragMove();
    }
    private void TitleBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (this.isMaximizedDrag && e.LeftButton == MouseButtonState.Pressed)
      {
        isMaximizedDrag = false;
        var point = PointToScreen(e.GetPosition(this));

        // 이전 위치 (RestoreBounds)가 아닌 최상위 가운데 위치하도록 한다.
        Left = point.X - (RestoreBounds.Width * 0.5);
        Top = point.Y - 16;
        WindowState = WindowState.Normal;
        this.DragMove();
      }
    }
    //private void TitleBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    //{
    //  if (this.isMaximizedDrag)
    //  {
    //    isMaximizedDrag = false;

    //    var restore = this.RestoreBounds;
    //    var position = e.GetPosition(this);
    //    var screen = DialogHelper.GetCurrentScreen(this);
    //    var rect = screen.WorkingArea;

    //    // screen.Width 기준 X 를 restore.Width 기준 X로 변환
    //    var x = (position.X * restore.Width) / rect.Width;

    //    this.Left = position.X - x + rect.X;
    //    this.Top = 0;
    //    this.WindowState = WindowState.Normal;
    //    this.DragMove();
    //  }
    //}
    private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      isMaximizedDrag = false;
    }
    #endregion


    public virtual void CloseWindow()
    {
      this.Close();
    }
    Thickness GetDefaultMarginForDpi()
    {
      int currentDPI = DialogHelper.GetCurrentDPI();
      Thickness thickness = new Thickness(8, 8, 8, 8);
      if (currentDPI == 120)
      {
        thickness = new Thickness(7, 7, 4, 5);
      }
      else if (currentDPI == 144)
      {
        thickness = new Thickness(7, 7, 3, 1);
      }
      else if (currentDPI == 168)
      {
        thickness = new Thickness(6, 6, 2, 0);
      }
      else if (currentDPI == 192)
      {
        thickness = new Thickness(6, 6, 0, 0);
      }
      else if (currentDPI == 240)
      {
        thickness = new Thickness(6, 6, 0, 0);
      }
      return thickness;
    }

    Thickness GetFromMinimizedMarginForDpi()
    {
      int currentDPI = DialogHelper.GetCurrentDPI();
      Thickness thickness = new Thickness(7, 7, 5, 7);
      if (currentDPI == 120)
      {
        thickness = new Thickness(6, 6, 4, 6);
      }
      else if (currentDPI == 144)
      {
        thickness = new Thickness(7, 7, 4, 4);
      }
      else if (currentDPI == 168)
      {
        thickness = new Thickness(6, 6, 2, 2);
      }
      else if (currentDPI == 192)
      {
        thickness = new Thickness(6, 6, 2, 2);
      }
      else if (currentDPI == 240)
      {
        thickness = new Thickness(6, 6, 0, 0);
      }
      return thickness;
    }
    T GetRequiredTemplateChild<T>(string childName) where T : DependencyObject
    {
      return (T)base.GetTemplateChild(childName);
    }
    void OpenSystemContextMenu(MouseButtonEventArgs e)
    {
      var position = e.GetPosition(this);
      var screen = this.PointToScreen(position);
      int num = 36;

      if (position.Y < (double)num)
      {
        IntPtr handle = new WindowInteropHelper(this).Handle;
        //IntPtr systemMenu = NativeUtils.GetSystemMenu(handle, false);

        bool isMax = this.WindowState == WindowState.Maximized;
        bool resizable = this.ResizeMode != ResizeMode.NoResize;

        IntPtr systemMenu = NativeUtils.GetSystemMenu(handle, isMax, resizable);
        int num1 = NativeUtils.TrackPopupMenuEx(systemMenu, NativeUtils.TPM_LEFTALIGN | NativeUtils.TPM_RETURNCMD,
                     Convert.ToInt32(screen.X + 2), Convert.ToInt32(screen.Y + 2), handle, IntPtr.Zero);
        if (num1 == 0)
        {
          return;
        }

        NativeUtils.PostMessage(handle, 274, new IntPtr(num1), IntPtr.Zero);
      }
    }
  }
}
