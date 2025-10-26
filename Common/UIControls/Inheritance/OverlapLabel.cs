using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace UIControls
{
  public class OverlapLabel : Label
  {
    public OverlapLabel() : base()
    {
      this.Stroke = Brushes.Transparent;
      this.StrokeThickness = 0;
      this.SizeChanged += new SizeChangedEventHandler(GLabel_SizeChanged);
    }

    private void GLabel_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
      if (!this.AutoSize && e.NewSize != Size.Empty)
      {
        OriginSize = e.NewSize;
      }
      this.InvalidateVisual();
    }

    /// <summary>
    /// Label 텍스트를 가져오거나 설정합니다.
    /// </summary>
    //public string Text
    //{
    //  get
    //  {
    //    return _Text;
    //  }
    //  set
    //  {
    //    _fitted = false;
    //    _Text = value;
    //    this.InvalidateVisual();
    //  }
    //}

    public static readonly DependencyProperty OverlapStringsProperty =
            DependencyProperty.Register("OverlapStrings", typeof(ObservableCollection<OverlapString>), typeof(OverlapLabel)
              ,new PropertyMetadata(new ObservableCollection<OverlapString>()));

    public ObservableCollection<OverlapString> OverlapStrings
    {
      get { return (ObservableCollection<OverlapString>)GetValue(OverlapStringsProperty); }
      set { SetValue(OverlapStringsProperty, value); }
    }

    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(OverlapLabel),
          new PropertyMetadata(null, new PropertyChangedCallback(OnTextChanged)));
    static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as OverlapLabel)?.TextChanged();
    }
    void TextChanged()
    {
      this.InvalidateVisual();
    }

    /// <summary>
    /// 컨트롤이 내용에 맞게 자신의 크기를 자동으로 지정하는지 여부르 가져오거나 설정합니다.
    /// </summary>
    public bool AutoSize
    {
      get { return _autosize; }
      set
      {
        _autosize = value;
        if (!OriginSize.IsEmpty && !_autosize)
        {
          this.Width = OriginSize.Width;
          this.Height = OriginSize.Height;
        }
        this.InvalidateVisual();
      }
    }

    /// <summary>
    /// 줄 높이 또는 텍스트 줄 간의 줄 간격을 가져오거나 설정합니다.
    /// </summary>
    public double LineHeight
    {
      get { return lineheight; }
      set
      {
        lineheight = value;
        this.InvalidateVisual();
      }
    }
    /// <summary>
    /// 문자간 간격을 가져오거나 설정합니다.
    /// </summary>
    public double LetterSpacing
    {
      get
      {
        return letter_spacing;
      }
      set
      {
        letter_spacing = value;
        this.InvalidateVisual();
      }
    }

    /// <summary>
    /// 윤곽선을 칠하는 방법을 지정하는 Brush를 가져오거나 설정합니다.
    /// </summary>
    public Brush Stroke { get; set; }
    /// <summary>
    /// 윤곽선의 너비를 가져오거나 설정합니다.
    /// </summary>
    public double StrokeThickness { get; set; }

    public Size OriginSize = Size.Empty;

    public Drawing GetDrawing()
    {
      DrawingGroup drawingGroup = new DrawingGroup();

      using (DrawingContext drawingContext = drawingGroup.Open())
      {
        OnRender(drawingContext);
        // Return the updated DrawingGroup content to be used by the control.
        return drawingGroup;
      }
    }

    //private string _Text = string.Empty;
    private bool _fitted = false;
    private bool _autosize = false;
    double lineheight = 0;
    double letter_spacing = 0;

    // List<OverlapString> _Effects = new List<OverlapString>();

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      drawingContext.DrawRectangle(Background, null, new Rect(RenderSize));

      if (string.IsNullOrEmpty(this.Text) || double.IsNaN(this.ActualWidth) || double.IsNaN(this.ActualHeight))
      {
        return;
      }

      //if (this.ActualHeight <= 0 || this.ActualWidth <= 0)
      //  return;

      OriginSize = new Size(this.ActualWidth, this.ActualHeight);

      //if (OriginSize.IsEmpty && !double.IsNaN(this.Width) && !double.IsNaN(this.Height))
      //  OriginSize = new Size(this.Width, this.Height);

      if (this.HorizontalContentAlignment == System.Windows.HorizontalAlignment.Stretch)
      {
        StretchOnRender(drawingContext);
        return;
      }
      // 자간 간격 조절
      if (letter_spacing > 0)
      {
        LetterSpacingOnRender(drawingContext);
        return;
      }

      FormattedText formattedText = AutoFontSize(this, this.Text, OriginSize, 10);    // Consts.MIN_FONTSIZE

      if (this.AutoSize && !OriginSize.IsEmpty && !_fitted)
      {
        _fitted = true;
        this.Width = formattedText.Width + this.Padding.Left + this.Padding.Right;
        this.Height = formattedText.Height + this.Padding.Top + this.Padding.Bottom;
      }

      double drawX = 0, drawY = 0;

      if (this.VerticalContentAlignment == System.Windows.VerticalAlignment.Bottom)
        drawY = OriginSize.Height - formattedText.Height - this.Padding.Bottom;
      else if (this.VerticalContentAlignment == System.Windows.VerticalAlignment.Center)
        drawY = (this.Padding.Top - this.Padding.Bottom) + (OriginSize.Height - formattedText.Height) / 2;
      else
        drawY = this.Padding.Top;

      drawX = this.Padding.Left;
      if (this.AutoSize)
      {
        double centerX = (OriginSize.Width - formattedText.Width) / 2;
        if (this.HorizontalContentAlignment == System.Windows.HorizontalAlignment.Right)
          drawX = -(centerX * 2) + this.Padding.Left + (this.Padding.Right * 2);
        else if (this.HorizontalContentAlignment == System.Windows.HorizontalAlignment.Center)
          drawX = -centerX + this.Padding.Left + this.Padding.Right;
        else
          drawX = this.Padding.Left;
        //
        drawY = this.Padding.Top;
      }

      // Build the geometry object that represents the text.
      if (this.StrokeThickness > 0 && this.Stroke != null)
      {
        Geometry _textGeometry = formattedText.BuildGeometry(new System.Windows.Point(drawX, drawY));
        drawingContext.DrawGeometry(this.Foreground, new Pen(Stroke, StrokeThickness), _textGeometry);
      }

      drawingContext.DrawText(formattedText, new Point(drawX, drawY));
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      this.InvalidateVisual();
    }

    void StretchOnRender(DrawingContext drawingContext)
    {
      List<FormattedText> formattedText = new List<FormattedText>();
      try
      {
        double tWidth = 0;
        for (int i = 0; i < this.Text.Length; i++)
        {
          formattedText.Add(AutoFontSize(this, this.Text.Substring(i, 1), OriginSize, 10));   // Consts.MIN_FONTSIZE
          tWidth += formattedText[i].Width;
        }

        int gapCnt = this.Text.Length - 1;
        double gapWidth = (this.Width - tWidth - this.Padding.Left - this.Padding.Right) / gapCnt;

        double drawX = 0, drawY = 0;
        if (this.VerticalContentAlignment == System.Windows.VerticalAlignment.Bottom)
          drawY = OriginSize.Height - formattedText[0].Height - this.Padding.Bottom;
        else if (this.VerticalContentAlignment == System.Windows.VerticalAlignment.Center)
          drawY = (this.Padding.Top - this.Padding.Bottom) + (OriginSize.Height - formattedText[0].Height) / 2;
        else
          drawY = this.Padding.Top;

        drawX = this.Padding.Left;
        foreach (var f in formattedText)
        {
          drawingContext.DrawText(f, new Point(drawX, drawY));
          drawX += (f.Width + gapWidth);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Stretch : {ex.Message}");
      }
    }

    void LetterSpacingOnRender(DrawingContext drawingContext)
    {
      List<FormattedText> formattedText = new List<FormattedText>();
      try
      {
        double tWidth = 0;
        for (int i = 0; i < this.Text.Length; i++)
        {
          formattedText.Add(AutoFontSize(this, this.Text.Substring(i, 1), OriginSize, 10));   // Consts.MIN_FONTSIZE
          tWidth += formattedText[i].WidthIncludingTrailingWhitespace;

          formattedText[i].TextAlignment = TextAlignment.Left;
        }

        double spWidth = ((this.Text.Length - 1) * letter_spacing);
        tWidth += spWidth;

        double drawX = 0, drawY = 0;
        if (this.VerticalContentAlignment == System.Windows.VerticalAlignment.Bottom)
          drawY = OriginSize.Height - formattedText[0].Height - this.Padding.Bottom;
        else if (this.VerticalContentAlignment == System.Windows.VerticalAlignment.Center)
          drawY = (this.Padding.Top - this.Padding.Bottom) + (OriginSize.Height - formattedText[0].Height) / 2;
        else
          drawY = this.Padding.Top;

        double centerX = (OriginSize.Width - tWidth) / 2;
        if (this.HorizontalContentAlignment == System.Windows.HorizontalAlignment.Right)
          drawX = OriginSize.Width - tWidth - this.Padding.Right;
        else if (this.HorizontalContentAlignment == System.Windows.HorizontalAlignment.Center)
          drawX = centerX + this.Padding.Left;// + this.Padding.Right;
        else
          drawX = this.Padding.Left;

        foreach (var f in formattedText)
        {
          drawingContext.DrawText(f, new Point(drawX, drawY));
          drawX += (f.WidthIncludingTrailingWhitespace + letter_spacing);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Stretch : {ex.Message}");
      }
    }

    FormattedText AutoFontSize(OverlapLabel control, string text, Size OriginSize, double FontMinSize)
    {
      double fSize = control.FontSize;

      double eleWidth = OriginSize.Width - control.Padding.Left - control.Padding.Right;
      double eleHeight = OriginSize.Height - control.Padding.Top - control.Padding.Bottom;

      FormattedText formattedText = new FormattedText(
                text,
                CultureInfo.GetCultureInfo("Ko"),   // Consts.Globalization_Culture.ToString()
                FlowDirection.LeftToRight,
                new Typeface(control.FontFamily, control.FontStyle, control.FontWeight, control.FontStretch),
                fSize,
                control.Foreground);

      if (control.LineHeight > 0)
        formattedText.LineHeight = control.LineHeight;
      if (!string.IsNullOrEmpty(text))
      {
        foreach (OverlapString effect in this.OverlapStrings)
        {
          if (string.IsNullOrEmpty(effect.Text))
            continue;
          // 효과 줄 문자열 찾기.
          int indexof = control.Text.IndexOf(effect.Text);
          if (indexof < 0 || effect.Text.Length <= 0)
            continue;

          formattedText.SetForegroundBrush(new SolidColorBrush(effect.ForeColor), indexof, effect.Text.Length);
        }
      }

      if (double.IsNaN(eleWidth) || double.IsNaN(eleHeight))
        return formattedText;

      formattedText.Trimming = TextTrimming.None;
      formattedText.TextAlignment = GetTextAlignment(control.HorizontalContentAlignment);

      formattedText.MaxTextWidth = eleWidth;
      // 높이 측정을 위해
      formattedText.MaxTextHeight = 1920.0;

      while (FontMinSize <= fSize)
      {
        if (formattedText.Width < eleWidth && formattedText.Height < eleHeight)
        {
          // 원래 높이를 맞춘다.
          formattedText.MaxTextHeight = eleHeight;
          break;
        }
        fSize -= 1;
        // The font size is calculated in terms of points -- not as device-independent pixels.
        formattedText.SetFontSize(fSize);
      }

      return formattedText;
    }

    TextAlignment GetTextAlignment(HorizontalAlignment Alignment)
    {
      switch (Alignment)
      {
        case HorizontalAlignment.Center:
          return TextAlignment.Center;
        case HorizontalAlignment.Left:
          return TextAlignment.Left;
        case HorizontalAlignment.Right:
          return TextAlignment.Right;
        case HorizontalAlignment.Stretch:
          return TextAlignment.Justify;
      }
      return TextAlignment.Left;
    }

    #region Click

    public static readonly RoutedEvent ClickEvent;

    static OverlapLabel()
    {
      ClickEvent = ButtonBase.ClickEvent.AddOwner(typeof(OverlapLabel));
    }

    public event RoutedEventHandler Click
    {
      add { AddHandler(ClickEvent, value); }
      remove { RemoveHandler(ClickEvent, value); }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      CaptureMouse();
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);

      if (IsMouseCaptured)
      {
        ReleaseMouseCapture();

        if (IsMouseOver)
          RaiseEvent(new RoutedEventArgs(ClickEvent, this));
      }
    }

    #endregion Click
  }

  /// <summary>
  /// 텍스트의 효과 표출 클래스 입니다. 문자열에서 특정 문자를 폰트, 색상 등을 다르게 지정할때 사용합니다.
  /// </summary>
  public class OverlapString
  {
    public string Text { get; set; }
    public Color ForeColor { get; set; }
  }

  public class FontSetting
  {
    public FontSetting()
    {
      this.FontFamily = new FontFamily();
      this.FontSize = 12;
      this.FontStretch = new FontStretch();
      this.FontStyle = new FontStyle();
      this.FontWeight = new FontWeight();
    }

    /// <summary>
    /// 관련 글꼴 패밀리를 나타냅니다.
    /// </summary>
    public FontFamily FontFamily { get; set; }

    public double FontSize { get; set; }

    /// <summary>
    /// 글꼴의 일반 가로 세로 비율을 비교 하는 글꼴이 늘어나는 정도 설명 합니다.
    /// </summary>
    public FontStretch FontStretch { get; set; }

    /// <summary>
    /// 글꼴 스타일을 보통, 기울임꼴 또는 오블리크로 나타내는 구조를 정의합니다.
    /// </summary>
    public FontStyle FontStyle { get; set; }

    /// <summary>
    /// 밝기 또는 스트로크의 측면에서 서체의 밀도를 나타냅니다.
    /// </summary>
    public FontWeight FontWeight { get; set; }
  }
}