using Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UIControls
{
  public partial class MarqueeControl : UserControl
  {
    Storyboard _storyboard;
    public MarqueeControl()
    {
      InitializeComponent();
      this.SizeChanged += (s, e) => this.refresh("size");
      this.IsVisibleChanged += (s, e) => this.refresh("vis");
      this.Loaded += (s, e) =>
      {
        LOG.d("MarqueeControl::Loaded");
        this.refresh("load");
      };
    }
    void refresh(string s)
    {
      if (!this.IsLoaded || !this.IsVisible) return;
      var canvasWidth = OutBorder.ActualWidth;
      if (canvasWidth <= 0) return;
      if (string.IsNullOrEmpty(this.Message)) return;

      LOG.wc(s);
      _storyboard?.Stop();
      // Caption.Content는 this.Message에 바인딩되어있지만, 이 시점에서 실제로 값이 바인딩되었는지는
      // 보장할 수 없다
      // this.Caption.Content = this.Message;
      // scroll 설정 (메시지가 화면보다 긴 경우 전체를 회전할 시간 계산)
      var textWidth = this.Caption.MeasureText(this.Message).Width;
      if (this.AlwaysScroll || textWidth > canvasWidth)
      {
        int time = this.ScrollSpeed * (int)Math.Round(textWidth / canvasWidth + 0.5);
        var animation = new DoubleAnimation()
        {
          From = canvasWidth,
          To = - textWidth,
          Duration = new Duration(new TimeSpan(0,0,time)),
          RepeatBehavior = RepeatBehavior.Forever,
        };

        Storyboard.SetTarget(animation, this.Caption);
        Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));

        _storyboard = new Storyboard();
        _storyboard.Children.Add(animation);
        _storyboard.Begin();
      }
      else
      {
        // 가운데 정렬
        double d = (canvasWidth - textWidth) / 2;
        this.Caption.SetValue(Canvas.LeftProperty, d);
      }
    }

    static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is MarqueeControl control && control.IsLoaded)
      {
        control.refresh("binding");
      }
    }

    #region Property
    public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message),
                            typeof(string), typeof(MarqueeControl),
                            new PropertyMetadata(null, OnValueChanged));

    public int ScrollSpeed { get => (int)GetValue(ScrollSpeedProperty); set => SetValue(ScrollSpeedProperty, value); }
    public static readonly DependencyProperty ScrollSpeedProperty = DependencyProperty.Register(nameof(ScrollSpeed),
                            typeof(int), typeof(MarqueeControl),
                            new PropertyMetadata(10, OnValueChanged));
    /// <summary>
    /// 무조건 회전. false면 화면을 넘는 경우에만 회전
    /// </summary>
    public bool AlwaysScroll { get => (bool)GetValue(AlwaysScrollProperty); set => SetValue(AlwaysScrollProperty, value); }
    public static readonly DependencyProperty AlwaysScrollProperty = DependencyProperty.Register(nameof(AlwaysScroll),
                            typeof(bool), typeof(MarqueeControl),
                            new PropertyMetadata(false, OnValueChanged));
    #endregion Property
  }
}