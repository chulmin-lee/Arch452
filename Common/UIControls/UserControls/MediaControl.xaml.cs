//#define SAVE_CAPTURE

using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace UIControls
{
  public partial class MediaControl : UserControl
  {
    MediaManager MEDIA_MANAGER = new MediaManager();
    DispatcherTimer _image_timer;
    MEDIA_FILE _current;
    Dictionary<int, BitmapSource> _cache = new Dictionary<int, BitmapSource>();
    bool _loaded = false;
    public MediaControl()
    {
      InitializeComponent();

      this.myMedia.MediaOpened += (s, e) =>
      {
        this.myMedia.Visibility = Visibility.Visible;
        this.myImage.Visibility = Visibility.Hidden;
      };
      this.myMedia.MediaEnded += (s, e) => video_endede();
      this.myMedia.MediaFailed += (s, e) =>
      {
        // 재생 오류 발생시 해당 content 제거
        this.MEDIA_MANAGER.RemoveCurrent();
        this.play_next_content();
      };
      this.Loaded += (s, e) =>
      {
        _loaded = true;
        this.OptionChanged();
      };
    }

    void video_endede()
    {
      if (_current != null && _current.IsVideo)
      {
        // 영상 전환시 다음 영상이 출력되기전에 잠깐 검은 화면이 출력될수밖에 없다
        // 다음 영상이 출력되기전까지 이전 영상의 마지막 프레임을 이미지로 출력하여 이를 방지한다
        // 영상 출력 완료 후 캐쉬 이미지를 생성한다
        if (!_cache.ContainsKey(_current.MediaId))
        {
          // 주의: MediaElement 자체를 캡춰시 문제가 있어서 Grid 자체를 캡춰함
          // - 캡춰 영역이 MediaElement 영역을 걸쳐서 캡춰됨.
          var v = this.MainGrid;
          var ds = v.GetDpiScale();
          var rtb = new RenderTargetBitmap((int)v.ActualWidth, (int)v.ActualHeight,
                     ds.PixelsPerInchX, ds.PixelsPerInchY, PixelFormats.Default);
          rtb.Render(v);
          _cache.Add(_current.MediaId, rtb);

#if SAVE_CAPTURE
          var encoder = new PngBitmapEncoder();
          encoder.Frames.Add(BitmapFrame.Create(rtb));
          var name = $"{Path.GetFileNameWithoutExtension(_current.ContentPath)}.png";
          using (FileStream fileStream = new FileStream(name, FileMode.Create))
          {
            encoder.Save(fileStream);
          }
#endif
        }
      }
      this.play_next_content();
    }
    void play_next_content()
    {
      this.StopTimer();

      if (!this.UseMedia || !this.IsPlaying || this.MEDIA_MANAGER.IsEmpty)
      {
        return;
      }

      try
      {
        var prev = _current;
        _current = this.MEDIA_MANAGER.GetNextContent();
        if (_current != null)
        {
          // one way to source binding
          this.SetCurrentValue(MediaIdProperty, _current.MediaId);

          if (_current.IsImage)
          {
            var src = ImageLoader.LoadImage(_current.ContentPath);
            if (src != null)
            {
              this.display_image(src);
              if (this.MEDIA_MANAGER.ImageCount > 1)
                this.StartTimer();
            }
          }
          else
          {
            if (prev != null && prev.IsVideo)
            {
              if (_cache.TryGetValue(prev.MediaId, out var capture))
              {
                this.display_image(capture);
              }
            }

            LOG.d($"play video: {_current.ContentPath}");
            this.media_play(_current.GetUri());
          }
        }
      }
      catch (Exception ex)
      {
        LOG.except(ex);
      }
    }
    void media_play(Uri uri)
    {
      this.media_clear();
      this.myMedia.Source = uri;
      this.myMedia.Play();
    }
    void media_clear()
    {
      this.myMedia.Position = TimeSpan.Zero;
      this.myMedia.Stop();
      this.myMedia.Source = null;
    }
    void display_image(ImageSource s)
    {
      this.myImage.Source = s;
      this.myImage.Visibility = Visibility.Visible;
      this.myMedia.Visibility = Visibility.Hidden;
    }
    void StartTimer()
    {
      if (_image_timer == null)
      {
        _image_timer = new DispatcherTimer();
        _image_timer.Tick += (s, e) => play_next_content();
      }
      if (this.Duration > 0)
      {
        _image_timer.Interval = TimeSpan.FromSeconds(Duration);
        _image_timer.Start();
      }
    }
    void StopTimer()
    {
      _image_timer?.Stop();
    }

    void OptionChanged()
    {
      if (_loaded &&
          this.UseMedia &&
          this.IsPlaying &&
          this.Duration > 0 &&
          this.ItemsSource != null)
      {
        this.play_next_content();
        var mvs = DependencyPropertyHelper.GetValueSource(this, MediaIdProperty);
        LOG.w($"MediaId is binded : {mvs.IsExpression}");
      }
      else
      {
        this.StopTimer();
        this.media_clear();
      }
    }

    #region DependencyProperty
    static void OptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as MediaControl)?.OptionChanged();
    }

    #region ItemsSource
    public IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource),
                        typeof(IEnumerable), typeof(MediaControl),
                        new FrameworkPropertyMetadata(null, (d,e) => {(d as MediaControl)?.OnItemsSourceChanged(e); }));
    void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        itemssource_changed();

        var o = ItemsSource as INotifyCollectionChanged;
        if (o != null)
        {
          // CollectionChanged 인터페이스가 존재하는 경우 핸들러 연결
          o.CollectionChanged += (s, e1) => itemssource_changed();
        }
      }
    }
    void itemssource_changed()
    {
      this.StopTimer();
      this.media_clear();

      _cache.Clear();

      var list = new List<MEDIA_FILE>();
      if (ItemsSource != null)
      {
        foreach (MEDIA_FILE file in ItemsSource)
        {
          list.Add(file);
        }
      }
      this.MEDIA_MANAGER.SetContents(list);
      this.play_next_content();
    }
    #endregion ItemsSource

    public bool UseMedia { get => (bool)GetValue(UseMediaProperty); set => SetValue(UseMediaProperty, value); }
    public static readonly DependencyProperty UseMediaProperty = DependencyProperty.Register(nameof(UseMedia),
                        typeof(bool), typeof(MediaControl),
                        new PropertyMetadata(false, OptionChanged));

    public bool IsPlaying { get => (bool)GetValue(IsPlayingProperty); set => SetValue(IsPlayingProperty, value); }
    public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(nameof(IsPlaying),
                        typeof(bool), typeof(MediaControl),
                        new PropertyMetadata(false, OptionChanged));

    public int Duration { get => (int)GetValue(DurationProperty); set => SetValue(DurationProperty, value); }
    public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(Duration),
                        typeof(int), typeof(MediaControl),
                        new FrameworkPropertyMetadata(0, OptionChanged));

    public double MediaVolumn { get => (double)GetValue(MediaVolumnProperty); set => SetValue(MediaVolumnProperty, value); }
    public static readonly DependencyProperty MediaVolumnProperty = DependencyProperty.Register(nameof(MediaVolumn),
                        typeof(double), typeof(MediaControl),
                        new FrameworkPropertyMetadata(0.0));
    public int MediaId { get => (int)GetValue(MediaIdProperty); set => SetValue(MediaIdProperty, value); }
    public static readonly DependencyProperty MediaIdProperty = DependencyProperty.Register(nameof(MediaId),
            typeof(int), typeof(MediaControl),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    #endregion DependencyProperty
  }
}