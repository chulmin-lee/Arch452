using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace UIControls
{
  public partial class CarouselUniformGrid : Canvas
  {
    DispatcherTimer _delay_timer = new DispatcherTimer();
    public CarouselUniformGrid()
    {
      this.ClipToBounds = true;
      this.AutoStart = true;
      this.SizeChanged += ControlSizeChanged;
      _delay_timer = new DispatcherTimer();
      _delay_timer.Tick += DelayTimerExpired;
    }

    #region EventHandler

    public delegate void SelectionChangedEventHandler(FrameworkElement selectedElement);

    //public event SelectionChangedEventHandler SelectionChanged;

    void ControlSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this.Children.Count > 0) size_changed();
    }

    void AddMouseLeftButtonDownHandlers()
    {
      foreach (FrameworkElement element in this.Children)
      {
        element.MouseLeftButtonDown += ElementMouseLeftButtonDown;
        element.Unloaded += delegate { MouseLeftButtonDown -= ElementMouseLeftButtonDown; };
        element.Cursor = Cursors.Hand;
      }
      MouseWheel += CarouselControl_MouseWheel;
    }

    void CarouselControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
    }
    private void ElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
    }
    #endregion EventHandler

    ~CarouselUniformGrid()
    {
      _delay_timer.Tick -= DelayTimerExpired;
    }

    FrameworkElement GetChild(int index)
    {
      if ((this.Children.Count > 0) && (index < this.Children.Count))
      {
        var element = this.Children[index] as FrameworkElement;
        if (element == null)
        {
          throw new NotSupportedException("children are not Framework elements");
        }
        return element;
      }
      return null;
    }
    int GetItemIndex(FrameworkElement element)
    {
      // DataContext 철??
      return this.Children.IndexOf(element);
    }

    #region DP
    public bool AutoStart { get; set; }
    /// <summary>
    /// Animation 시간
    /// </summary>
    public double AnimationDuration { get; set; } = 500;
    /// <summary>
    /// 지연 시간
    /// </summary>
    public double DelayInterval { get; set; } = 1000 * 10;

    #region Items

    #region ItemsSource

    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
                          typeof(IEnumerable), typeof(CarouselUniformGrid),
                          new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));
    public IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CarouselUniformGrid)d).OnItemsSourceChanged(e);
    }
    protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        itemssource_changed(); // e.NewValue as IEnumerable);
        // CollectionChanged 인터페이스가 존재하는 경우 핸들러 연결
        var o = ItemsSource as INotifyCollectionChanged;
        if (o != null)
        {
          o.CollectionChanged += CarouselControl_CollectionChanged;
        }
      }
    }
    private void CarouselControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      //NotifyCollectionChangedAction action = e.Action;
      //int old_index = e.OldStartingIndex;
      //var old_items = e.OldItems;

      //int new_index = e.NewStartingIndex;
      //var new_items = e.NewItems;
      collection_changed();
    }

    #endregion ItemsSource

    #region SelectedItem
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem",
                        typeof(Object), typeof(CarouselUniformGrid),
                        new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));
    public object SelectedItem
    {
      get => GetValue(SelectedItemProperty);
      set { if (value != SelectedItem) SetValue(SelectedItemProperty, value); }
    }

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CarouselUniformGrid)d).OnSelectedItemChanged(e);
    }
    protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        for (int index = 0; index < this.Children.Count; index++)
        {
          var element = GetChild(index);
          if (element == null) continue;
          if (element.DataContext == e.NewValue)
          {
            //SelectElement(element);
            return;
          }
        }
      }
    }
    #endregion SelectedItem
    #endregion Items

    #region Style
    #region ContentItemTemplate
    // 좀 생각해보자
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static readonly DependencyProperty ContentItemTemplateProperty = DependencyProperty.Register("ContentItemTemplate",
                    typeof(ControlTemplate), typeof(CarouselUniformGrid),
                    new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentItemTemplateChanged)));
    public ControlTemplate ContentItemTemplate
    {
      get => (ControlTemplate)GetValue(ContentItemTemplateProperty);
      set => SetValue(ContentItemTemplateProperty, value);
    }
    private static void OnContentItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CarouselUniformGrid)d).OnContentItemTemplateChanged(e);
    }
    protected virtual void OnContentItemTemplateChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        ContentItemTemplate = (ControlTemplate)e.NewValue;
      }
    }
    #endregion ContentItemTemplate
    #endregion Style

    #region Layout

    #region Columns
    public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns",
                    typeof(int), typeof(CarouselUniformGrid),
                    new FrameworkPropertyMetadata(4, new PropertyChangedCallback(OnColumnsChanged)));
    public int Columns { get => (int)GetValue(ColumnsProperty); set => SetValue(ColumnsProperty, value); }
    static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CarouselUniformGrid)d).OnColumnsChanged(e);
    }
    protected virtual void OnColumnsChanged(DependencyPropertyChangedEventArgs e)
    {
      if (int.TryParse(e.NewValue?.ToString(), out int col))
      {
        //if (this.Columns != col)
        {
          Columns = col;
          this.layout_changed();
        }
      }
    }
    #endregion Columns

    #region Rows

    public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows",
                    typeof(int), typeof(CarouselUniformGrid),
                    new FrameworkPropertyMetadata(1, new PropertyChangedCallback(OnRowsChanged)));
    public int Rows { get => (int)GetValue(RowsProperty); set => SetValue(RowsProperty, value); }
    static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CarouselUniformGrid)d).OnRowsChanged(e);
    }
    protected virtual void OnRowsChanged(DependencyPropertyChangedEventArgs e)
    {
      if (int.TryParse(e.NewValue?.ToString(), out int row))
      {
        //if (this.Rows != row)
        {
          Rows = row;
          this.layout_changed();
        }
      }
    }
    #endregion Rows

    #endregion Layout
    #endregion DP
  }
}