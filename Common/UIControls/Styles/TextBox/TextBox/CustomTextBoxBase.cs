using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace UIControls
{
  public abstract class CustomTextBoxBase : TextBox
  {
    static CustomTextBoxBase()
    {
      // UpdateTrigger : Explicit
      TextProperty.OverrideMetadata(typeof(CustomTextBoxBase),
        new FrameworkPropertyMetadata()
        {
          PropertyChangedCallback = new PropertyChangedCallback(TextPropertyChanged),
          //DefaultUpdateSourceTrigger = UpdateSourceTrigger.Explicit
          //CoerceValueCallback = new CoerceValueCallback(CoerceTextProperty)
        });

      // AcceptsReturn이 설정된 경우 처리
      AcceptsReturnProperty.OverrideMetadata(typeof(CheckedTextBox),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(AcceptsReturnPropertyChanged)));
    }

    protected static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var tb = (CustomTextBoxBase)d;
      if (!tb.IsFocused)
      {
        //Text 가 처음 설정될때 처리할일이 있는 경우(Focus 되지 않았을때만 처리)
        tb.TextUpdated(e.NewValue.ToString());
      }
    }
    static object CoerceTextProperty(DependencyObject d, object baseValue)
    {
      var tb = (CustomTextBoxBase)d;
      if (tb != null)
      {
        return tb.CoerceWatermark(baseValue);
      }
      return baseValue;
    }

    // string 인 경우 s == null 이면 watermark 보이기
    // number 인 경우 처리

    /// <summary>
    /// Text가 변경되었을때 처리
    /// </summary>
    /// <param name="s"></param>
    protected virtual void TextUpdated(string s) { }
    /// <summary>
    /// Text가 변경되었을때 검사할일이 있는 경우. NumberTextBox 인 경우 변환 처리
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    protected virtual object CoerceWatermark(object s) { return s; }
    protected virtual void update()
    {
      //var expr = BindingOperations.GetBindingExpression(this, TextProperty);
      var expr = GetBindingExpression(TextBox.TextProperty);
      expr?.UpdateSource();
    }
    protected virtual void cancel()
    {
      var expr = GetBindingExpression(TextBox.TextProperty);
      expr?.UpdateTarget();
    }
    /// <summary>
    /// 여러번 상속 구현하는 경우 모든 부모의 base 매소드가 호출되므로 별도의 매소드 추가함
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);
      on_key_down(e.Key);
    }
    protected virtual void on_key_down(Key key)
    {
      if(key == Key.Enter && this.EnterUpdateTrigger && !IsAcceptsReturnOn)
      {
        update();
      }
      else if(key == Key.Escape && EscCancelEdit)
      {
        cancel();
      }
    }
    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
      on_got_focus();
    }
    protected virtual void on_got_focus()
    {
      if (FocusSelectAll)
      {
        Dispatcher.BeginInvoke(new Action(() => SelectAll()));
      }
    }
    protected override void OnLostFocus(RoutedEventArgs e)
    {
      // base.OnLostFocus()에서 바인딩 업데이트되므로 그 전에 검사해야 한다.
      update();
      base.OnLostFocus(e);
    }
    protected virtual void on_lost_focus()
    {
      update();
    }


    #region Key 처리

    // EnterUpdateTrigger


    public bool EnterUpdateTrigger
    {
      get { return (bool)GetValue(EnterUpdateTriggerProperty); }
      set { SetValue(EnterUpdateTriggerProperty, value); }
    }

    public static readonly DependencyProperty EnterUpdateTriggerProperty =
    DependencyProperty.Register("EnterUpdateTrigger", typeof(bool), typeof(CustomTextBoxBase), new PropertyMetadata(true));


    /// <summary>
    /// Enter key 입력시 UpdateTrigger
    /// </summary>
    public bool EnterKey
    {
      get { return (bool)GetValue(EnterKeyProperty); }
      set { SetValue(EnterKeyProperty, value); }
    }
    public static readonly DependencyProperty EnterKeyProperty =
        DependencyProperty.Register("EnterKey", typeof(bool), typeof(CustomTextBoxBase), new PropertyMetadata(true));

    // AcceptReturn이 True인 경우 EnterKey 동작을 막아야 한다.
    protected bool IsAcceptsReturnOn = false;

    /// <summary>
    /// TextWrapping 모드에서는 Enter key 무시
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected static void AcceptsReturnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var tb = (CustomTextBoxBase)sender;
      if (!tb.IsFocused)
      {
        tb.IsAcceptsReturnOn = (bool)e.NewValue;
      }
    }
    #endregion


    #region COMMON DP


    public bool EscCancelEdit
    {
      get { return (bool)GetValue(EscCancelEditProperty); }
      set { SetValue(EscCancelEditProperty, value); }
    }

    // Using a DependencyProperty as the backing store for EscCancelEdit.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty EscCancelEditProperty =
    DependencyProperty.Register("EscCancelEdit", typeof(bool), typeof(CustomTextBoxBase), new PropertyMetadata(true));





    /// <summary>
    /// ESC key 입력시 입력 취소
    /// </summary>
    public bool EscKey
    {
      get { return (bool)GetValue(EscKeyProperty); }
      set { SetValue(EscKeyProperty, value); }
    }
    public static readonly DependencyProperty EscKeyProperty =
        DependencyProperty.Register("EscKey", typeof(bool), typeof(CustomTextBoxBase), new PropertyMetadata(true));

    /// <summary>
    /// Focus 되었을때 전체 선택
    /// </summary>
    public bool FocusSelectAll
    {
      get { return (bool)GetValue(FocusSelectAllProperty); }
      set { SetValue(FocusSelectAllProperty, value); }
    }
    public static readonly DependencyProperty FocusSelectAllProperty =
        DependencyProperty.Register("FocusSelectAll", typeof(bool), typeof(CustomTextBoxBase), new PropertyMetadata(true));

    /// <summary>
    /// string : 내용이 없을때 표시할 내용
    /// number : 출력형식에 맞게 변형하여 출력
    /// </summary>
    public string Watermark
    {
      get { return (string)GetValue(WatermarkProperty); }
      set { SetValue(WatermarkProperty, value); }
    }
    public static readonly DependencyProperty WatermarkProperty =
        DependencyProperty.Register("Watermark", typeof(string), typeof(CustomTextBoxBase), new PropertyMetadata(null));
    #endregion


    #region Header
    public string Header
    {
      get { return (string)GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(string), typeof(CustomTextBoxBase), new PropertyMetadata(null));

    public GridLength HeaderWidth
    {
      get { return (GridLength)GetValue(HeaderWidthProperty); }
      set { SetValue(HeaderWidthProperty, value); }
    }
    public static readonly DependencyProperty HeaderWidthProperty =
        DependencyProperty.Register("HeaderWidth", typeof(GridLength), typeof(CustomTextBoxBase),
          new PropertyMetadata(new GridLength(0.0, GridUnitType.Auto)));

    public HorizontalAlignment HeaderHorizontalAlignment
    {
      get { return (HorizontalAlignment)GetValue(HeaderHorizontalAlignmentProperty); }
      set { SetValue(HeaderHorizontalAlignmentProperty, value); }
    }
    public static readonly DependencyProperty HeaderHorizontalAlignmentProperty =
        DependencyProperty.Register("HeaderHorizontalAlignment", typeof(HorizontalAlignment), typeof(CustomTextBoxBase),
          new PropertyMetadata(HorizontalAlignment.Left));

    public Thickness HeaderMargin
    {
      get { return (Thickness)GetValue(HeaderMarginProperty); }
      set { SetValue(HeaderMarginProperty, value); }
    }
    public static readonly DependencyProperty HeaderMarginProperty =
        DependencyProperty.Register("HeaderMargin", typeof(Thickness), typeof(CustomTextBoxBase),
          new PropertyMetadata(new Thickness(0, 0, 0, 0)));

    public SolidColorBrush HeaderForeground
    {
      get { return (SolidColorBrush)GetValue(HeaderForegroundProperty); }
      set { SetValue(HeaderForegroundProperty, value); }
    }
    public static readonly DependencyProperty HeaderForegroundProperty =
        DependencyProperty.Register("HeaderForeground", typeof(SolidColorBrush), typeof(CustomTextBoxBase),
          new PropertyMetadata(Brushes.White));
    #endregion


  }
}