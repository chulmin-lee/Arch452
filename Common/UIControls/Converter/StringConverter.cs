using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace UIControls
{
  /// <summary>
  /// 문자열 한글자씩 띄워서 보여준다
  /// </summary>
  public class StringExpandConverter : IValueConverter
  {
    public static StringExpandConverter Default { get; private set; } = new StringExpandConverter();
    StringExpandConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var s = value.ToString();
      if (!string.IsNullOrEmpty(s))
      {
        var list = s.ToList();
        return string.Join(" ", list);
      }
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}