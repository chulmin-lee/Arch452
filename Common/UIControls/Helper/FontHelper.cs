using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UIControls
{
  public static class FontHelper
  {
    public static Typeface GetTypeface(this TextBlock tb) => new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
    public static Typeface GetTypeface(this System.Windows.Controls.Control tb) => new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);

    /// <summary>
    /// 현재의 Typeface, text, fontsize 일때 문자열 길이
    /// </summary>
    public static Size MeasureText(this TextBlock tb) => measure_text(tb.Text, tb.GetTypeface(), tb.FontSize);
    /// <summary>
    /// 현재의 Typeface, text 일때 지정된 font size 일때 문자열 길이
    /// </summary>
    public static Size MeasureText(this TextBlock tb, double fontsize) => measure_text(tb.Text, tb.GetTypeface(), fontsize);
    public static Size MeasureText(this Label tb, double fontsize) => measure_text(tb.Content.ToString(), tb.GetTypeface(), fontsize);

    /// <summary>
    /// 현재의 Typeface 일때, text, fontsize 에 따른 문자열 길이
    /// </summary>
    public static Size MeasureText(this TextBlock tb, string s, double fontsize) => measure_text(s ?? tb.Text, tb.GetTypeface(), fontsize);
    public static Size MeasureText(this Label tb, string s, double fontsize) => measure_text(s ?? tb.Content.ToString(), tb.GetTypeface(), fontsize);

    static Size measure_text(string s, Typeface face, double fontsize)
    {
      var formattedText = new FormattedText(s, CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight, face, fontsize, Brushes.Black);
      return new Size(formattedText.Width, formattedText.Height);
    }

    /// <summary>
    /// 부모 control 크기에 따른 폰트 크기 조절
    /// MaxFontSize 보다 작게 구하므로 최대한 큰값을 전달할것
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    /// <param name="len">문자열 길이</param>
    /// <param name="max_font_size"></param>
    /// <returns></returns>
    public static double ParentFitFontSize(FrameworkElement parent, TextBlock child, int len = 0, double max_font_size = 200)
    {
      double fontSize = (max_font_size == 0) ? child.FontSize : max_font_size;
      string text = (len > 0) ? new string('A', len) : child.Text;
      Size size = child.MeasureText(text, fontSize);
      return calc_parent_fit_font_size(parent, child, size, fontSize);
    }
    public static double ParentFitFontSize(FrameworkElement parent, Label child, int len = 0, double max_font_size = 200)
    {
      double fontSize = (max_font_size == 0) ? child.FontSize : max_font_size;
      string text = (len > 0) ? new string('A', len) : child.Content.ToString();
      Size size = child.MeasureText(text, fontSize);
      return calc_parent_fit_font_size(parent, child, size, fontSize);
    }
    static double calc_parent_fit_font_size(FrameworkElement parent, FrameworkElement child, Size size, double MaxFontSize)
    {
      double fontSize = MaxFontSize;
      double HEIGHT = parent.ActualHeight;
      double WIDTH = parent.ActualWidth;

      // child
      double margin_width = child.Margin.Left + child.Margin.Right;
      double margin_height = child.Margin.Top + child.Margin.Bottom;

      double calc_height = size.Height + margin_height;
      double calc_width = size.Width + margin_width;

      // adjust fontsize if text would be clipped vertically
      if (HEIGHT < calc_height)
      {
        double factor = (calc_height - margin_height) / (parent.ActualHeight - margin_height);
        fontSize = Math.Min(fontSize, MaxFontSize / factor);
      }

      // adjust fontsize if text would be clipped horizontally
      if (WIDTH < calc_width)
      {
        double factor = (calc_width - margin_width) / (WIDTH - margin_width);
        fontSize = Math.Min(fontSize, MaxFontSize / factor);
      }
      return fontSize;
    }
  }
}