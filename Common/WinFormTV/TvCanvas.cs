using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using TVServices.Core;
using WinTVPublic;

namespace WinFormTV
{
  // 주의
  // WinTV 카드가 없는 컴퓨터에서 실행할 수 있으므로 WinTV를 분리하여
  // 관련 라이브러리가 노출되지 않도록 한다.
  // 또한 DLL도 예전것으로 하자
  public class TvCanvas : Canvas, IDisposable
  {
    WinTV winTV;
    bool _loaded = false;

    public TvCanvas()
    {
      this.ClipToBounds = true;
      this.SizeChanged += (s, e) => this.winTV?.Resize(this.ActualWidth, this.ActualHeight);
      this.Loaded += (s, e) =>
      {
        _loaded = true;
        this.OptionChanged();
      };
    }

    void OptionChanged()
    {
      if (this.UseTV && _loaded)
      {
        if (this.winTV == null)
        {
          this.winTV = new WinTV();
          this.Children.Add(this.winTV.WpfHost);
        }
        this.winTV.ShowTV(this.IsPlaying);
        if (this.IsPlaying)
        {
          if (this.TvChannel > 0)
          {
            this.winTV.Watch(this.TvChannel);
          }
        }
      }
      else
      {
        this.Dispose();
      }
    }

    #region DP
    static void OptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as TvCanvas)?.OptionChanged();
    }
    /// <summary>
    /// TV 사용 여부
    /// </summary>
    public bool UseTV { get => (bool)GetValue(UseTVProperty); set => SetValue(UseTVProperty, value); }
    public static readonly DependencyProperty UseTVProperty = DependencyProperty.Register(nameof(UseTV),
                        typeof(bool), typeof(TvCanvas),
                        new PropertyMetadata(false, OptionChanged));

    /// <summary>
    /// TV channel
    /// </summary>
    public int TvChannel { get => (int)GetValue(TvChannelProperty); set => SetValue(TvChannelProperty, value); }
    public static readonly DependencyProperty TvChannelProperty = DependencyProperty.Register(nameof(TvChannel),
                        typeof(int), typeof(TvCanvas),
                        new PropertyMetadata(0, OptionChanged));

    /// <summary>
    /// TV 재생여부
    /// </summary>
    public bool IsPlaying { get => (bool)GetValue(IsPlayingProperty); set => SetValue(IsPlayingProperty, value); }
    public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(nameof(IsPlaying),
                        typeof(bool), typeof(TvCanvas),
                        new PropertyMetadata(false, OptionChanged));
    #endregion DP

    public void Dispose()
    {
      this.winTV?.Dispose();
      this.winTV = null;
    }
  }

  public class WinTV : IDisposable
  {
    ServiceFactory serviceFactory;
    LiveTVServices liveTVServices;
    List<Channel> channels;
    int _channelNo;

    public WinTV()
    {
      this.PicBox = this.CreatePictureBox();
      this.WpfHost = this.CreateWindowsFormsHost();
      this.WpfHost.Child = this.PicBox;
      serviceFactory = ServiceFactory.GetInstance();
      channels = serviceFactory.GetAllChannels();
      LOG.ic($"channels.count={channels.Count}");
    }

    void Watch() => this.Watch(_channelNo);
    public bool Watch(int channelNo)
    {
      this.StopTimer();

      LOG.ic($"Channel NO : {channelNo}");

      if (channelNo == 0)
      {
        LOG.ec("channel no = 0");
        return false;
      }

      _channelNo = channelNo;
      var channel = get_channel(_channelNo);

      _is_channel_error = channel == null;

      if (_is_channel_error)
      {
        LOG.ec($"channel error: {channelNo}");
      }
      else
      {
        if (liveTVServices == null)
        {
          LOG.ic("start play");
          liveTVServices = serviceFactory.StartLiveTV(PicBox.Handle, channel);
        }
        else
        {
          if (!liveTVServices.SwitchToChannel(channel))
          {
            LOG.ec($"channel switch error: {channelNo}");
            _is_channel_error = true;
          }
        }
      }
      this.StartTimer();
      return !_is_channel_error;
    }

    void error_recover()
    {
      LOG.ec($"recovering: channel error: {_is_channel_error}");
      // 재진입을 예방하기 위해서 타이머 중지
      this.StopTV();
      if (_is_channel_error)
      {
        Thread.Sleep(3000); // 테스트 목적
        channels = serviceFactory.GetAllChannels();
      }
      this.Watch();
    }

    Channel get_channel(int channel_no)
    {
      if (channels != null)
      {
        return channels.Where(x => x.PreferredNumber == channel_no).FirstOrDefault();
      }
      return null;
    }

    #region UI 구성
    PictureBox PicBox;
    public WindowsFormsHost WpfHost;

    public void ShowTV(bool show = true)
    {
      LOG.ic($"show = {show}");
      this.WpfHost.Child.Visible = show;
      this.WpfHost.Visibility = show ? Visibility.Visible : Visibility.Hidden;
    }
    public void Resize(double width, double height)
    {
      LOG.ic($"{width} x {height}");
      this.PicBox.Width = (int)width;
      this.PicBox.Height = (int)height;
      this.liveTVServices?.WindowResized();
    }
    PictureBox CreatePictureBox()
    {
      var pic = new PictureBox();
      pic.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
                  | AnchorStyles.Left)
                  | AnchorStyles.Right)));
      pic.BackColor = System.Drawing.Color.Black;
      pic.Location = new System.Drawing.Point(0, 0);
      pic.Name = "pictureBox1";
      pic.TabIndex = 0;
      pic.TabStop = false;
      pic.Visible = true;
      return pic;
    }
    WindowsFormsHost CreateWindowsFormsHost()
    {
      var host = new WindowsFormsHost();
      host.ClipToBounds = true;
      host.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
      host.VerticalAlignment = System.Windows.VerticalAlignment.Top;
      host.Visibility = System.Windows.Visibility.Visible;
      return host;
    }
    #endregion UI 구성

    #region Timer
    DispatcherTimer _timer;
    bool _is_channel_error;

    void StartTimer()
    {
      if (_timer == null)
      {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
        _timer.Tick += (s, e) =>
        {
          if (_is_channel_error)
          {
            this.error_recover();
          }
          else
          {
            if (liveTVServices != null)
            {
              if (liveTVServices.Tick() == false)
              {
                LOG.e("liveTVServices.Tick() error");
                this.error_recover();
              }
            }
          }
        };
      }
      _timer.Start();
    }
    void StopTimer() => _timer?.Stop();
    #endregion Timer

    public void StopTV()
    {
      LOG.dc("stop");
      StopTimer();
      liveTVServices?.Stop();
      liveTVServices = null;
    }

    public void Dispose()
    {
      this.StopTV();
    }
  }
}