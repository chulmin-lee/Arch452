using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace UIControls
{
  public static class NumberHelper
  {
    public static string ScienceNotation(double value, bool notation, double notationMin, int decimalPoint)
    {
      bool isDouble = value % 1 != 0;

      string viewValue = value.ToString();
      if (notation)
      {
        if (value < notationMin && value > -notationMin)
        {
          // 소수점뒤에 0을 추가하기 위해서 변경
          //viewValue = Math.Round(value, decimalPoint).ToString();
          if (isDouble)
            viewValue = value.ToString($"F{decimalPoint}");
        }
        else
        {
          viewValue = value.ToString("0.####E-0", CultureInfo.InvariantCulture);
        }
      }
      else
      {
        //viewValue = Math.Round(value, decimalPoint).ToString();
        if (isDouble)
          viewValue = value.ToString($"F{decimalPoint}");
      }
      return viewValue;
    }

    public static string ConvertToScienceNotation(BindingPropertyInfo info, ref double value) //, bool notation, double notationMin, int decimalPoint)
    {
      if (value < info.Min) value = info.Min;
      else if (value > info.Max) value = info.Max;

      if (info.IsInteger)
      {
        if (value % 1 != 0)
        {
          // 실제 바로 바인딩되는 값이 아니므로 int로 casting 해도 된다.
          value = (int)value;
        }
      }


      bool isDouble = value % 1 != 0;

      string viewValue = value.ToString();
      if (info.UseSciNotation)
      {
        if (value < info.MinSciValue && value > -info.MinSciValue)
        {
          // 소수점뒤에 0을 추가하기 위해서 변경
          //viewValue = Math.Round(value, decimalPoint).ToString();
          if (isDouble)
            viewValue = value.ToString($"F{info.DecimalPoint}");
        }
        else
        {
          viewValue = value.ToString("0.####E-0", CultureInfo.InvariantCulture);
        }
      }
      else
      {
        //viewValue = Math.Round(value, decimalPoint).ToString();
        if (isDouble)
          viewValue = value.ToString($"F{info.DecimalPoint}");
      }
      return viewValue;
    }

    /// <summary>
    /// DependencyProperty에 바인딩된 source property의 Type을 얻는다.
    /// </summary>
    /// <param name="expr">DP 바인딩 정보</param>
    /// <returns></returns>
    public static Type FindSourcePropertyType(BindingExpression expr)
    {
      Type sourceType = null;
      if(expr != null)
      {
        if (expr.ResolvedSource != null)
        {
          object src = expr.ResolvedSource;
          string pn = expr.ResolvedSourcePropertyName;
          sourceType = src.GetType().GetProperty(pn).PropertyType;
        }
        else
        {
          // 부모의 DataContext를 사용할때는 ResolvedSource가 null 임
          string[] splits = expr.ParentBinding.Path.Path.Split('.');
          Type type = expr.DataItem.GetType();
          foreach (string split in splits)
          {
            var pi = type.GetProperty(split);
            if (pi != null)
            {
              // Binding=Shape.Width 처럼 계층으로 바인딩한 경우 계층별로 찾아야 한다.
              type = sourceType = pi.PropertyType;
            }
          }
        }
      }
      else
      {
        // 바인딩이 없다.
        Debug.Write("no binding info");
      }
      return sourceType;
    }

    public static BindingPropertyInfo GetBindingInfo(BindingExpression expr)
    {
      if (expr != null)
      {
        var info = new BindingPropertyInfo();

        if (expr.ResolvedSource != null)
        {
          object src = expr.ResolvedSource;
          string pn = expr.ResolvedSourcePropertyName;
          info.SourceType = src.GetType().GetProperty(pn).PropertyType;
        }
        else
        {
          // 부모의 DataContext를 사용할때는 ResolvedSource가 null 임
          string[] splits = expr.ParentBinding.Path.Path.Split('.');
          Type type = expr.DataItem.GetType();
          foreach (string split in splits)
          {
            var pi = type.GetProperty(split);
            if (pi != null)
            {
              // Binding=Shape.Width 처럼 계층으로 바인딩한 경우 계층별로 찾아야 한다.
              type = pi.PropertyType;
            }
          }
          info.SourceType = type;
        }

        if (info.SourceType == null)
          return null;

        info.IsNullable = Nullable.GetUnderlyingType(info.SourceType) != null;
        if (info.IsNullable)
        {
          info.SourceType = Nullable.GetUnderlyingType(info.SourceType);
        }

        double min = 0;
        double max = 0;
        bool isInteger = true;
        if (min == max)
        {
          if (info.SourceType == typeof(Byte)) { min = byte.MinValue; max = byte.MaxValue; }
          else if (info.SourceType == typeof(SByte)) { min = sbyte.MinValue; max = sbyte.MaxValue; }
          else if (info.SourceType == typeof(UInt16)) { min = ushort.MinValue; max = ushort.MaxValue; }
          else if (info.SourceType == typeof(Int16)) { min = short.MinValue; max = short.MaxValue; }
          else if (info.SourceType == typeof(UInt32)) { min = uint.MinValue; max = uint.MaxValue; }
          else if (info.SourceType == typeof(Int32)) { min = int.MinValue; max = int.MaxValue; }
          else if (info.SourceType == typeof(UInt64)) { min = ulong.MinValue; max = ulong.MaxValue; }
          else if (info.SourceType == typeof(Int64)) { min = long.MinValue; max = long.MaxValue; }
          else if (info.SourceType == typeof(Single)) { min = float.MinValue; max = float.MaxValue; isInteger = false; }
          else if (info.SourceType == typeof(Double)) { min = double.MinValue; max = double.MaxValue; isInteger = false; }
          else
          {
            throw new Exception("unknown type");
          }

          info.IsInteger = isInteger;
          info.Min = min;
          info.Max = max;
        }

        return info;
      }
      else
      {
        // 바인딩이 없다.
        Debug.Write("no binding info");
        return null;
      }
    }
  }

  public class BindingPropertyInfo
  {
    public Type SourceType = null;
    public bool IsNullable = false;
    public double Min = 0;
    public double Max = 0;
    public bool IsInteger = false;

    //
    public bool UseSciNotation;
    public double MinSciValue;
    public int DecimalPoint;
  }

}
