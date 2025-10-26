using EUMC.ClientServices;
using System;
using System.Globalization;
using System.Windows;
using UIControls;

namespace EUMC.Client
{
  public class TitleBarVisibleConverter : BaseOnewayConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var binding = (TitlebarStyle)value;
      var style = (TitlebarStyle)parameter;
      return binding == style ? Visibility.Visible : Visibility.Collapsed;
    }
  }

  public class BottomVisibleConverter : BaseOnewayConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var binding = (BottomStyle)value;
      var style = (BottomStyle)parameter;
      return binding == style ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}