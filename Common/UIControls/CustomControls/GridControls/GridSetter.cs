using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace UIControls
{
  /// <summary>
  /// Grid Row/Column을 쉽게 설정하기 위한 helper
  /// Usage:
  /// - GridSetter.RowCount = "3"
  /// - GridSetter.RowValues = "100, 1*, 2*"
  /// </summary>

  public static class GridSetter
  {
    static void RowUpdate(Grid d)
    {
      var rows = GetRowCount(d);
      if (rows != 0)
      {
        d.RowDefinitions.Clear();
        GetRowValues(d).Parse(rows)
          .ForEach(x => d.RowDefinitions.Add(new RowDefinition { Height = x.GetGridLength() }));
      }
    }
    static void ColumnUpdate(Grid d)
    {
      var columns = GetColumnCount(d);
      if (columns != 0)
      {
        d.ColumnDefinitions.Clear();
        GetColumnValues(d).Parse(columns)
          .ForEach(x => d.ColumnDefinitions.Add(new ColumnDefinition { Width = x.GetGridLength() }));
      }
    }

    #region Row 설정
    static void OnRowUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is Grid o) { RowUpdate(o); }
    }
    public static readonly DependencyProperty RowCountProperty
    = DependencyProperty.RegisterAttached("RowCount", typeof(int), typeof(GridSetter),
      new PropertyMetadata(0, OnRowUpdated));

    public static int GetRowCount(DependencyObject o) => (int)o.GetValue(RowCountProperty);
    public static void SetRowCount(DependencyObject o, int v) => o.SetValue(RowCountProperty, v);

    public static readonly DependencyProperty RowValuesProperty
    = DependencyProperty.RegisterAttached("RowValues", typeof(string), typeof(GridSetter),
      new PropertyMetadata(null, OnRowUpdated));
    public static string GetRowValues(DependencyObject o) => (string)o.GetValue(RowValuesProperty);
    public static void SetRowValues(DependencyObject o, string v) => o.SetValue(RowValuesProperty, v);
    #endregion Row 설정

    #region Column 설정
    static void OnColumnUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is Grid o) { ColumnUpdate(o); }
    }
    public static readonly DependencyProperty ColumnCountProperty
    = DependencyProperty.RegisterAttached("ColumnCount", typeof(int), typeof(GridSetter),
        new PropertyMetadata(0, OnColumnUpdated));

    public static int GetColumnCount(DependencyObject o) => (int)o.GetValue(ColumnCountProperty);
    public static void SetColumnCount(DependencyObject o, int v) => o.SetValue(ColumnCountProperty, v);

    public static readonly DependencyProperty ColumnValuesProperty =
    DependencyProperty.RegisterAttached("ColumnValues", typeof(string), typeof(GridSetter),
      new PropertyMetadata(null,OnColumnUpdated));
    public static string GetColumnValues(DependencyObject o) => (string)o.GetValue(ColumnValuesProperty);
    public static void SetColumnValues(DependencyObject o, string v) => o.SetValue(ColumnValuesProperty, v);
    #endregion Column 설정

    #region Extension
    static Regex white_space = new Regex(@"\s+");
    /// <summary>
    /// grid height/width 비율 문자열 파싱
    /// </summary>
    public static List<GridLenValue> Parse(this string s, int count)
    {
      /*
      (1) "100,2*,3,4",
      (2) "2*,*"    ==> "2*,1*,...1*"
      (3) "2*,*,3*" ==> "2*,1*, ..., 3*"
      (4) "1,2" ==> { 1, 2, 2, 2,2,2}
      */

      //------------------------------
      // 문자열 => 목록 변환
      //------------------------------
      var values = new List<string>();
      #region ToList
      {
        if (!string.IsNullOrEmpty(s?.Trim()))
        {
          s = white_space.Replace(s, "");
          values = s.ToString().Split(',').ToList();
        }

        // "*" 이 1개만 있어야 한다
        int starCount = values.Where(x => x == "*").Count();
        if (starCount > 1)
        {
          bool exist = false;
          var temp = new List<string>();

          foreach (var p in values)
          {
            if (p != "*")
            {
              temp.Add(p);
            }
            else if (!exist)
            {
              exist = true;
              temp.Add(p);
            }
          }
          values = temp;
        }

        // 남는것 버리기
        values = values.Take(count).ToList();
      }
      #endregion ToList

      //------------------------------
      // 목록값 처리
      //------------------------------
      #region tunning
      {
        if (values.Count == 0)
        {
          // count 만큼 기본값 생성
          values = Enumerable.Repeat<string>("1*", count).ToList();
        }
        else if (values.Count == 1)
        {
          var c = values[0];
          if (c == "*")
          {
            c = "1*";
          }
          // count 만큼 값 반복
          values = Enumerable.Repeat<string>(c, count).ToList();
        }
        else if (values.Count != count)
        {
          var temp = new List<string>();
          int num = count - values.Count;
          int index = values.IndexOf("*");

          // "*" 은 남는 갯수만큼 1* 으로 변환
          if (index == 0)
          {
            temp.AddRange(Enumerable.Repeat("1*", num + 1).ToList());
            temp.AddRange(values.Skip(index + 1).Take(values.Count - 1));
          }
          else if (index > 0)
          {
            temp.AddRange(values.Take(index));
            temp.AddRange(Enumerable.Repeat("1*", num + 1).ToList());
            temp.AddRange(values.Skip(index + 1).Take(values.Count - index - 1));
          }
          else
          {
            temp.AddRange(values);
            temp.AddRange(Enumerable.Repeat(values.Last(), num).ToList());
          }
          values = temp;
        }
      }
      #endregion tunning

      //------------------------------
      // GridLenValue로 변환
      //------------------------------
      var grid_lengths = new List<GridLenValue>();
      #region GridLengValue로 변환
      {
        foreach (string p in values)
        {
          GridUnitType type = GridUnitType.Pixel;

          var d = p;

          if (d.EndsWith("*"))
          {
            type = GridUnitType.Star;
            d = p.Substring(0, p.Length - 1);
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
          grid_lengths.Add(new GridLenValue(type, len));
        }
      }
      #endregion GridLengValue로 변환

      return grid_lengths;
    }
    #endregion Extension
  }

  public class GridLenValue
  {
    public GridUnitType UnitType { get; private set; }

    double _value;
    public bool IsStar => UnitType == GridUnitType.Star;
    public bool IsAuto => UnitType == GridUnitType.Auto;
    public bool IsPixel => UnitType == GridUnitType.Pixel;
    public double Value => this.IsAuto ? 1.0 : _value;
    public bool IsDraw { get; private set; }
    public GridLenValue(GridUnitType type, double v, bool draw = true)
    {
      UnitType = type;
      _value = v;
      IsDraw = draw;
    }
    public double GetDesiredLength(double step)
    {
      switch (UnitType)
      {
        case GridUnitType.Star: return this.Value * step;
        case GridUnitType.Auto: return 1 * step;
        case GridUnitType.Pixel: return this.Value;
        default:
          return this.Value;
      }
    }
    public GridLength GetGridLength()
    {
      switch (UnitType)
      {
        case GridUnitType.Star: return new GridLength(this.Value, UnitType);
        case GridUnitType.Auto: return new GridLength(1.0, GridUnitType.Auto);
        case GridUnitType.Pixel: return new GridLength(this.Value);
        default:
          return Auto;
      }
    }

    static readonly GridLength s_auto = new GridLength(1.0, GridUnitType.Auto);
    public static GridLength Auto => s_auto;
  }
  public class StarGridLength : GridLenValue
  {
    public StarGridLength(double v, bool draw = true) : base(GridUnitType.Star, v, draw) { }
  }
  public class PixelGridLength : GridLenValue
  {
    public PixelGridLength(double v, bool draw = true) : base(GridUnitType.Pixel, v, draw) { }
  }
  public class AutoGridLength : GridLenValue
  {
    public AutoGridLength(bool draw = true) : base(GridUnitType.Auto, 0, draw) { }
  }
}