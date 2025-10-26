using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UIControls
{
  /// <summary>
  /// Reverse True/False to Visibility Converter
  /// </summary>
  public class ReverseBooleanToVisisibilityConverter : IValueConverter
  {
    public static ReverseBooleanToVisisibilityConverter Default { get; private set; } = new ReverseBooleanToVisisibilityConverter();
    ReverseBooleanToVisisibilityConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var visible = (bool)value;
      return visible ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  /// <summary>
  /// 문자열이 있을때만 보이기
  /// </summary>
  public class StringToVisibilityConverter : IValueConverter
  {
    public static StringToVisibilityConverter Default { get; private set; } = new StringToVisibilityConverter();
    StringToVisibilityConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool exist = !string.IsNullOrWhiteSpace(value?.ToString());

      return exist ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}