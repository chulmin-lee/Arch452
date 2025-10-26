using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace UIControls
{
  [ContentProperty("Text")]
  public class TextBorder : Control
  {
    //static Dictionary<int,double> _fontSizes = new Dictionary<int, double>();
    static double _dpi = 1.25;
    static TextBorder()
    {
      TextBorder._dpi = GetDpi();

      //LOG.dc("static constructor");

      // 이 부분이 없으면 기본 style을 찾지 못한다.
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBorder),
       new FrameworkPropertyMetadata(typeof(TextBorder)));
    }

    public TextBorder()
    {
      //LOG.dc("constructor");
      TextDecorations = new TextDecorationCollection();

      //this.Initialized += (s, e) => { LOG.w("Initialized"); };
      //this.Loaded += (s, e) => { LOG.w("Loaded"); };
      //this.Unloaded += (s, e) => { LOG.w("Unloaded"); };
      //this.SizeChanged += (s, e) => { LOG.w("SizeChanged"); };
      //LOG.w($"IsInitialized = {this.IsInitialized}");
    }
    //protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    //{
    //  //LOG.wc(""); ;
    //  base.OnDpiChanged(oldDpi, newDpi);
    //}

    //protected override void OnInitialized(EventArgs e)
    //{
    //  //LOG.wc($"IsInitialized = {this.IsInitialized}");
    //  base.OnInitialized(e);
    //}
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      //LOG.wc(""); ;
      base.OnRenderSizeChanged(sizeInfo);
    }
    public override void BeginInit()
    {
      //LOG.wc(""); ;
      base.BeginInit();
    }

    public override void EndInit()
    {
      //LOG.wc(""); ;
      base.EndInit();
    }

    public override void OnApplyTemplate()
    {
      //LOG.wc(""); ;
      base.OnApplyTemplate();
    }

    /// <summary>
    /// 지정된 폰트 사용시 영역을 넘게되는 경우 최소 폰트 크기를 계산한다.
    /// TextTrimming 을 사용하면 계산하지 않는다.
    /// TextWrapping == none 이면 줄바꿈없이 영역에 들어가도록 계산한다
    /// 아니면 2줄이 되도록 계산한다.
    ///
    /// 최대한 1줄에 표시하도록하는데, 지정 폰트의 60% 로도 안되면 2줄로 한다.
    ///
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="text"></param>
    /// <param name="typeface"></param>
    /// <returns></returns>
    double GetFontSize(Size bounds, string text, Typeface typeface)
    {
      double min_size = 5;
      double max_size = this.FontSize;

      //if (_fontSizes.TryGetValue(text.Length, out var desiredFontSize))
      //{
      //  return desiredFontSize;
      //}

      FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, 1, Brushes.Black);//, _dpi);
      //formattedText.MaxTextWidth = bounds.Width;
      //formattedText.MaxTextHeight = bounds.Height;

      formattedText.SetFontSize(max_size);
      Size box = formattedText.GetSize();

      //if (box.Height <= bounds.Height && box.Width <= bounds.Width)
      //{
      //  _fontSizes.Add(text.Length, max_size);
      //  return max_size;
      //}

      Size text_size = new Size(0,0);

      // 높이부터 맞춘다
      while (max_size - min_size > 0.5f)
      {
        double pt = (min_size + max_size) / 2d;

        formattedText.SetFontSize(pt);
        text_size = formattedText.GetSize();

        if (text_size.Height > bounds.Height)
          max_size = pt;
        else
          min_size = pt;
      }

      max_size = min_size;
      min_size = 5;

      if (this.TextWrapping == TextWrapping.NoWrap)
      {
        do
        {
          double pt = (min_size + max_size) / 2d;

          formattedText.SetFontSize(pt);
          text_size = formattedText.GetSize();

          if (text_size.Width > bounds.Width)
            max_size = pt;
          else
            min_size = pt;
        } while (max_size - min_size > 0.5f);

        max_size = min_size;
      }
      else
      {
        // 2줄로 하기전에 50%까지 줄여봄
        double temp_max = max_size;
        double temp_min = max_size * 0.7;

        formattedText.SetFontSize(temp_min);
        Size temp_size = formattedText.GetSize();

        if (temp_size.Width <= bounds.Width)
        {
          // 1줄로 가능
          do
          {
            double pt = (temp_min + temp_max) / 2d;

            formattedText.SetFontSize(pt);
            text_size = formattedText.GetSize();

            if (text_size.Width > bounds.Width)
              temp_max = pt;
            else
              temp_min = pt;
          } while (temp_max - temp_min > 0.5f);

          max_size = temp_max;
        }
        else
        {
          // 2줄로 해야함
          double want_height = text_size.Height / 2;
          temp_min = 10;
          do
          {
            double pt = (temp_min + temp_max) / 2d;

            formattedText.SetFontSize(pt);
            text_size = formattedText.GetSize();

            if (text_size.Height > want_height)
              temp_max = pt;
            else
              temp_min = pt;
          } while (temp_max - temp_min > 0.1f);

          max_size = temp_max;
        }
      }

      //_fontSizes.Add(text.Length, max_size);
      return max_size;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      //LOG.ic($"size = {availableSize}");
      CreateFormattedText();

      if (formattedText != null)
      {
        // auto font size 시 availableSize를 넘으면 안된다. 0,0 됨

        if (this.AutoFontSize && this.TextTrimming == TextTrimming.None)
        {
          Size s = new Size(availableSize.Width * this.AutoFontRatio, availableSize.Height * this.AutoFontRatio);
          double fs = this.GetFontSize(s, this.CurrentText, _typeface);

          formattedText.SetFontSize(fs);
          //var size = formattedText.GetSize();

          //LOG.d($"1. FontSize: {fs}, Height: {size.Height}");
          dynamicFontSize = fs;
          //formattedText.SetFontSize(dynamicFontSize);
        }

        formattedText.MaxTextWidth = Math.Min(3579139, availableSize.Width);
        formattedText.MaxTextHeight = Math.Max(0.0001d, availableSize.Height);

        //LOG.ic($"avai size = {availableSize} : formattedText.Height: {formattedText.Width}, {formattedText.Height}");
        return new Size(Math.Ceiling(formattedText.Width), Math.Ceiling(formattedText.Height));
      }
      // return the desired size
      return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      //LOG.ic($"finalSize size = {finalSize} : formattedText.Height: {formattedText?.Height ?? 0}");

      CreateFormattedText();
      if (formattedText != null)
      {
        //LOG.ic($"size = {finalSize} : formattedText exist");
        formattedText.MaxTextWidth = finalSize.Width;
        formattedText.MaxTextHeight = Math.Max(0.0001d, finalSize.Height);

        //LOG.ic($"finalSize size = {finalSize} : formattedText.Height: {formattedText.Height}");
      }
      return finalSize;
    }
    protected override void OnRender(DrawingContext dc)
    {
      Size size = new Size(this.ActualWidth, this.ActualHeight);
      this.draw_background(dc, size);

      // left/right AutoMargin
      // 1. 글자가 작은 경우 가운데로 몰릴수있다.
      // 2. 글자 높이가 50% 이상일때 자동 여백
      if (this.formattedText == null)
      {
        //LOG.wc($"formattedText is null or Height < 50% of ActualHeight");
        return;
      }

      var minw = formattedText.MinWidth;
      var lineheight = formattedText.LineHeight;

      var h = formattedText.Height;
      var w = formattedText.Width;
      var rec = new Rect(formattedText.GetSize());

      //LOG.d($"formattedText.Size: {rec}");

      var xm = (this.ActualWidth - rec.Width)/2;
      var ym = (this.ActualHeight - rec.Height)/2;

      // box 그리기
      //rec.Offset(xm, ym);
      //dc.DrawRectangle(Brushes.Yellow, null, rec);

      dc.DrawText(formattedText, new Point(0, ym));

      // PathFigure

      //dc.DrawGeometry(Fill, null, textGeometry);

      //if (StrokePosition == StrokePosition.Outside)
      //{
      //  dc.PushClip(_clipGeometry);
      //}
      //else if (StrokePosition == StrokePosition.Inside)
      //{
      //  dc.PushClip(textGeometry);
      //}

      //dc.DrawGeometry(null, _Pen, textGeometry);

      //if (StrokePosition == StrokePosition.Outside || StrokePosition == StrokePosition.Inside)
      //{
      //  dc.Pop();
      //}
    }
    void draw_background(DrawingContext dc, Size size)
    {
      //LOG.wc($"size = {size}");
      Rect boundRect = new Rect(size);
      var border = this.BorderThickness;
      Rect innerRect = HelperDeflateRect(boundRect, border);

      CornerRadius cornerRadius = new CornerRadius(this.CornerRadius);
      Brush borderBrush = BorderBrush;

      double outerCornerRadius = cornerRadius.TopLeft; // Already validated that all corners have the same radius
      bool roundedCorners = !DoubleUtil.IsZero(outerCornerRadius);

      // border 그리기
      if (!IsZero(border) && (borderBrush = BorderBrush) != null)
      {
        // Initialize the first pen.  Note that each pen is created via new()
        // and frozen if possible.  Doing this avoids the pen
        // being copied when used in the DrawLine methods.
        Pen pen = new Pen();
        pen.Brush = borderBrush;

        pen.Thickness = border.Left;

        if (borderBrush.IsFrozen)
        {
          pen.Freeze();
        }

        double halfThickness = pen.Thickness * 0.5;

        // Create rect w/ border thickness, and round if applying layout rounding.
        Rect rect = new Rect(new Point(halfThickness, halfThickness),
                                             new Point(RenderSize.Width - halfThickness, RenderSize.Height - halfThickness));

        if (roundedCorners)
        {
          dc.DrawRoundedRectangle(null, pen, rect, outerCornerRadius, outerCornerRadius);
        }
        else
        {
          dc.DrawRectangle(null, pen, rect);
        }
      }

      // Draw background in rectangle inside border.
      Brush background = Background;
      if (background != null)
      {
        // Intialize background
        Point ptTL, ptBR;

        ptTL = new Point(border.Left, border.Top);
        ptBR = new Point(RenderSize.Width - border.Right, RenderSize.Height - border.Bottom);

        // Do not draw background if the borders are so large that they overlap.
        if (ptBR.X > ptTL.X && ptBR.Y > ptTL.Y)
        {
          if (roundedCorners)
          {
            Radii innerRadii = new Radii(cornerRadius, border, false); // Determine the inner edge radius
            double innerCornerRadius = innerRadii.TopLeft;  // Already validated that all corners have the same radius
            dc.DrawRoundedRectangle(background, null, new Rect(ptTL, ptBR), innerCornerRadius, innerCornerRadius);
          }
          else
          {
            dc.DrawRectangle(background, null, new Rect(ptTL, ptBR));
          }
        }
      }

      /*
      // inner border
      {
        // geomerty merge
        // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/how-to-create-a-combined-geometry?view=netframeworkdesktop-4.8

        Radii innerRadii = new Radii(cornerRadius, border, false);
        var backgroundGeometry = GeometryHelper.GenerateGeometry(innerRect, innerRadii);
        //var brush = this.Background;
        //dc.DrawGeometry(brush, null, backgroundGeometry);

        GeometryGroup group = new GeometryGroup() {FillRule = FillRule.EvenOdd};
        group.Children.Add(backgroundGeometry);
        dc.DrawGeometry(Background, null, group);
      }
      */
    }

    protected FormattedText formattedText;
    protected string CurrentText = string.Empty;
    Typeface _typeface;
    private void CreateFormattedText()
    {
      if (formattedText == null)
      {
        if (string.IsNullOrEmpty(this.Text))
        {
          this.CurrentText = "   ";
        }
        else
        {
          this.CurrentText = string.IsNullOrWhiteSpace(this.TextFormat) ? this.Text : string.Format(this.TextFormat, this.Text);
        }
        //LOG.wc($"{this.CurrentText}");

        _typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);

        formattedText = new FormattedText(CurrentText, CultureInfo.CurrentUICulture, FlowDirection,
                                      _typeface, FontSize, Foreground); //, _dpi);

        UpdateFormattedTextOption();
      }
    }

    double dynamicFontSize = 0;
    private void UpdateFormattedTextOption()
    {
      if (dynamicFontSize == 0)
      {
        dynamicFontSize = this.FontSize;
      }

      if (formattedText != null)
      {
        formattedText.MaxLineCount = TextWrapping == TextWrapping.NoWrap ? 1 : int.MaxValue;
        formattedText.TextAlignment = TextAlignment;
        formattedText.Trimming = TextTrimming;

        formattedText.SetFontSize(FontSize);
        formattedText.SetFontStyle(FontStyle);
        formattedText.SetFontWeight(FontWeight);
        formattedText.SetFontFamily(FontFamily);
        formattedText.SetFontStretch(FontStretch);
        formattedText.SetTextDecorations(TextDecorations);

        //LOG.wc($"FontSize: {this.FontSize}, Formatted: H: {formattedText.Height}");
      }
    }
    void TextChanged()
    {
      if (this.IsInitialized)
      {
        //this.CurrentText = string.IsNullOrWhiteSpace(this.TextFormat) ? this.Text : string.Format(this.TextFormat, this.Text);
        ////LOG.ic($"{this.CurrentText}");

        formattedText = null;
        //LOG.ic($"call InvalidateMeasure()");
        InvalidateMeasure();
        //LOG.ic($"call InvalidateVisual()");
        InvalidateVisual();
      }
    }

    #region Text
    public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBorder),
    new FrameworkPropertyMetadata((d,e) =>
    {
      (d as TextBorder)?.TextChanged();
      //if (d is TextBorder o)
      //{
      //  if(o.IsInitialized)
      //    o.TextChanged();
      //}
    }));

    //static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //  if (d is TextBorder o)
    //  {
    //    if(o.IsInitialized)
    //      o.TextChanged();
    //  }
    //}
    // stringFormat 사용: "대기자 {0} 명"
    public string TextFormat { get { return (string)GetValue(TextFormatProperty); } set { SetValue(TextFormatProperty, value); } }

    public static readonly DependencyProperty TextFormatProperty = DependencyProperty.Register(nameof(TextFormat), typeof(string), typeof(TextBorder),
    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
                                          new ValidateValueCallback(IsTextFormatValid));
    private static bool IsTextFormatValid(object value)
    {
      var s = value?.ToString();
      if (!string.IsNullOrWhiteSpace(s))
      {
        return s.IndexOf("{0}") >= 0;
      }
      return true;
    }
    #endregion Text

    #region Option
    static void OnOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //LOG.ic($"{e.Property.Name} : {e.OldValue}  -> {e.NewValue}");
      if (d is TextBorder o)
      {
        if (o.IsInitialized)
        {
          o.UpdateFormattedTextOption();
          o.InvalidateMeasure();
          o.InvalidateVisual();
        }
      }
    }

    #region FONT
    //public FontFamily FontFamily { get { return (FontFamily)GetValue(FontFamilyProperty); } set { SetValue(FontFamilyProperty, value); } }
    //public FontStyle FontStyle { get { return (FontStyle)GetValue(FontStyleProperty); } set { SetValue(FontStyleProperty, value); } }
    //[TypeConverter(typeof(FontSizeConverter))]
    //public double FontSize { get { return (double)GetValue(FontSizeProperty); } set { SetValue(FontSizeProperty, value); } }
    //public FontWeight FontWeight { get { return (FontWeight)GetValue(FontWeightProperty); } set { SetValue(FontWeightProperty, value); } }
    //public FontStretch FontStretch { get { return (FontStretch)GetValue(FontStretchProperty); } set { SetValue(FontStretchProperty, value); } }
    //public Brush Foreground { get { return (Brush)GetValue(ForegroundProperty); } set { SetValue(ForegroundProperty, value); } }

    // DependencyProperty
    //public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(TextBorder), new FrameworkPropertyMetadata(OnOptionChanged));
    //public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(TextBorder), new FrameworkPropertyMetadata(FontStyles.Normal, OnOptionChanged));
    //public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(TextBorder), new FrameworkPropertyMetadata(20d, OnOptionChanged));
    //public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(TextBorder), new FrameworkPropertyMetadata(OnOptionChanged));
    //public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(TextBorder), new FrameworkPropertyMetadata(FontStretches.Normal, OnOptionChanged));
    //public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(nameof(Foreground), typeof(Brush),
    //      typeof(TextBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender, OnOptionChanged));

    public bool AutoFontSize { get { return (bool)GetValue(AutoFontSizeProperty); } set { SetValue(AutoFontSizeProperty, value); } }
    public static readonly DependencyProperty AutoFontSizeProperty = DependencyProperty.Register(nameof(AutoFontSize), typeof(bool), typeof(TextBorder), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// desired size / actual size 비율 (Default 0.6)
    /// </summary>
    public double AutoFontRatio { get { return (double)GetValue(AutoFontRatioProperty); } set { SetValue(AutoFontRatioProperty, value); } }

    public static readonly DependencyProperty AutoFontRatioProperty = DependencyProperty.Register(nameof(AutoFontRatio), typeof(double), typeof(TextBorder), new FrameworkPropertyMetadata(0.6, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion FONT

    #region Text style
    public TextAlignment TextAlignment { get { return (TextAlignment)GetValue(TextAlignmentProperty); } set { SetValue(TextAlignmentProperty, value); } }
    public TextDecorationCollection TextDecorations { get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); } set { SetValue(TextDecorationsProperty, value); } }
    public TextTrimming TextTrimming { get { return (TextTrimming)GetValue(TextTrimmingProperty); } set { SetValue(TextTrimmingProperty, value); } }
    public TextWrapping TextWrapping { get { return (TextWrapping)GetValue(TextWrappingProperty); } set { SetValue(TextWrappingProperty, value); } }

    // DependencyProperty
    public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(nameof(TextAlignment), typeof(TextAlignment), typeof(TextBorder), new FrameworkPropertyMetadata(TextAlignment.Left, OnOptionChanged));
    public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(nameof(TextDecorations), typeof(TextDecorationCollection), typeof(TextBorder), new FrameworkPropertyMetadata(OnOptionChanged));
    public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(nameof(TextTrimming), typeof(TextTrimming), typeof(TextBorder), new FrameworkPropertyMetadata(TextTrimming.CharacterEllipsis, OnOptionChanged));
    public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(TextBorder), new FrameworkPropertyMetadata(TextWrapping.NoWrap, OnOptionChanged));
    #endregion Text style

    #region TextBorder Option
    //public Brush Background { get { return (Brush)GetValue(BackgroundProperty); } set { SetValue(BackgroundProperty, value); } }
    //public Brush BorderBrush { get { return (Brush)GetValue(BorderBrushProperty); } set { SetValue(BorderBrushProperty, value); } }
    //public Thickness BorderThickness { get { return (Thickness)GetValue(BorderThicknessProperty); } set { SetValue(BorderThicknessProperty, value); } }
    public double CornerRadius { get { return (double)GetValue(CornerRadiusProperty); } set { SetValue(CornerRadiusProperty, value); } }

    //  public static readonly DependencyProperty BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(TextBorder), new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
    //public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(TextBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
    //public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(TextBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
    //public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(TextBorder), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(TextBorder), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion TextBorder Option
    #endregion Option

    static double GetDpi()
    {
      return (int)(typeof(SystemParameters).GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null, null) ?? 120) / 96.0;
    }
    static Rect HelperDeflateRect(Rect rt, Thickness thick)
    {
      return new Rect(rt.Left + thick.Left,
                      rt.Top + thick.Top,
                      Math.Max(0.0, rt.Width - thick.Left - thick.Right),
                      Math.Max(0.0, rt.Height - thick.Top - thick.Bottom));
    }
    static bool AreUniformCorners(CornerRadius borderRadii)
    {
      double topLeft = borderRadii.TopLeft;
      return DoubleUtil.AreClose(topLeft, borderRadii.TopRight) &&
          DoubleUtil.AreClose(topLeft, borderRadii.BottomLeft) &&
          DoubleUtil.AreClose(topLeft, borderRadii.BottomRight);
    }

    static bool IsZero(Thickness o)
    {
      return DoubleUtil.IsZero(o.Left)
             && DoubleUtil.IsZero(o.Top)
             && DoubleUtil.IsZero(o.Right)
             && DoubleUtil.IsZero(o.Bottom);
    }
    static bool HasValue(Thickness o)
    {
      return o.Left != 0 || o.Top != 0 || o.Right != 0 || o.Bottom != 0;
    }
  }
}