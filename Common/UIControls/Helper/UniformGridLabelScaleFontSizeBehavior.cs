/*
using Microsoft.Xaml.Behaviors;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UIControls;

public class UniformGridLabelScaleFontSizeBehavior : Behavior<UniformGrid>
{
  // MaxFontSize
  public double MaxFontSize
  {
    get { return (double)GetValue(MaxFontSizeProperty); }
    set { SetValue(MaxFontSizeProperty, value); }
  }

  public static readonly DependencyProperty MaxFontSizeProperty = DependencyProperty
  .Register("MaxFontSize", typeof(double), typeof(UniformGridLabelScaleFontSizeBehavior), new PropertyMetadata(20d));

  protected override void OnAttached()
  {
    //this.AssociatedObject.SizeChanged += (s, e) => { CalculateFontSize(); };
    this.AssociatedObject.LayoutUpdated += (s, e) => { CalculateFontSize(); };
  }

  private void AssociatedObject_LayoutUpdated(object sender, EventArgs e)
  {
    throw new NotImplementedException();
  }

  private void CalculateFontSize()
  {
    double fontSize = this.MaxFontSize;

    List<Label> childs = VisualHelper.FindVisualChildren<Label>(this.AssociatedObject);

    double cellHeight = this.AssociatedObject.ActualHeight / this.AssociatedObject.Rows;
    double cellWidth = this.AssociatedObject.ActualWidth / this.AssociatedObject.Columns;

    var desiredSize = new Size(cellWidth * 1.1, cellHeight* 1.1);

    //var tb = childs[0];
    //Size desiredSize = MeasureText(tb);

    double widthMargins = 25; // tb.Margin.Left + tb.Margin.Right;
    double heightMargins = 25; // tb.Margin.Top + tb.Margin.Bottom;

    double desiredHeight = desiredSize.Height + heightMargins;
    double desiredWidth = desiredSize.Width + widthMargins;

    // adjust fontsize if text would be clipped vertically
    if (cellHeight < desiredHeight)
    {
      double factor = (desiredHeight - heightMargins) / (cellHeight - heightMargins);
      fontSize = Math.Min(fontSize, MaxFontSize / factor);
    }

    if (cellWidth < desiredWidth)
    {
      double factor = (desiredWidth - widthMargins) / (cellWidth - widthMargins);
      fontSize = Math.Min(fontSize, MaxFontSize / factor);
    }

    // apply fontsize (always equal fontsizes)
    foreach (var p in childs)
    {
      p.FontSize = fontSize;
    }
  }

  private Size MeasureText(Label tb)
  {
    var formattedText = new FormattedText("AAAAAAA", CultureInfo.CurrentUICulture,
    FlowDirection.LeftToRight,
    new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
    this.MaxFontSize, Brushes.Black); // always uses MaxFontSize for desiredSize

    return new Size(formattedText.Width, formattedText.Height);
  }
}
*/