using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UIControls
{
  /// <summary>
  /// Grid Cell 을 line으로 그린다
  /// 쉽게 row/col을 설정
  /// 개선할 사항
  /// - 선 그리지 않기 ==> GridSetter 를 사용하면 된다
  /// </summary>
  public partial class TableGrid : Grid
  {
    public TableGrid()
    {
      this.RowBackgroundBrushes = new List<BrushValue>();
      this.ColumnBackgroundBrushes = new List<BrushValue>();
      this.RowDataTemplates = new List<DataTemplate>();
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      Size size = base.ArrangeOverride(arrangeSize);

      if (this.Children.Count == 0) return arrangeSize;
      if (this.BorderThickness == 0) return size;

      double thick = this.BorderThickness;
      double doubleThick = thick * 2D;

      foreach (UIElement child in this.InternalChildren)
      {
        this.GetChildBounds(child, out double left, out double top, out double width, out double height);

        left += thick;
        left -= left % 0.5;

        top += thick;
        top -= top % 0.5;

        width -= doubleThick;
        width = Math.Max(width, 0);
        width -= width % 0.5;

        height -= doubleThick;
        height = Math.Max(height, 0);
        height -= height % 0.5;

        child.Arrange(new Rect(left, top, width, height));
      }

      if (this.BorderBrush != null)
      {
        this.InvalidateVisual();
      }

      return size;
    }

    protected override void OnRender(DrawingContext dc)
    {
      base.OnRender(dc);
      this.DrawBorder(dc);
    }

    void RowUpdate()
    {
      if (this.RowCount != 0)
      {
        this.RowDefinitions.Clear();
        this.RowValues.Parse(this.RowCount)
          .ForEach(x => this.RowDefinitions.Add(new RowDefinition { Height = x.GetGridLength() }));
        this.GenerateItems();
        this.InvalidateVisual();
      }
    }

    void ColumnUpdate()
    {
      if (this.ColumnCount != 0)
      {
        this.ColumnDefinitions.Clear();
        this.ColumnValues.Parse(this.ColumnCount)
          .ForEach(x => this.ColumnDefinitions.Add(new ColumnDefinition { Width = x.GetGridLength() }));
        this.GenerateItems();
        this.InvalidateVisual();
      }
    }

    void GenerateItems()
    {
      if (this.RowCount != 0 && this.ColumnCount != 0 && this.RowDataTemplates.Count > 0 && this.ItemsSource != null && this.Children.Count == 0)
      {
        int row = 0;
        foreach (var item in this.ItemsSource)
        {
          int col = 0;
          foreach (var template in this.RowDataTemplates)
          {
            var o = new ContentControl()
            {
              Content = item,
              ContentTemplate = template,
            };
            Grid.SetRow(o, row);
            Grid.SetColumn(o, col++);
            this.Children.Add(o);
          }
          if (++row == this.RowCount)
            break;
        }
        this.InvalidateMeasure();
        this.InvalidateVisual();
      }
    }

    #region Row Setting

    static void OnRowUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is TableGrid o) { o.RowUpdate(); }
    }

    public static readonly DependencyProperty RowCountProperty = DependencyProperty.Register(
    nameof(RowCount), typeof(int), typeof(TableGrid), new FrameworkPropertyMetadata(0,
      FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnRowUpdated));

    public int RowCount
    {
      get { return (int)GetValue(RowCountProperty); }
      set { SetValue(RowCountProperty, value); }
    }

    public static readonly DependencyProperty RowValuesProperty = DependencyProperty.Register(
    nameof(RowValues), typeof(string), typeof(TableGrid), new PropertyMetadata(null, OnRowUpdated));
    public string RowValues
    {
      get { return (string)GetValue(RowValuesProperty); }
      set { SetValue(RowValuesProperty, value); }
    }
    #endregion Row Setting

    #region Column Setting

    static void OnColumnUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as TableGrid)?.ColumnUpdate();
    }
    public static readonly DependencyProperty ColumnCountProperty =
    DependencyProperty.Register(nameof(ColumnCount), typeof(int), typeof(TableGrid), new FrameworkPropertyMetadata(0,
      FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnColumnUpdated));
    public int ColumnCount
    {
      get { return (int)GetValue(ColumnCountProperty); }
      set { SetValue(ColumnCountProperty, value); }
    }

    public static readonly DependencyProperty ColumnValuesProperty = DependencyProperty.Register(
    nameof(ColumnValues), typeof(string), typeof(TableGrid), new PropertyMetadata(null, OnColumnUpdated));
    public string ColumnValues
    {
      get { return (string)GetValue(ColumnValuesProperty); }
      set { SetValue(ColumnValuesProperty, value); }
    }
    #endregion Column Setting

    static void OnInvalidate(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
      if (d is UIElement element)
      {
        element.InvalidateVisual();
      }
    }

    #region Border Setting

    public static readonly DependencyProperty DrawOutterBorderProperty =
    DependencyProperty.Register(nameof(DrawOutterBorder), typeof(bool), typeof(TableGrid), new FrameworkPropertyMetadata(true,
      FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnInvalidate));
    public bool DrawOutterBorder
    {
      get { return (bool)GetValue(DrawOutterBorderProperty); }
      set { SetValue(DrawOutterBorderProperty, value); }
    }

    public static readonly DependencyProperty DrawColumnLineProperty =
    DependencyProperty.Register(nameof(DrawColumnLine), typeof(bool), typeof(TableGrid), new FrameworkPropertyMetadata(true,
      FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnInvalidate));
    public bool DrawColumnLine
    {
      get { return (bool)GetValue(DrawColumnLineProperty); }
      set { SetValue(DrawColumnLineProperty, value); }
    }
    public static readonly DependencyProperty DrawRowLineProperty =
    DependencyProperty.Register(nameof(DrawRowLine), typeof(bool), typeof(TableGrid), new FrameworkPropertyMetadata(true,
      FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnInvalidate));
    public bool DrawRowLine
    {
      get { return (bool)GetValue(DrawRowLineProperty); }
      set { SetValue(DrawRowLineProperty, value); }
    }
    public static readonly DependencyProperty ColumnLineRatioProperty =
    DependencyProperty.Register(nameof(ColumnLineRatio), typeof(double), typeof(TableGrid), new FrameworkPropertyMetadata(1.0,
      FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnInvalidate));
    public double ColumnLineRatio
    {
      get { return (double)GetValue(ColumnLineRatioProperty); }
      set { SetValue(ColumnLineRatioProperty, value); }
    }
    public static readonly DependencyProperty RowLineRatioProperty =
    DependencyProperty.Register(nameof(RowLineRatio), typeof(double), typeof(TableGrid), new FrameworkPropertyMetadata(1.0,
      FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnInvalidate));
    public double RowLineRatio
    {
      get { return (double)GetValue(RowLineRatioProperty); }
      set { SetValue(RowLineRatioProperty, value); }
    }

    public static readonly DependencyProperty BorderBrushProperty
    = DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(TableGrid), new FrameworkPropertyMetadata(Brushes.Transparent,
        FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnInvalidate));
    public Brush BorderBrush
    {
      get => (Brush)this.GetValue(BorderBrushProperty);
      set => this.SetValue(BorderBrushProperty, value);
    }

    #endregion Border Setting

    #region BorderThickness
    public static readonly DependencyProperty BorderThicknessProperty
    = DependencyProperty.Register(nameof(BorderThickness), typeof(double), typeof(TableGrid), new FrameworkPropertyMetadata(0D,
      FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnInvalidate, CoerceBorderThickness));
    private static object CoerceBorderThickness(DependencyObject d, object baseValue)
    {
      if (baseValue is double value)
      {
        return value < 0D || double.IsNaN(value) || double.IsInfinity(value) ? 0D : value;
      }
      return 0D;
    }
    public double BorderThickness
    {
      get => (double)this.GetValue(BorderThicknessProperty);
      set => this.SetValue(BorderThicknessProperty, value);
    }

    #endregion BorderThickness

    #region Item Background

    public static readonly DependencyPropertyKey  _rowBackgroundBrushesPropertyKey
    = DependencyProperty.RegisterReadOnly(nameof(RowBackgroundBrushes), typeof(List<BrushValue>), typeof(TableGrid), new PropertyMetadata());
    public List<BrushValue> RowBackgroundBrushes
    {
      get => (List<BrushValue>)this.GetValue(_rowBackgroundBrushesPropertyKey.DependencyProperty);
      set => this.SetValue(_rowBackgroundBrushesPropertyKey, value);
    }
    public static readonly DependencyPropertyKey _columnBackgroundBrushesPropertyKey
    = DependencyProperty.RegisterReadOnly(nameof(ColumnBackgroundBrushes), typeof(List<BrushValue>), typeof(TableGrid), new PropertyMetadata());

    public List<BrushValue> ColumnBackgroundBrushes
    {
      get => (List<BrushValue>)this.GetValue(_columnBackgroundBrushesPropertyKey.DependencyProperty);
      set => this.SetValue(_columnBackgroundBrushesPropertyKey, value);
    }

    public static readonly DependencyPropertyKey  _rowDataTemplatesPropertyKey
    = DependencyProperty.RegisterReadOnly(nameof(RowDataTemplates), typeof(List<DataTemplate>), typeof(TableGrid), new PropertyMetadata());
    public List<DataTemplate> RowDataTemplates
    {
      get => (List<DataTemplate>)this.GetValue(_rowDataTemplatesPropertyKey.DependencyProperty);
      set => this.SetValue(_rowDataTemplatesPropertyKey, value);
    }

    #endregion Item Background

    #region ItemsSource
    public IEnumerable ItemsSource
    {
      get { return (IEnumerable)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
    DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(TableGrid), new PropertyMetadata(null, OnItemsSourcePropertyChanged));

    private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is TableGrid o)
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

      this.Children.Clear();
      this.GenerateItems();
    }

    void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.Children.Clear();
      this.GenerateItems();
    }
    #endregion ItemsSource

    private void GetChildBounds(UIElement child, out double left, out double top, out double width, out double height)
    {
      this.GetChildLayout(child, out int row, out int rowSpan, out int column, out int columnSpan);

      var columns = this.ColumnDefinitions;
      var rows = this.RowDefinitions;

      left = columns[column].Offset;
      top = rows[row].Offset;

      ColumnDefinition right = columns[column + columnSpan - 1];
      width = right.Offset + right.ActualWidth - left;
      width = Math.Max(width, 0);

      RowDefinition bottom = rows[row + rowSpan - 1];
      height = bottom.Offset + bottom.ActualHeight - top;
      height = Math.Max(height, 0);
    }

    private void GetChildLayout(UIElement o, out int r, out int rs, out int c, out int cs)
    {
      int rowCount = this.RowDefinitions.Count;

      r = Math.Min((int)o.GetValue(Grid.RowProperty), rowCount - 1);
      rs = (int)o.GetValue(Grid.RowSpanProperty);
      if (r + rs > rowCount)
      {
        rs = rowCount - r;
      }

      int columnCount = this.ColumnDefinitions.Count;
      c = Math.Min((int)o.GetValue(Grid.ColumnProperty), columnCount - 1);
      cs = (int)o.GetValue(Grid.ColumnSpanProperty);
      if (c + cs > columnCount)
      {
        cs = columnCount - c;
      }
    }
  }

  public class BrushValue
  {
    public SolidColorBrush Color { get; set; } = Brushes.Transparent;
    public int Repeat { get; set; } = 1;  // 반복 횟수
  }
}