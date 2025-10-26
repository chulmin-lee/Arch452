using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace UIControls
{
  public class BorderClipConverter : BaseOnewayMultiConverter
  {
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length == 3 && values[0] is double && values[1] is double && values[2] is CornerRadius)
      {
        var width = (double)values[0];
        var height = (double)values[1];

        if (width < Double.Epsilon || height < Double.Epsilon)
        {
          return Geometry.Empty;
        }

        var radius = (CornerRadius)values[2];

        var clip = new RectangleGeometry(new Rect(0, 0, width, height), radius.TopLeft, radius.TopLeft);
        clip.Freeze();

        return clip;
      }

      return DependencyProperty.UnsetValue;
    }
  }
}
