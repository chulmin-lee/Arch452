using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace UIControls
{
  public class OutlineTextBox : TextBox
  {
    TextBoxInternalState STATE;
    Grid RootGrid { get; set; }

    static OutlineTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(OutlineTextBox),
  new FrameworkPropertyMetadata(typeof(OutlineTextBox)));

      // UpdateTrigger : Explicit
      TextProperty.OverrideMetadata(typeof(OutlineTextBox),
        new FrameworkPropertyMetadata()
        {
          PropertyChangedCallback = new PropertyChangedCallback(TextPropertyChanged),
          DefaultUpdateSourceTrigger = UpdateSourceTrigger.Explicit
          //CoerceValueCallback = new CoerceValueCallback(CoerceTextProperty)
        });
    }

    static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var tb = (OutlineTextBox)d;
      if (!tb.IsFocused)
      {
        //Text 가 처음 설정될때 처리할일이 있는 경우(Focus 되지 않았을때만 처리)
        tb.TextUpdated(e.NewValue.ToString());
      }
    }

    public OutlineTextBox()
    {
      STATE = new TextBoxInternalState(this);
    }

    public override void OnApplyTemplate()
    {
      this.RootGrid = (Grid)base.GetTemplateChild("RootGrid");
      base.OnApplyTemplate();
    }

    /// <summary>
    /// Text가 변경되었을때 처리
    /// </summary>
    /// <param name="s"></param>
    protected virtual void TextUpdated(string s)
    {
      update_watermark(s);
    }

    protected virtual bool update_watermark(object o)
    {
      this.Watermark = o?.ToString();
      return true;
    }

    protected virtual void update()
    {
      if (update_watermark(this.Text))
      {
        var expr = this.GetBindingExpression(TextBox.TextProperty);
        expr?.UpdateSource();
        STATE.IsProcessed = true;
        remove_focus();
      }
      else
      {
        this.cancel();
      }
    }
    protected virtual void cancel()
    {
      var expr = this.GetBindingExpression(TextBox.TextProperty);
      expr?.UpdateTarget();
      STATE.IsProcessed = true;
      remove_focus();
    }
    protected virtual void remove_focus()
    {
      if (this.IsFocused)
      {
        this.RootGrid.Focus();
      }
    }
    /// <summary>
    /// 여러번 상속 구현하는 경우 모든 부모의 base 매소드가 호출되므로 별도의 매소드 추가함
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);
      this.on_key_down(e.Key);
    }
    protected virtual void on_key_down(Key key)
    {
      if (this.EnterKey && key == Key.Enter)
      {
        this.update();
      }
      else if (this.EscKey && key == Key.Escape)
      {
        this.cancel();
      }
    }
    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
      this.on_got_focus();
    }
    protected virtual void on_got_focus()
    {
      if (this.FocusSelectAll)
      {
        this.Dispatcher.BeginInvoke(new Action(() => this.SelectAll()));
      }

      #region DETCT_MOUSE_DOWN
      if (this.DetectMouseDown)
      {
        if (!STATE.IsMouseEnter)
        {
          // tab으로 들어온 경우임. MouseDown에서 capture하면 Up에서 풀림
          add_outside_handler();
        }
      }
      #endregion
    }
    protected override void OnLostFocus(RoutedEventArgs e)
    {
      // base.OnLostFocus()에서 바인딩 업데이트되므로 그 전에 검사해야 한다.
      this.on_lost_focus();
      base.OnLostFocus(e);
      STATE.IsProcessed = false;
    }
    protected virtual void on_lost_focus()
    {
      #region DETCT_MOUSE_DOWN
      if (this.DetectMouseDown)
      {
        STATE.IsMouseEnter = false;
        if (STATE.IsAddHandler)
        {
          remove_outside_handler();
        }
      }
      #endregion

      // 외부 마우스 클릭 또는 Tab으로 벗어나는 경우 처리
      if(!STATE.IsProcessed)
        this.update();
    }
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      #region DETCT_MOUSE_DOWN
      if (this.DetectMouseDown)
      {
        // base.OnMouseDown() 호출전에 처리하자
        // 나중에 하면 OnGotFocus 처리후 호출된다.

        STATE.IsMouseEnter = true;
        if (STATE.IsAddHandler)
        {
          remove_outside_handler();
        }
      }
      #endregion

      base.OnMouseDown(e);
    }
    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);

      #region DETCT_MOUSE_DOWN
      if (this.DetectMouseDown)
      {
        STATE.IsMouseEnter = true;
        if (!STATE.IsAddHandler)
        {
          add_outside_handler();
        }
      }
      #endregion
    }


    #region DETCT_MOUSE_DOWN
    void add_outside_handler()
    {
      if (this.DetectMouseDown && STATE.CanCapture)
      {
        if (Mouse.Capture(this))
        {
          Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(this, outside_mouse_down);
          Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, outside_mouse_down);
          STATE.IsAddHandler = true;
        }
      }
    }
    void remove_outside_handler()
    {
      if (this.DetectMouseDown)
      {
        Mouse.Capture(null);
        Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(this, outside_mouse_down);
        STATE.IsAddHandler = false;
      }
    }
    void outside_mouse_down(object sender, MouseButtonEventArgs e)
    {
      if (this.DetectMouseDown)
      {
        HitTestResult result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
        if (result == null)
        {
          //Debug.WriteLine($"outhandler : outside click");
          STATE.IsMouseEnter = false;
          STATE.IsProcessed = false;
          on_lost_focus();
        }
      }
    }
    #endregion



    #region COMMON DP
    /// <summary>
    /// Enter key 입력시 UpdateTrigger
    /// </summary>
    public bool EnterKey
    {
      get { return (bool)GetValue(EnterKeyProperty); }
      set { SetValue(EnterKeyProperty, value); }
    }
    public static readonly DependencyProperty EnterKeyProperty =
        DependencyProperty.Register("EnterKey", typeof(bool), typeof(OutlineTextBox),
          new PropertyMetadata(true));

    /// <summary>
    /// ESC key 입력시 입력 취소
    /// </summary>
    public bool EscKey
    {
      get { return (bool)GetValue(EscKeyProperty); }
      set { SetValue(EscKeyProperty, value); }
    }
    public static readonly DependencyProperty EscKeyProperty =
        DependencyProperty.Register("EscKey", typeof(bool), typeof(OutlineTextBox),
          new PropertyMetadata(true));

    /// <summary>
    /// Focus 되었을때 전체 선택
    /// </summary>
    public bool FocusSelectAll
    {
      get { return (bool)GetValue(FocusSelectAllProperty); }
      set { SetValue(FocusSelectAllProperty, value); }
    }
    public static readonly DependencyProperty FocusSelectAllProperty =
        DependencyProperty.Register("FocusSelectAll", typeof(bool), typeof(OutlineTextBox),
          new PropertyMetadata(true));

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
        DependencyProperty.Register("Watermark", typeof(string), typeof(OutlineTextBox),
          new PropertyMetadata(defaultValue: null));
    #endregion

    #region Prefix/Postfix
    public string Prefix
    {
      get { return (string)GetValue(PrefixProperty); }
      set { SetValue(PrefixProperty, value); }
    }
    public static readonly DependencyProperty PrefixProperty =
        DependencyProperty.Register("Prefix", typeof(string), typeof(OutlineTextBox),
          new PropertyMetadata(defaultValue: null));

    public string Postfix
    {
      get { return (string)GetValue(PostfixProperty); }
      set { SetValue(PostfixProperty, value); }
    }
    public static readonly DependencyProperty PostfixProperty =
        DependencyProperty.Register("Postfix", typeof(string), typeof(OutlineTextBox),
          new PropertyMetadata(defaultValue: null));
    #endregion

    #region OUTLINE
    public Brush OutlineFill
    {
      get { return (Brush)GetValue(OutlineFillProperty); }
      set { SetValue(OutlineFillProperty, value); }
    }
    public static readonly DependencyProperty OutlineFillProperty = DependencyProperty.Register(
      "OutlineFill", typeof(Brush), typeof(OutlineTextBox),
      new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush OutlineStroke
    {
      get { return (Brush)GetValue(OutlineStrokeProperty); }
      set { SetValue(OutlineStrokeProperty, value); }
    }
    public static readonly DependencyProperty OutlineStrokeProperty = DependencyProperty.Register(
      "OutlineStroke", typeof(Brush), typeof(OutlineTextBox),
      new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region OutsideMouseDown
    /// <summary>
    /// 외부 마우스 클릭 감지 여부
    /// </summary>
    public bool DetectMouseDown
    {
      get { return (bool)GetValue(DetectMouseDownProperty); }
      set { SetValue(DetectMouseDownProperty, value); }
    }
    public static readonly DependencyProperty DetectMouseDownProperty =
        DependencyProperty.Register("DetectMouseDown", typeof(bool), typeof(OutlineTextBox), new PropertyMetadata(false));
    #endregion
  }


  internal class TextBoxInternalState
  {
    TextBox M;
    public bool IsProcessed { get; set; }
    public bool IsAddHandler { get; set; }
    public bool IsMouseEnter { get; set; }
    public bool IsFocused => M.IsFocused;
    public bool IsCaptured => M.IsMouseCaptured;
    public bool CanCapture => M.IsFocused && !M.IsMouseCaptured;

    public TextBoxInternalState(TextBox p)
    {
      this.M = p;
    }
  }
}
