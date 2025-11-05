using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UIControls
{
  public static class ScreenHelper
  {
    public static Typeface GetTypeFace(this Control tb) => new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
    public static Size MeasureText(this Control tb, string message)
    {
      var formattedText = new FormattedText(message, CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight, tb.GetTypeFace(),
        tb.FontSize, Brushes.Black);
      return new Size(formattedText.Width, formattedText.Height);
    }

  }
}
