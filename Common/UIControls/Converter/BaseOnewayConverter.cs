using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace UIControls
{
  public abstract class BaseOnewayConverter : MarkupExtension, IValueConverter
  {
    public BaseOnewayConverter() { }
    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new NotImplementedException();
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }

  /*
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var width = (double)values[0];
      var height = (double)values[1];
      var diagonal = Math.Sqrt(width * width + height * height);
      var horzmargin = (diagonal - width) / 2;
      var vertmargin = (diagonal - height) / 2;
      return new Thickness(horzmargin, vertmargin, horzmargin, vertmargin);
    }
  */
  public abstract class BaseOnewayMultiConverter : MarkupExtension, IMultiValueConverter
  {
    public BaseOnewayMultiConverter() { }
    public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);
    public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }
}