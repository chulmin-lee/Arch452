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
      this.SizeChanged += (s, e) => this.refresh();
      this.IsVisibleChanged += (s, e) => this.refresh();
    }

    #region TickerMessage
    public string Message
    {
      get { return (string)GetValue(MessageProperty); }
      set { SetValue(MessageProperty, value); }
    }
    public static readonly DependencyProperty MessageProperty =
      DependencyProperty.Register(nameof(Message), typeof(string), typeof(MarqueeControl),
        new PropertyMetadata(null, new PropertyChangedCallback(OnMessageChanged)));
    static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as MarqueeControl)?.refresh();
    }
    #endregion TickerMessage

    #region ScrollSpeed
    public int ScrollSpeed
    {
      get { return (int)GetValue(ScrollSpeedProperty); }
      set { SetValue(ScrollSpeedProperty, value); }
    }
    public static readonly DependencyProperty ScrollSpeedProperty =
      DependencyProperty.Register(nameof(ScrollSpeed), typeof(int), typeof(MarqueeControl),
        new PropertyMetadata(10));
    #endregion ScrollSpeed

    Size MeasureText(Label tb)
    {
      var formattedText = new FormattedText(tb.Content.ToString(), CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight,
        new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
        tb.FontSize, Brushes.Black);
      return new Size(formattedText.Width, formattedText.Height);
    }

    void refresh()
    {
      _storyboard?.Stop();
      if (!this.IsVisible) return;
      if (string.IsNullOrEmpty(this.Message)) return;

      var canvasWidth = out_canvas.ActualWidth;
      if (canvasWidth <= 0 || out_canvas.ActualHeight <= 0)
        return;

      // scroll 설정 (메시지가 화면보다 긴 경우 전체를 회전할 시간 계산)
      var textWidth = this.MeasureText(this.Caption).Width;

      if (textWidth > canvasWidth)
      {
        int time = this.ScrollSpeed * (int)Math.Round(textWidth / canvasWidth + 0.5);

        var animation = new DoubleAnimation()
        {
          From = out_canvas.ActualWidth,
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
  }
}