using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections;
using UIControls;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Collections.Generic;

namespace UIControls
{
  /// <summary>
  /// Interaction logic for ArcMeterControl.xaml
  /// </summary>
  public partial class ArcMeterControl : UserControl
  {
    public ArcMeterControl()
    {
      InitializeComponent();
      //this.DataContext = VM;
    }

    void GenerateItems()
    {
      if (this.ItemsSource == null)
      {
        //VM.Update(new List<ErAreaCongestion>());
        return;
      }

      List<ErAreaCongestion> list = new List<ErAreaCongestion>();

      int idx = 0;
      foreach (var item in this.ItemsSource)
      {
        var p = item as ErAreaCongestion;
        if (p != null)
        {
          list.Add(p);
          if (++idx == this.Rows)
            break;
        }
      }
      VM.Update(list);
    }
    void TotalCongestionChange(ErTotalConstion o)
    {
      VM.Update(o);
    }

    #region DP

    #region VM
    public ArcMeterModel VM
    {
      get { return (ArcMeterModel)GetValue(VMProperty); }
      set { SetValue(VMProperty, value); }
    }
    public static readonly DependencyProperty VMProperty =
            DependencyProperty.Register("VM", typeof(ArcMeterModel), typeof(ArcMeterControl),
            new FrameworkPropertyMetadata(new ArcMeterModel()));
    #endregion VM

    #region ItemsSource
    public IEnumerable ItemsSource
    {
      get { return (IEnumerable)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
    DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ArcMeterControl), new PropertyMetadata(null, OnItemsSourcePropertyChanged));

    private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is ArcMeterControl o)
      {
        o.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
      }
    }

    private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
      // ObservableCollection인 경우 handler 연결
      if (oldValue is INotifyCollectionChanged d)
      {
        d.CollectionChanged -= CollectionChanged;
      }
      if (newValue is INotifyCollectionChanged n)
      {
        n.CollectionChanged += CollectionChanged;
      }
      this.GenerateItems();
    }

    void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.GenerateItems();
    }
    #endregion ItemsSource

    public ErTotalConstion TotalCongestion
    {
      get { return (ErTotalConstion)GetValue(TotalCongestionProperty); }
      set { SetValue(TotalCongestionProperty, value); }
    }
    public static readonly DependencyProperty TotalCongestionProperty =
            DependencyProperty.Register("TotalCongestion", typeof(ErTotalConstion), typeof(ArcMeterControl),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTotalCongestionChanged)));
    static void OnTotalCongestionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as ArcMeterControl)?.TotalCongestionChange(e.NewValue as ErTotalConstion);
    }

    public int Rows
    {
      get { return (int)GetValue(RowsProperty); }
      set { SetValue(RowsProperty, value); }
    }
    public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(ArcMeterControl),
            new FrameworkPropertyMetadata(7));

    #endregion DP
  }

  #region DataModel

  public class ErAreaCongestion
  {
    public int AreaCode { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public double Percent { get; set; }
  }

  public class ErTotalConstion
  {
    public string Name { get; set; } = "응급실 혼잡도";
    public int TotalInBed { get; set; }
    public int Percent { get; set; }
  }

  public class ArcMeterModel : ViewModelBase
  {
    public void Update(ErTotalConstion o)
    {
      if (o == null)
        return;

      this.TotalCongestionName = o.Name;
      this.TotalInBed = o.TotalInBed;
      this.TotalPercentage = o.Percent;
    }

    string _totalCongestionName = string.Empty;
    public string TotalCongestionName { get { return _totalCongestionName; } set { Set(ref _totalCongestionName, value); } }

    int _totalInBed;
    public int TotalInBed { get { return _totalInBed; } set { Set(ref _totalInBed, value); } }

    int _percentage;
    public int TotalPercentage { get { return _percentage; } set { if (Set(ref _percentage, value)) { this.PercentChanged(value); } } }

    private void PercentChanged(int percent)
    {
      if (percent <= 25)
      {
        this.Arc45 = true;
        this.Arc90 = this.Arc135 = this.Arc180 = false;
      }
      else if (25 < percent && percent <= 50)
      {
        this.Arc45 = this.Arc90 = true;
        this.Arc135 = this.Arc180 = false;
      }
      else if (50 < percent && percent <= 75)
      {
        this.Arc45 = this.Arc90 = this.Arc135 = true;
        this.Arc180 = false;
      }
      else
      {
        this.Arc45 = this.Arc90 = this.Arc135 = this.Arc180 = true;
      }
    }

    bool _arc45;
    public bool Arc45 { get { return _arc45; } set { Set(ref _arc45, value); } }
    bool _arc90;
    public bool Arc90 { get { return _arc90; } set { Set(ref _arc90, value); } }
    bool _arc135;
    public bool Arc135 { get { return _arc135; } set { Set(ref _arc135, value); } }
    bool _arc180;
    public bool Arc180 { get { return _arc180; } set { Set(ref _arc180, value); } }

    #region Area Congestion
    public void Update(List<ErAreaCongestion> list)
    {
      this.Items.Clear();
      foreach (var item in list)
      {
        if (item.Percent > 1) item.Percent = 1d;
        this.Items.Add(item);
      }
    }
    public ObservableCollection<ErAreaCongestion> Items { get; set; } = new ObservableCollection<ErAreaCongestion>();
    #endregion Area Congestion
  }
  #endregion DataModel

  #region ArcMeter
  public class ArcMeter : Shape
  {
    readonly double RotationAngle = 270;
    double _outer_radius;
    double _inner_radius;
    double _centre_x;
    double _centre_y;

    public ArcMeter()
    {
      this.SizeChanged += ArcShape_SizeChanged;
    }
    void ArcShape_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      var w = e.NewSize.Width;
      var h = e.NewSize.Height;

      if (w > 0 && h > 0)
      {
        _centre_x = w / 2;
        _centre_y = w / 2;
        _outer_radius = w / 2;
        _inner_radius = w / 5;
        this.InvalidateVisual();
      }
    }

    #region dependency properties
    public double WedgeAngle
    {
      get { return (double)GetValue(WedgeAngleProperty); }
      set { SetValue(WedgeAngleProperty, value); }
    }
    public static readonly DependencyProperty WedgeAngleProperty =
            DependencyProperty.Register("WedgeAngle", typeof(double), typeof(ArcMeter),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
              new PropertyChangedCallback(OnWedgeAngleChanged)
              ));
    static void OnWedgeAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as ArcMeter)?.InvalidateVisual();
    }
    public double Percentage
    {
      get { return (double)GetValue(PercentageProperty); }
      set { SetValue(PercentageProperty, value); }
    }
    public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(ArcMeter),
            new FrameworkPropertyMetadata(50.0, new PropertyChangedCallback(OnPercentageChanged)));
    static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as ArcMeter)?.PercentageChanged((double)e.NewValue);
    }
    void PercentageChanged(double d)
    {
      this.WedgeAngle = (d / 100) * 180;
    }
    #endregion dependency properties

    protected override Geometry DefiningGeometry
    {
      get
      {
        var geo = new StreamGeometry() {FillRule = FillRule.EvenOdd };
        using (var context = geo.Open())
        {
          draw(context);
        }
        geo.Freeze();
        return geo;
      }
    }
    void draw(StreamGeometryContext context)
    {
      Point inArcStart = coordinate(RotationAngle, _inner_radius);
      Point inArcEnd = coordinate(RotationAngle + WedgeAngle, _inner_radius);
      Point outArcStart = coordinate(RotationAngle, _outer_radius);
      Point outArcEnd = coordinate(RotationAngle + WedgeAngle, _outer_radius);

      bool largeArc = WedgeAngle > 180.0;
      Size outerArcSize = new Size(_outer_radius, _outer_radius);
      Size innerArcSize = new Size(_inner_radius, _inner_radius);

      context.BeginFigure(inArcStart, true, true);
      context.LineTo(outArcStart, true, true);
      context.ArcTo(outArcEnd, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, true);
      context.LineTo(inArcEnd, true, true);
      context.ArcTo(inArcStart, innerArcSize, 0, largeArc, SweepDirection.Counterclockwise, true, true);
    }
    Point coordinate(double angle, double radius)
    {
      double rad = (Math.PI / 180.0) * (angle - 90);
      var pt = new Point(radius * Math.Cos(rad), radius * Math.Sin(rad));
      pt.Offset(_centre_x, _centre_y);
      return pt;
    }
  }
  #endregion ArcMeter

  public class WidthConverter : IMultiValueConverter
  {
    public static WidthConverter Default { get; private set; } = new WidthConverter();
    WidthConverter()
    {
    }
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length != 2)
        throw new ArgumentException("The number of values must be 2");

      double width = (double)values[0];
      double percent = (double)values[1];

      return width * percent;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}