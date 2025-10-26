using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UIControls
{
  /*
  public class WpfHelper
  {
    public static Size MeasureText(Label tb) => MeasureText(tb.Content?.ToString() ?? string.Empty, tb);
    public static Size MeasureText(string s, Label tb)
    {
      var tf = new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
      var fontsize = tb.FontSize;
      double dpi = VisualTreeHelper.GetDpi(tb).PixelsPerDip;
      return MeasureText(s, fontsize, dpi, tf);
    }
    public static Size MeasureText(TextBlock tb) => MeasureText(tb.Text, tb);
    public static Size MeasureText(string s, TextBlock tb)
    {
      var tf = new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
      var fontsize = tb.FontSize;
      double dpi = VisualTreeHelper.GetDpi(tb).PixelsPerDip;

      return MeasureText(s, fontsize, dpi, tf);
    }
    public static double PixelsPerDpi(Visual v) => VisualTreeHelper.GetDpi(v).PixelsPerDip;
    public static Size MeasureText(string s, double fontsize, double dpi, Typeface tf)
    {
      var formattedText = new FormattedText(s, CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight, tf,fontsize, Brushes.Black, dpi);
      return new Size(formattedText.Width, formattedText.Height);
    }
    public static Typeface GetTypeFace(Label tb) => new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
    public static Typeface GetTypeFace(TextBlock tb) => new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
    public static Typeface GetTypeFace(TextBox tb) => new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
    public static double CalcFontSize(string text, double dpi, Size size, Typeface tf, double start_fontsize = 200)
    {
      while (true)
      {
        Size s = MeasureText(text, start_fontsize, dpi, tf);
        if (s.Width < size.Width && s.Height < size.Height)
        {
          return start_fontsize;
        }
        start_fontsize--;
        if (start_fontsize <= 0)
          return start_fontsize;
      }
    }
  } */
}