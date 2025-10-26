using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EUMC.Client
{
  public class OperationStateCodeToBrushConverter : IValueConverter
  {
    public static OperationStateCodeToBrushConverter Default { get; private set; } = new OperationStateCodeToBrushConverter();
    OperationStateCodeToBrushConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string text = (string)value;
      switch (text)
      {
        case "1": return Brushes.Red;
        case "2": return Brushes.Green;
        case "3": return Brushes.Blue;
        case "4": return Brushes.Black;
        default:
          return Brushes.Black;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}