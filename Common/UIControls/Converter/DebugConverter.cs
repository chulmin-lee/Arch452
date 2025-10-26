using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace UIControls
{
  /// <summary>
  /// 동작 테스트용
  /// <DataTrigger Binding="{Binding MenuSelected, Converter={ui:DebugConverter}}" Value="True">
  /// </summary>
  public class DebugConverter : BaseOnewayConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Debug.WriteLine($"DebugConverter.Convert({value?.ToString()})");
      return value;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Debug.WriteLine($"DebugConverter.ConvertBack({value?.ToString()})");
      return value;
    }
  }

  /// <summary>
  /// 값이 0 일때 빈칸으로 변환
  /// </summary>
  public class ZeroToEmptyStringConverter : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (double.TryParse(value.ToString(), out double v))
      {
        return v != 0 ? v.ToString() : string.Empty;
      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return 0;
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }
}