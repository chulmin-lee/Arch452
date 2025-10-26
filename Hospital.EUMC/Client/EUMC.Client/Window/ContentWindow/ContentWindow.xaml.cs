using Common;
using ServiceCommon.ClientServices;
using EUMC.ClientServices;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UIControls;
using System;

namespace EUMC.Client
{
  /// <summary>
  /// Interaction logic for ScreenView.xaml
  /// </summary>
  public partial class ContentWindow : Window
  {
    IWindowViewModel VM;
    int ScreenWidth => (int)SystemParameters.PrimaryScreenWidth;
    int ScreenHeight => (int)SystemParameters.PrimaryScreenHeight;
    public ContentWindow(IWindowViewModel vm)
    {
      InitializeComponent();

      UIContextHelper.Initialize(this);

      this.VM = vm;
      this.DataContext = this.VM;
      this.Closing += (s, e) =>
      {
        this.VM.Close();
        CLIENT_SERVICE.Close();
      };
      this.VM.ContentChanged += ContentChanged;
      this.ShortcutBinding();
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

#if !DEBUG
    Cursor = Cursors.None;
    Taskbar.Hide();
    this.Closing += (s, e) => { Taskbar.Show(); };
    this.Topmost = true;

    this.SizeToContent = SizeToContent.Manual; // 기본값
    this.WindowStartupLocation = WindowStartupLocation.Manual;
    this.WindowStyle = WindowStyle.None;
    this.ResizeMode = ResizeMode.NoResize;
    this.WindowState = WindowState.Maximized;
#endif
    }
    private void ContentChanged(object sender, EventArgs e)
    {
#if DEBUG
      LOG.ic($"{this.VM.Contents?.ClientPackage}");

      if (VM.Contents == null)
      {
        LOG.e("Contents is null, cannot display content.");
        return;
      }
      var o = VM.Contents;

      this.SizeToContent = SizeToContent.WidthAndHeight; // WidthAndHeight로 해야 scale 조절이 된다
      this.WindowStartupLocation = WindowStartupLocation.Manual;
      this.Left = 0;
      this.Top = 0;
      this.KeyDown += Window_KeyDown;

      switch (o.WindowType)
      {
        case WindowScreenType.Predefined:
          {
            this.Width = this.WindowOuterBorder.Width = o.IsWideContent ? Math.Max(this.ScreenWidth, this.ScreenHeight) : Math.Min(this.ScreenWidth, this.ScreenHeight);
            this.Height = this.WindowOuterBorder.Height = o.IsWideContent ? Math.Min(this.ScreenWidth, this.ScreenHeight) : Math.Max(this.ScreenWidth, this.ScreenHeight);
          }
          break;
        case WindowScreenType.Maximized:
          {
            this.Width = this.WindowOuterBorder.Width = this.ScreenWidth;
            this.Height = this.WindowOuterBorder.Height = this.ScreenHeight;
          }
          break;
      }
      this.WindowOuterBorder.LayoutTransform = new ScaleTransform(0.5, 0.5);
#endif
    }

    void ShortcutBinding()
    {
      this.InputBindings.Clear();
      var list = VM.KeyBindings();
      foreach (var cmd in list)
      {
        this.InputBindings.Add(cmd);
      }
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      Debug.WriteLine("CurrentDomain_UnhandledException " + ((Exception)e.ExceptionObject).Message + " Is Terminating: " + e.IsTerminating.ToString());
    }

    //#if DEBUG
    void LandscapeChanged(bool e)
    {
      UIContextHelper.CheckInvokeOnUI(() =>
      {
        var w = SystemParameters.PrimaryScreenWidth;
        var h = SystemParameters.PrimaryScreenHeight;

        this.WindowOuterBorder.Width = e ? w : h;
        this.WindowOuterBorder.Height = e ? h : w;
      });
    }

    public override void OnApplyTemplate()
    {
#if DEBUG
      this.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
      this.MouseMove += TitleBar_MouseMove;
      this.MouseLeftButtonUp += TitleBar_MouseLeftButtonUp;
#endif
      base.OnApplyTemplate();
    }

    protected bool isMaximizedDrag;  // 최대화 상태에서 드래그
    protected void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
      if (WindowState == WindowState.Maximized)
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
    private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      isMaximizedDrag = false;
    }

    T GetRequiredTemplateChild<T>(string childName) where T : DependencyObject
    {
      return (T)base.GetTemplateChild(childName);
    }

    //Window? _optionWin;
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.System:
          {
            //if (e.SystemKey == Key.F10)
            //{
            //  if (_optionWin == null)
            //  {
            //    _optionWin = new OptionView(VM);
            //    _optionWin.Closed += (s, e) => { _optionWin = null; };
            //    _optionWin.Show();
            //  }
            //}
          }
          break;

        case Key.F11:
          {
            double v = this.WindowOuterBorder.LayoutTransform.Value.M11; // M11, M22
            v = (v == 0.5) ? 1.0 : 0.5;
            this.WindowOuterBorder.LayoutTransform = new ScaleTransform(v, v);
          }
          break;
      }
    }
    //#endif
  }
}