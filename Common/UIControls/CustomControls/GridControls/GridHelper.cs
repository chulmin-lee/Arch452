using System.Windows;
using System.Windows.Controls;

namespace UIControls
{
  /// <summary>
  /// Grid Row/Column을 쉽게 설정하기 위한 helper
  /// 주의: row/column height/width는 모두 같은 값을 가진다.
  /// Usage:
  /// - GridHelper.RowCount = "3"
  /// - GridHelper.RowHeight = "auto"
  /// </summary>
  public static class GridHelper
  {
    static void RowUpdate(Grid d)
    {
      var rows = GetRowCount(d);
      var height = GetRowHeight(d);

      if (rows != 0 && !string.IsNullOrEmpty(height))
      {
        d.RowDefinitions.Clear();
        var length = GetGridLength(height);
        for (int i = 0; i < rows; i++)
        {
          d.RowDefinitions.Add(new RowDefinition { Height = length });
        }
      }
    }
    static void ColumnUpdate(Grid d)
    {
      var columns = GetColumnCount(d);
      var width = GetColumnWidth(d);
      if (columns != 0 && !string.IsNullOrEmpty(width))
      {
        d.ColumnDefinitions.Clear();

        var length = GetGridLength(width);
        for (int i = 0; i < columns; i++)
        {
          d.ColumnDefinitions.Add(new ColumnDefinition { Width = length });
        }
      }
    }
    static GridLength GetGridLength(string s)
    {
      var d = s;
      GridUnitType type = GridUnitType.Pixel;
      if (s.EndsWith("*"))
      {
        type = GridUnitType.Star;
        d = s.Substring(0, s.Length - 1);
      }
      else if (d.ToLower() == "auto")
      {
        type = GridUnitType.Auto;
        d = "1";
      }
      if (!double.TryParse(d, out double len))
      {
        len = 1;
      }
      return new GridLength(len, type);
    }

    #region Row 설정
    static void OnRowUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is Grid o) { RowUpdate(o); }
    }
    public static readonly DependencyProperty RowCountProperty
    = DependencyProperty.RegisterAttached("RowCount", typeof(int), typeof(GridHelper),
      new PropertyMetadata(0, OnRowUpdated));

    public static int GetRowCount(DependencyObject o) => (int)o.GetValue(RowCountProperty);
    public static void SetRowCount(DependencyObject o, int v) => o.SetValue(RowCountProperty, v);

    public static readonly DependencyProperty RowHeightProperty
    = DependencyProperty.RegisterAttached("RowHeight", typeof(string), typeof(GridHelper),
      new PropertyMetadata("auto", OnRowUpdated));

    public static string GetRowHeight(DependencyObject o) => (string)o.GetValue(RowHeightProperty);
    public static void SetRowHeight(DependencyObject o, string v) => o.SetValue(RowHeightProperty, v);
    #endregion Row 설정

    #region Column 설정
    static void OnColumnUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is Grid o) { ColumnUpdate(o); }
    }
    public static readonly DependencyProperty ColumnCountProperty
    = DependencyProperty.RegisterAttached("ColumnCount", typeof(int), typeof(GridHelper),
        new PropertyMetadata(0, OnColumnUpdated));

    public static int GetColumnCount(DependencyObject o) => (int)o.GetValue(ColumnCountProperty);
    public static void SetColumnCount(DependencyObject o, int v) => o.SetValue(ColumnCountProperty, v);

    public static readonly DependencyProperty ColumnWidthProperty =
    DependencyProperty.RegisterAttached("ColumnWidth", typeof(string), typeof(GridHelper),
      new PropertyMetadata("auto",OnColumnUpdated));
    public static string GetColumnWidth(DependencyObject o) => (string)o.GetValue(ColumnWidthProperty);
    public static void SetColumnWidth(DependencyObject o, string v) => o.SetValue(ColumnWidthProperty, v);
    #endregion Column 설정
  }
}