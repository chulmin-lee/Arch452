using System.Windows;
using System.Windows.Media;

namespace UIControls
{
  public static class FormattedTextExtension
  {
    public static Size GetSize(this FormattedText o)
    {
      return new Size(o.Width, o.Height);
    }
  }
}