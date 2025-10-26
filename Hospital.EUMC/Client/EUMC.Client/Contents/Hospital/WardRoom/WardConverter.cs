using Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using UIControls;

namespace EUMC.Client
{
  /// <summary>
  /// WardRoom 전체 높이 계산
  /// - 타이틀 높이
  /// - header 높이
  /// - row 높이
  /// - row count
  /// </summary>
  public class WardRoomHeightConverter : BaseOnewayMultiConverter
  {
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length == 4)
      {
        int title = (int)values[0];
        int header = (int)values[1];
        int row = (int)values[2];
        int count = (int)values[3];
        int margin = (int)values[4];

        double height = title + header + row * count + margin*2;

        //LOG.dc($"capcity: {count} : height = {height}");

        return height;
      }
      return DependencyProperty.UnsetValue;
    }
  }

  /// <summary>
  /// Capacity에 따른 Grid row 높이
  /// </summary>
  public class WardRowHeightConverter : IValueConverter
  {
    public static WardRowHeightConverter Default { get; private set; } = new WardRowHeightConverter();
    WardRowHeightConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var capacity = (int)value;
      var height = (int)parameter;
      return capacity * height;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}