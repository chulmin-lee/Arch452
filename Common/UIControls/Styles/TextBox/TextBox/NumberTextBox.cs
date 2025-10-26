using System.Windows;

namespace UIControls
{
  public class NumberTextBox : CustomTextBoxBase
  {
    static NumberTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberTextBox),
       new FrameworkPropertyMetadata(typeof(NumberTextBox)));
    }

    BindingPropertyInfo _bindingInfo = null;

    protected override void TextUpdated(string s)
    {
      var expr = this.GetBindingExpression(TextProperty);
      _bindingInfo = NumberHelper.GetBindingInfo(expr);

      if (_bindingInfo == null)
        return;

      if (this.Min != this.Max)
      {
        _bindingInfo.Min = this.Min;
        _bindingInfo.Max = this.Max;
      }
      update_watermark(s);
    }

    protected override void update()
    {
      if (update_watermark(this.Text))
      {
        base.update();
      }
      else
      {
        this.cancel();
      }
    }

    /// <summary>
    /// 변환 처리
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    bool update_watermark(object o)
    {
      if (_bindingInfo == null)
      {
        // 바인딩이 없거나, 존재하지 않는 속성이 바인딩 된 경우
        // 그래도 값은 watermark로 보여줘야 한다.
        this.Watermark = o.ToString();
        return false;
      }

      string s = o?.ToString();
      if (string.IsNullOrEmpty(s))
      {
        if (_bindingInfo.IsNullable)
        {
          this.Watermark = "";
          this.Text = "0";
          return true;
        }
        else
        {
          return false;
        }
      }

      if (!double.TryParse(s, out double value))
      {
        // 숫자가 아니면 cancel
        return false;
      }

      double saveValue = value;

      // Min/Max는 바인딩 값이므로 변경하면 안된다.
      // 또한 runtime중 변경될 수 있으므로 매번 검사해야 한다.
      if (this.Min != this.Max)
      {
        _bindingInfo.Min = this.Min;
        _bindingInfo.Max = this.Max;
      }

      _bindingInfo.UseSciNotation = this.ScientificNotation;
      _bindingInfo.MinSciValue = this.MinScienticNotationtValue;
      _bindingInfo.DecimalPoint = this.DecimalPoint;

      string viewValue = NumberHelper.ConvertToScienceNotation(_bindingInfo, ref value);

      this.Watermark = viewValue;
      if (value != saveValue)
      {
        // 예) 정수형에 실수값을 입력하는 경우 변경해야 한다.
        this.Text = value.ToString();
      }
      return true;
    }

    protected override object CoerceWatermark(object o)
    {
      string s = o?.ToString();
      if (string.IsNullOrEmpty(s))
        return o;

      double value = 0;
      if (double.TryParse(s, out value))
      {
        return null;
      }
      else
      {
        cancel();
        return null; // ???
      }
    }

    #region Scientific Notation
    public double Min
    {
      get { return (double)GetValue(MinProperty); }
      set { SetValue(MinProperty, value); }
    }
    public static readonly DependencyProperty MinProperty =
        DependencyProperty.Register("Min", typeof(double), typeof(NumberTextBox),
          new PropertyMetadata(0.0));

    public double Max
    {
      get { return (double)GetValue(MaxProperty); }
      set { SetValue(MaxProperty, value); }
    }
    public static readonly DependencyProperty MaxProperty =
        DependencyProperty.Register("Max", typeof(double), typeof(NumberTextBox),
          new PropertyMetadata(0.0));
    /// <summary>
    /// scientific notation 사용 여부 (기본: true)
    /// </summary>
    public bool ScientificNotation
    {
      get { return (bool)GetValue(ScientificNotationProperty); }
      set { SetValue(ScientificNotationProperty, value); }
    }
    public static readonly DependencyProperty ScientificNotationProperty =
        DependencyProperty.Register("ScientificNotation", typeof(bool), typeof(NumberTextBox),
          new PropertyMetadata(true));

    /// <summary>
    /// 과학적 표기법을 적용할 정수 자리수 (기본: 7자리 숫자부터 과학적 표기법으로 표시)
    /// </summary>
    public int IntegerPoint
    {
      get { return (int)GetValue(IntegerPointProperty); }
      set { SetValue(IntegerPointProperty, value); }
    }
    public static readonly DependencyProperty IntegerPointProperty =
        DependencyProperty.Register("IntegerPoint", typeof(int), typeof(NumberTextBox),
          new PropertyMetadata(7, new PropertyChangedCallback(OnScienticNotationPointChanged)));
    static void OnScienticNotationPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var tb = d as NumberTextBox;
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
        DependencyProperty.Register("DecimalPoint", typeof(int), typeof(NumberTextBox),
          new PropertyMetadata(4));
    #endregion
  }
}