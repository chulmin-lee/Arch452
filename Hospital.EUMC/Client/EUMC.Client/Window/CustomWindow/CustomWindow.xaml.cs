using Common;
using ServiceCommon.ClientServices;
using System;
using System.Windows;
using UIControls;

namespace EUMC.Client
{
  /// <summary>
  /// Interaction logic for UpdaterWindow.xaml
  /// </summary>
  public partial class CustomWindow : Window
  {
    int ScreenWidth => (int)SystemParameters.PrimaryScreenWidth;
    int ScreenHeight => (int)SystemParameters.PrimaryScreenHeight;
    IWindowViewModel VM;
    public CustomWindow(IWindowViewModel vm)
    {
      InitializeComponent();
      UIContextHelper.Initialize(this);

      this.VM = vm;
      this.DataContext = this.VM;
      this.Closing += (s, e) => this.VM.Close();
      this.VM.ContentChanged += ContentChanged;
    }

    private void ContentChanged(object sender, EventArgs e)
    {
      LOG.ic($"{this.VM.Contents?.ClientPackage}");

      if (VM.Contents == null)
      {
        LOG.e("Contents is null, cannot display content.");
        return;
      }
      var o = VM.Contents;
      switch (o.WindowType)
      {
        case WindowScreenType.Custom:
          {
            this.SizeToContent = SizeToContent.WidthAndHeight; // 기본값
            this.Visibility = Visibility.Collapsed;
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Visibility = Visibility.Visible;
            this.Left = (this.ScreenWidth - o.ContentWidth) / 2;
            this.Top = (this.ScreenHeight - o.ContentHeight) / 3;
          }
          break;
        default:
          throw new Exception($"not supported screen style : {o.WindowType}");
      }
      this.Activate();
    }
  }
}