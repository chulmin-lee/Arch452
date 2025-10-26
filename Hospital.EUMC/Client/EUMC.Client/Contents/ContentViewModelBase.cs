using Common;
using EUMC.ClientServices;
using ServiceCommon;
using ServiceCommon.ClientServices;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using UIControls;

namespace EUMC.Client
{
  internal class ContentViewModelBase : ViewModelBase, IContentViewModel
  {
    protected IPackageViewConfig Config { get; private set; }
    public PACKAGE ClientPackage => Config.Package;
    bool _screenOn = false;
    public bool IsScreenOn { get => _screenOn; set => Set(ref _screenOn, value); }
    //===================================
    // Window Style
    //===================================
    public WindowScreenType WindowType => Config.WindowType;
    public bool IsWideContent => Config.IsWideContent;
    //==================================
    // Titlebar
    //==================================
    public bool ShowTitleBar => Config.ShowTitleBar;
    public TitlebarStyle TitlebarStyle { get; private set; }
    string _contentTitle = string.Empty;
    public string ContentTitle { get => _contentTitle; set => Set(ref _contentTitle, value); }
    public ClockSetting ClockSetting { get; set; } = new ClockSetting()
    {
      YearLength = 2,
      YearDelimiter = ".",
    };

    //==================================
    // Bottom Area
    //==================================
    public bool ShowBottomArea => this.Config.ShowBottomArea;
    public BottomStyle BottomStyle { get; private set; }
    public NoticeConfig Notice => Config.Notice;

    public ContentViewModelBase(IPackageViewConfig config)
    {
      LOG.w($"Contetnt Created: {config.Package}");
      this.Config = config;
      this.IsScreenOn = config.IsScreenOn;
      this.ContentTitle = config.ContentTitle;

      var o = this.Config as PackageViewConfig ?? throw new Exception("this.Config");
      this.TitlebarStyle = o.TitlebarStyle;
      this.BottomStyle = o.BottomStyle;
    }

    public int ContentWidth { get; set; }
    public int ContentHeight { get; set; }

    public virtual bool MessageReceived(ServiceMessage m)
    {
      if (m.ServiceId == SERVICE_ID.USER_MESSAGE)
      {
        var msg = m.CastTo<UserMessage>();
        if (msg.Type == USER_MESSAGE_TYPE.SCREEN_ON_OFF)
        {
          var screen = msg.CastTo<ScreenOnOffMessage>();
          this.IsScreenOn = screen.ScreenOn;
          this.ScreenOnOff(this.IsScreenOn);
          return true;
        }
      }
      return false;
    }
    protected virtual void ScreenOnOff(bool on) { }
    public virtual void Close()
    {
      this.Dispose();
    }
    bool _disposed;
    protected override void Dispose(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
        {
          this.ContentClose();
        }
        _disposed = true;
      }
      base.Dispose(disposing);
    }
    protected virtual void ContentClose() { }

    public virtual List<KeyBinding> KeyBindings()
    {
      return new List<KeyBinding>();
    }
  }
}