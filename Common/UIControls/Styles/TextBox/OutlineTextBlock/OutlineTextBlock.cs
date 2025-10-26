using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace UIControls
{
  // https://stackoverflow.com/questions/93650/apply-stroke-to-a-textblock-in-wpf
  public enum StrokePosition
  {
    Center,
    Outside,
    Inside
  }

  [ContentProperty("Text")]
  public class OutlineTextBlock : FrameworkElement
  {
    static OutlineTextBlock()
    {
      // 이 부분이 없으면 기본 style을 찾지 못한다.
      DefaultStyleKeyProperty.OverrideMetadata(typeof(OutlineTextBlock),
       new FrameworkPropertyMetadata(typeof(OutlineTextBlock)));
    }

    public OutlineTextBlock()
    {
      UpdatePen();
      TextDecorations = new TextDecorationCollection();
    }

    protected FormattedText formattedText;
    protected Geometry textGeometry;
    private Pen _Pen;
    private PathGeometry _clipGeometry;

    protected string CurrentText;

    private void UpdatePen()
    {
      _Pen = new Pen(Stroke, StrokeThickness)
      {
        DashCap = PenLineCap.Round,
        EndLineCap = PenLineCap.Round,
        LineJoin = PenLineJoin.Round,
        StartLineCap = PenLineCap.Round
      };

      if (StrokePosition == StrokePosition.Outside || StrokePosition == StrokePosition.Inside)
      {
        _Pen.Thickness = StrokeThickness * 2;
      }

      InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      EnsureGeometry();

      drawingContext.DrawGeometry(Fill, null, textGeometry);

      if (StrokePosition == StrokePosition.Outside)
      {
        drawingContext.PushClip(_clipGeometry);
      }
      else if (StrokePosition == StrokePosition.Inside)
      {
        drawingContext.PushClip(textGeometry);
      }

      drawingContext.DrawGeometry(null, _Pen, textGeometry);

      if (StrokePosition == StrokePosition.Outside || StrokePosition == StrokePosition.Inside)
      {
        drawingContext.Pop();
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      EnsureFormattedText();

      // constrain the formatted text according to the available size

      double w = availableSize.Width;
      double h = availableSize.Height;

      // the Math.Min call is important - without this constraint (which seems arbitrary, but is the maximum allowable text width), things blow up when availableSize is infinite in both directions
      // the Math.Max call is to ensure we don't hit zero, which will cause MaxTextHeight to throw
      formattedText.MaxTextWidth = Math.Min(3579139, w);
      formattedText.MaxTextHeight = Math.Max(0.0001d, h);

      // return the desired size
      return new Size(Math.Ceiling(formattedText.Width), Math.Ceiling(formattedText.Height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      EnsureFormattedText();

      // update the formatted text with the final size
      formattedText.MaxTextWidth = finalSize.Width;
      formattedText.MaxTextHeight = Math.Max(0.0001d, finalSize.Height);

      // need to re-generate the geometry now that the dimensions have changed
      textGeometry = null;
      UpdatePen();

      return finalSize;
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as OutlineTextBlock).TextChanged();
    }
    void TextChanged()
    {
      CurrentText = this.GetContent();
      formattedText = null;
      textGeometry = null;

      InvalidateMeasure();
      InvalidateVisual();
    }
    protected virtual string GetContent()
    {
      return $"{this.Prefix}{this.Text}{this.Postfix}";
    }
    private static void OnOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var tb = (OutlineTextBlock)d;
      tb.UpdateFormattedText();
      tb.textGeometry = null;

      tb.InvalidateMeasure();
      tb.InvalidateVisual();
    }

    private void EnsureFormattedText()
    {
      if (formattedText != null)
      {
        return;
      }

      formattedText = new FormattedText(
        CurrentText ?? "",
        CultureInfo.CurrentUICulture,
        FlowDirection,
        new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
        FontSize,
        Brushes.Black); //, VisualTreeHelper.GetDpi(this).PixelsPerDip);

      UpdateFormattedText();
    }

    private void UpdateFormattedText()
    {
      if (formattedText == null)
      {
        return;
      }

      formattedText.MaxLineCount = TextWrapping == TextWrapping.NoWrap ? 1 : int.MaxValue;
      formattedText.TextAlignment = TextAlignment;
      formattedText.Trimming = TextTrimming;

      formattedText.SetFontSize(FontSize);
      formattedText.SetFontStyle(FontStyle);
      formattedText.SetFontWeight(FontWeight);
      formattedText.SetFontFamily(FontFamily);
      formattedText.SetFontStretch(FontStretch);
      formattedText.SetTextDecorations(TextDecorations);
    }

    private void EnsureGeometry()
    {
      if (textGeometry != null)
      {
        return;
      }

      EnsureFormattedText();
      textGeometry = formattedText.BuildGeometry(new Point(0, 0));

      if (StrokePosition == StrokePosition.Outside)
      {
        var boundsGeo = new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight));
        _clipGeometry = Geometry.Combine(boundsGeo, textGeometry, GeometryCombineMode.Exclude, null);
      }
    }

    #region DP

    #region StrokePosition
    public StrokePosition StrokePosition
    {
      get { return (StrokePosition)GetValue(StrokePositionProperty); }
      set { SetValue(StrokePositionProperty, value); }
    }

    public static readonly DependencyProperty StrokePositionProperty = DependencyProperty.Register(
      "StrokePosition", typeof(StrokePosition), typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(StrokePosition.Outside, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Fill
    public Brush Fill
    {
      get { return (Brush)GetValue(FillProperty); }
      set { SetValue(FillProperty, value); }
    }
    public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
      "Fill",
      typeof(Brush),
      typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Stroke
    public Brush Stroke
    {
      get { return (Brush)GetValue(StrokeProperty); }
      set { SetValue(StrokeProperty, value); }
    }
    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
      "Stroke", typeof(Brush), typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region StrokeThickness
    public double StrokeThickness
    {
      get { return (double)GetValue(StrokeThicknessProperty); }
      set { SetValue(StrokeThicknessProperty, value); }
    }
    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
      "StrokeThickness", typeof(double), typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region FontFamily
    public FontFamily FontFamily
    {
      get { return (FontFamily)GetValue(FontFamilyProperty); }
      set { SetValue(FontFamilyProperty, value); }
    }
    public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(
      typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(OnOptionChanged));
    #endregion

    #region FontSize
    [TypeConverter(typeof(FontSizeConverter))]
    public double FontSize
    {
      get { return (double)GetValue(FontSizeProperty); }
      set { SetValue(FontSizeProperty, value); }
    }
    public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(
      typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(OnOptionChanged));
    #endregion

    #region FontStretch
    public FontStretch FontStretch
    {
      get { return (FontStretch)GetValue(FontStretchProperty); }
      set { SetValue(FontStretchProperty, value); }
    }
    public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(
      typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(OnOptionChanged));
    #endregion

    #region FontStyle
    public FontStyle FontStyle
    {
      get { return (FontStyle)GetValue(FontStyleProperty); }
      set { SetValue(FontStyleProperty, value); }
    }
    public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(
      typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(OnOptionChanged));
    #endregion

    #region FontWeight
    public FontWeight FontWeight
    {
      get { return (FontWeight)GetValue(FontWeightProperty); }
      set { SetValue(FontWeightProperty, value); }
    }
    public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(
      typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(OnOptionChanged));
    #endregion

    #region Text
    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
      "Text", typeof(string), typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(OnTextChanged));
    #endregion

    #region FontAlignment
    public TextAlignment TextAlignment
    {
      get { return (TextAlignment)GetValue(TextAlignmentProperty); }
      set { SetValue(TextAlignmentProperty, value); }
    }
    public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
      "TextAlignment", typeof(TextAlignment), typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(OnOptionChanged));
    #endregion

    #region TextDecorations
    public TextDecorationCollection TextDecorations
    {
      get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
      set { SetValue(TextDecorationsProperty, value); }
    }
    public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(
      "TextDecorations", typeof(TextDecorationCollection), typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(OnOptionChanged));
    #endregion

    #region TextTrimming
    public TextTrimming TextTrimming
    {
      get { return (TextTrimming)GetValue(TextTrimmingProperty); }
      set { SetValue(TextTrimmingProperty, value); }
    }
    public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
      "TextTrimming", typeof(TextTrimming), typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(OnOptionChanged));
    #endregion

    #region TextWrapping
    public TextWrapping TextWrapping
    {
      get { return (TextWrapping)GetValue(TextWrappingProperty); }
      set { SetValue(TextWrappingProperty, value); }
    }
    public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
      "TextWrapping", typeof(TextWrapping), typeof(OutlineTextBlock),
      new FrameworkPropertyMetadata(TextWrapping.NoWrap, OnOptionChanged));
    #endregion

    #endregion

    #region DP2
    public string Prefix
    {
      get { return (string)GetValue(PrefixProperty); }
      set { SetValue(PrefixProperty, value); }
    }

    public static readonly DependencyProperty PrefixProperty =
        DependencyProperty.Register("Prefix", typeof(string), typeof(OutlineTextBlock),
          new PropertyMetadata(defaultValue:null));


    public string Postfix
    {
      get { return (string)GetValue(PostfixProperty); }
      set { SetValue(PostfixProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Postfix.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PostfixProperty =
        DependencyProperty.Register("Postfix", typeof(string), typeof(OutlineTextBlock),
          new PropertyMetadata(defaultValue:null));
    #endregion
  }
}
