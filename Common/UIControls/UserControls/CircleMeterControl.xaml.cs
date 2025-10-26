using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UIControls
{
  /// <summary>
  /// Interaction logic for CircleMeterControl.xaml
  /// </summary>
  public partial class CircleMeterControl : UserControl
  {
    public CircleMeterControl()
    {
      InitializeComponent();
    }
    #region DP
    public double Value
    {
      get { return (double)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }
    public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register("Value", typeof(double), typeof(CircleMeterControl),
         new FrameworkPropertyMetadata(60d, new PropertyChangedCallback(OnValueChanged)));
    static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as CircleMeterControl)?.ValueChanged((double)e.NewValue);
    }
    void ValueChanged(double value)
    {
      this.circleMeter.Value = value;
    }

    public double Minimum
    {
      get { return (double)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    public static readonly DependencyProperty MinimumProperty =
         DependencyProperty.Register("Minimum", typeof(double),
         typeof(CircleMeterControl), new FrameworkPropertyMetadata(0d, OnMinimumChanged));

    static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as CircleMeterControl)?.MinimumChanged((double)e.NewValue);
    }
    void MinimumChanged(double value)
    {
      this.circleMeter.Minimum = value;
    }

    public double Maximum
    {
      get { return (double)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    public static readonly DependencyProperty MaximumProperty =
         DependencyProperty.Register("Maximum", typeof(double),
         typeof(CircleMeterControl), new FrameworkPropertyMetadata(360d, OnMaximumChanged));

    static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as CircleMeterControl)?.MaximumChanged((double)e.NewValue);
    }
    void MaximumChanged(double value)
    {
      this.circleMeter.Maximum = value;
    }

    public string AreaName
    {
      get { return (string)GetValue(AreaNameProperty); }
      set { SetValue(AreaNameProperty, value); }
    }
    public static readonly DependencyProperty AreaNameProperty =
            DependencyProperty.Register("AreaName", typeof(string), typeof(CircleMeterControl),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAreaNameChanged)));
    static void OnAreaNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as CircleMeterControl)?.AreaNameChanged(e.NewValue?.ToString() ?? string.Empty);
    }
    void AreaNameChanged(string msg)
    {
      this.tbTitle.Text = msg;
    }

    #endregion DP
  }

  #region CircleMeter
  public class CircleMeter : Canvas
  {
    Path _back_ellipse;
    Path _fore_ellipse;
    Path _text;

    double _radius = 0;
    Point _center = new Point(0, 0);
    Point _startPoint = new Point(0, 0);
    double _stroke_thick = 25;

    public CircleMeter()
    {
      _back_ellipse = new Path()
      {
        StrokeThickness = _stroke_thick,
        Stroke = Brushes.Black
      };

      _fore_ellipse = new Path()
      {
        StrokeThickness = _stroke_thick,
        Stroke = GetColor()
      };

      _text = new Path()
      {
        StrokeThickness = 1,
        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00"))
      };
      SizeChanged += OnSizeChanged;
      Children.Add(_back_ellipse);
      Children.Add(_fore_ellipse);
      Children.Add(_text);
    }

    void draw()
    {
      //_back_ellipse.Data = new EllipseGeometry(_center, _radius, _radius);

      if (Math.Abs(Value * (360 / (Maximum - Minimum)) - 360) < 0.0001)
      {
        _fore_ellipse.Data = new EllipseGeometry(_center, _radius, _radius);
      }
      else
      {
        var pt = new RotateTransform(Value*(360/(Maximum - Minimum)), _center.X, _center.Y).Transform(_startPoint);
        bool large = !(Value*(360/(Maximum - Minimum)) <= 180);
        var seqments = new PathSegmentCollection() { new ArcSegment(pt, new Size(_radius, _radius), 0, large, SweepDirection.Clockwise, true) };
        var figure = new PathFigure(_startPoint, seqments, false);
        _fore_ellipse.Data = new PathGeometry() { Figures = new PathFigureCollection() { figure } };
      }

      var text = new FormattedText($"{Value}분",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("EHWA"),
                35, // 0.8 * _radius,
                Brushes.White);

      _text.Data = text.BuildGeometry(new Point(_center.X - text.Width / 2, _center.Y - text.Height / 2));
    }

    void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      var quadSize = Math.Min(e.NewSize.Width, e.NewSize.Height);
      _radius = quadSize / 2;
      _center = new Point(e.NewSize.Width / 2, e.NewSize.Height / 2);
      _startPoint = _center - new Vector(0, _radius);

      _back_ellipse.Data = new EllipseGeometry(_center, _radius, _radius);
      draw();
    }

    #region DP
    public double Value
    {
      get { return (double)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }
    public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register("Value", typeof(double),
         typeof(CircleMeter), new FrameworkPropertyMetadata(60d, OnValueChanged));

    public double Minimum
    {
      get { return (double)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    public static readonly DependencyProperty MinimumProperty =
         DependencyProperty.Register("Minimum", typeof(double),
         typeof(CircleMeter), new FrameworkPropertyMetadata(0d, OnValueChanged));
    public double Maximum
    {
      get { return (double)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    public static readonly DependencyProperty MaximumProperty =
         DependencyProperty.Register("Maximum", typeof(double),
         typeof(CircleMeter), new FrameworkPropertyMetadata(360d, OnValueChanged));

    static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as CircleMeter).draw();
    }
    #endregion DP

    LinearGradientBrush GetColor()
    {
      LinearGradientBrush myVerticalGradient = new LinearGradientBrush();
      myVerticalGradient.StartPoint = new Point(0, 0.5);
      myVerticalGradient.EndPoint = new Point(1, 0.5);
      myVerticalGradient.GradientStops.Add(
          new GradientStop(Colors.Yellow, 0.0));
      myVerticalGradient.GradientStops.Add(
          new GradientStop(Colors.Orange, 0.5));

      return myVerticalGradient;
    }
  }
  #endregion CircleMeter
}