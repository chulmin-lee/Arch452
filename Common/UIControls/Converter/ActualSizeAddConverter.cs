using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace UIControls
{
  /// <summary>
  /// ActualSize + parameter
  /// - 대상의 크기에 맞춰서 크기 조절이 필요할때
  /// 사용예: Width="{Binding ElementName=TooltipTextBlock, Path=ActualWidth, Converter={tb:ActualSizeAddConverter}, ConverterParameter=30}"
  /// </summary>
  public class ActualSizeAddConverter : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double size = 0;
      double.TryParse(value?.ToString(), out size);
      int margin = 0;
      int.TryParse(parameter?.ToString(), out margin);
      return size + margin;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new NotImplementedException();
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }
}
