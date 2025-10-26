using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UIControls
{
  public class NumberTextBlock : TextBlock
  {
    static NumberTextBlock()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberTextBlock),
        new FrameworkPropertyMetadata(typeof(NumberTextBlock)));

      TextProperty.OverrideMetadata(typeof(NumberTextBlock),
        new FrameworkPropertyMetadata()
        {
          CoerceValueCallback = new CoerceValueCallback(CoerceTextProperty),
        });
                //default(PropertyChangedCallback),
                //(CoerceValueCallback)CoerceTextProperty));
    }

    protected static object CoerceTextProperty(DependencyObject d, object o)
    {
      var tb = (NumberTextBlock)d;
      if (tb != null)
      {
        return tb.science_notation(o);
      }
      return o;
    }


    protected virtual object science_notation(object o)
    {
      string s = o?.ToString();
      if (string.IsNullOrEmpty(s))
      {
        return o;
      }

      double value = 0;
      if (!double.TryParse(s, out value))
      {
        return o;
      }

      var expr = this.GetBindingExpression(TextProperty);
      Type sourceType = NumberHelper.FindSourcePropertyType(expr);

      if (sourceType == null)
      {
        // 바인딩이 없거나, 존재하지 않는 속성이 바인딩 된 경우
        return o;
      }

      // bindin property 타입에 맞춰서 캐스팅
      var v = Convert.ChangeType(value, sourceType);
      double.TryParse(v.ToString(), out value);

      var sn = NumberHelper.ScienceNotation(value, this.ScientificNotation, this.MinScienticNotationtValue, this.DecimalPoint);
      return $"{this.Prefix}{sn}{this.Postfix}";
    }

    #region Scientific Notation
    /// <summary>
    /// scientific notation 사용 여부 (기본: true)
    /// </summary>
    public bool ScientificNotation
    {
      get { return (bool)GetValue(ScientificNotationProperty); }
      set { SetValue(ScientificNotationProperty, value); }
    }
    public static readonly DependencyProperty ScientificNotationProperty =
        DependencyProperty.Register("ScientificNotation", typeof(bool), typeof(NumberTextBlock), new PropertyMetadata(false));

    /// <summary>
    /// 과학적 표기법을 적용할 정수 자리수 (기본: 7자리 숫자부터 과학적 표기법으로 표시)
    /// </summary>
    public int IntegerPoint
    {
      get { return (int)GetValue(IntegerPointProperty); }
      set { SetValue(IntegerPointProperty, value); }
    }
    public static readonly DependencyProperty IntegerPointProperty =
        DependencyProperty.Register("IntegerPoint", typeof(int), typeof(NumberTextBlock),
          new PropertyMetadata(7, new PropertyChangedCallback(OnScienticNotationPointChanged)));
    static void OnScienticNotationPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var tb = d as NumberTextBlock;
      if (tb != null)
      {
        int digits = 0;
        if (int.TryParse(e.NewValue.ToString(), out digits))
        {
          if (digits > 1)
          {
            double v = 1;
            for (int i = 0; i < digits - 1; i++)
            {
              v *= 10;
            }
            tb.MinScienticNotationtValue = v;
          }
        }
      }
    }
    /// <summary>
    /// 과학적 표기법 사용시 매번 정수자리수를 검사하기보다는 기준값으로 비교하기 위한 값
    /// </summary>
    double MinScienticNotationtValue = 1000000; // 기본값 7자리

    /// <summary>
    /// 소수점 자리수 (기본 4)
    /// </summary>
    public int DecimalPoint
    {
      get { return (int)GetValue(DecimalPointProperty); }
      set { SetValue(DecimalPointProperty, value); }
    }
    public static readonly DependencyProperty DecimalPointProperty =
        DependencyProperty.Register("DecimalPoint", typeof(int), typeof(NumberTextBlock),
          new PropertyMetadata(4));

    #endregion

    #region Pre/Postfix
    /// <summary>
    /// Prefix
    /// </summary>
    public string Prefix
    {
      get { return (string)GetValue(PrefixProperty); }
      set { SetValue(PrefixProperty, value); }
    }
    public static readonly DependencyProperty PrefixProperty =
        DependencyProperty.Register("Prefix", typeof(string), typeof(NumberTextBlock),
          new PropertyMetadata(null,null));

    public string Postfix
    {
      get { return (string)GetValue(PostfixProperty); }
      set { SetValue(PostfixProperty, value); }
    }
    public static readonly DependencyProperty PostfixProperty =
        DependencyProperty.Register("Postfix", typeof(string), typeof(NumberTextBlock),
          new PropertyMetadata(null,null));
    #endregion
  }
}