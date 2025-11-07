using System.Globalization;
using System.Windows.Data;

//namespace EUMC.Client

//public class LargeOfficeRowsValuesConverter : IValueConverter
//{
//  public static LargeOfficeRowsValuesConverter Default { get; private set; } = new LargeOfficeRowsValuesConverter();
//  LargeOfficeRowsValuesConverter()
//  {
//  }

//  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//  {
//    int.TryParse(parameter?.ToString(), out int fixedRows);
//    int.TryParse(value?.ToString(), out int itemRows);

//    var list = new List<string>();

//    for (int f = 0; f < fixedRows; f++)
//    {
//      list.Add("1*");
//    }
//    list.Add($"{itemRows}*");
//    return string.Join(",", list);
//  }

//  public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//  {
//    return null;
//  }
//}