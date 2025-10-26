/*
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace UIControls
{
  public class BorderTextBlockScaleFontBehavior : Behavior<Border>
  {
    public int MaxStringLen { get { return (int)GetValue(MaxStringLenProperty); } set { SetValue(MaxStringLenProperty, value); } }
    public static readonly DependencyProperty MaxStringLenProperty = DependencyProperty
          .Register("MaxStringLen", typeof(int), typeof(BorderTextBlockScaleFontBehavior), new PropertyMetadata(5));

    public double MaxFontSize { get { return (double)GetValue(MaxFontSizeProperty); } set { SetValue(MaxFontSizeProperty, value); } }
    public static readonly DependencyProperty MaxFontSizeProperty = DependencyProperty
           .Register("MaxFontSize", typeof(double), typeof(BorderTextBlockScaleFontBehavior), new PropertyMetadata(200d));

    protected override void OnAttached()
    {
      this.AssociatedObject.SizeChanged += (s, e) => { CalculateFontSize(); };
    }

    private void CalculateFontSize()
    {
      TextBlock tb = VisualHelper.FindVisualChildren<TextBlock>(this.AssociatedObject).FirstOrDefault();
      var fontsize = FontHelper.ParentFitFontSize(this.AssociatedObject, tb, this.MaxStringLen, this.MaxFontSize);
      tb.FontSize = fontsize;
    }
  }
}
*/