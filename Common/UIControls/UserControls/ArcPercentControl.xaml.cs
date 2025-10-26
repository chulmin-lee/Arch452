using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace UIControls
{
  /// <summary>
  /// Interaction logic for ArcMeter.xaml
  /// </summary>
  public partial class ArcPercentControl : UserControl, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    public ArcPercentControl()
    {
      InitializeComponent();
    }

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public int Percent
    {
      get { return (int)GetValue(PercentProperty); }
      set { SetValue(PercentProperty, value); }
    }
    public static readonly DependencyProperty PercentProperty =
            DependencyProperty.Register(nameof(Percent), typeof(int), typeof(ArcPercentControl),
            new FrameworkPropertyMetadata(0,
              FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnColumnUpdated));

    static void OnColumnUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as ArcPercentControl)?.ColumnUpdate();
    }

    private void ColumnUpdate()
    {
      var percent = this.Percent;

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
    public bool Arc45 { get { return _arc45; } set { if (_arc45 != value) { _arc45 = value; this.OnPropertyChanged(nameof(Arc45)); } } }
    bool _arc90;
    public bool Arc90 { get { return _arc90; } set { if (_arc90 != value) { _arc90 = value; this.OnPropertyChanged(nameof(Arc90)); } } }
    bool _arc135;
    public bool Arc135 { get { return _arc135; } set { if (_arc135 != value) { _arc135 = value; this.OnPropertyChanged(nameof(Arc135)); } } }
    bool _arc180;
    public bool Arc180 { get { return _arc180; } set { if (_arc180 != value) { _arc180 = value; this.OnPropertyChanged(nameof(Arc180)); } } }
  }
}