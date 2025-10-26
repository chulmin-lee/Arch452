using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace UIControls
{
  /// <summary>
  /// string 내용이 있으면 true, 아니면 false
  /// </summary>
  public class StringToBooleanConverter : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return !string.IsNullOrEmpty(value?.ToString());
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new NotImplementedException();
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }


  /// <summary>
  /// ToolTip이 보일 조건 설정
  /// - MouseOver, No Focused, ToolTip exist
  /// </summary>
  public class ToolTipVisibilityConverter : MarkupExtension, IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == DependencyProperty.UnsetValue)
      {
        Debug.WriteLine("values is unset");
        return Visibility.Collapsed;
      }

      // check value
      for (int i = 0; i < values.Length; i++)
      {
        var p = values[i];
        if (p == DependencyProperty.UnsetValue)
        {
          Debug.WriteLine($"values[{i}] is unset");
          return Visibility.Collapsed;
        }
      }

      bool mouseOver = (bool)values[0];
      bool focus = (bool)values[1];
      bool tooltipExist = !string.IsNullOrEmpty(values[2]?.ToString());

      //return (mouseOver && !focus && tooltipExist) ? Visibility.Visible : Visibility.Collapsed;
      return (mouseOver && !focus && tooltipExist);
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }
  /// <summary>
  /// StringTextBox에서 Watermark가 보일 조건
  /// - Focused : false
  /// - Text : null
  /// - Watermark : Exist
  /// </summary>
  public class WatermarkVisibilityConverter : MarkupExtension, IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == DependencyProperty.UnsetValue)
      {
        Debug.WriteLine("values is unset");
        return Visibility.Collapsed;
      }

      // check value
      for (int i = 0; i < values.Length; i++)
      {
        var p = values[i];
        if (p == DependencyProperty.UnsetValue)
        {
          Debug.WriteLine($"values[{i}] is unset");
          return false;
        }
      }

      bool focused = (bool)values[0];
      bool textExist = !string.IsNullOrEmpty(values[1]?.ToString());
      bool watermarkExist = !string.IsNullOrEmpty(values[2]?.ToString());

      //return (mouseOver && !focus && tooltipExist) ? Visibility.Visible : Visibility.Collapsed;
      return (!focused && !textExist && watermarkExist);
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }

  /// <summary>
  /// StringTextBox에서 Watermark가 보일 조건
  /// - Focused : false
  /// - Text : null
  /// - Watermark : Exist
  /// </summary>
  public class NumberWatermarkVisibilityConverter : MarkupExtension, IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == DependencyProperty.UnsetValue)
      {
        Debug.WriteLine("values is unset");
        return Visibility.Collapsed;
      }

      // check value
      for (int i = 0; i < values.Length; i++)
      {
        var p = values[i];
        if (p == DependencyProperty.UnsetValue)
        {
          Debug.WriteLine($"values[{i}] is unset");
          return false;
        }
      }

      bool focused = (bool)values[0];
      bool keyforcused = (bool)values[1];

      return (!focused && !keyforcused);
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }
  /// <summary>
  /// TextBox가 Focused 되지 않은 상태에서 MouseOver
  /// </summary>
  public class NoFocusMouseOverConverter : MarkupExtension, IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == DependencyProperty.UnsetValue)
      {
        Debug.WriteLine("values is unset");
        return Visibility.Collapsed;
      }

      // check value
      for (int i = 0; i < values.Length; i++)
      {
        var p = values[i];
        if (p == DependencyProperty.UnsetValue)
        {
          Debug.WriteLine($"values[{i}] is unset");
          return Visibility.Collapsed;
        }
      }

      bool mouseOver = (bool)values[0];
      bool focused = (bool)values[1];

      return (mouseOver && !focused);
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }
}