using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UIControls
{
  public static class ScreenHelper
  {
    public static Typeface GetTypeFace(this Control tb) => new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
    public static Size MeasureSize(this FormattedText f) => new Size(f.Width, f.Height);
    public static Size MeasureText(this Control tb, string message)
    {
      var ft = new FormattedText(message, CultureInfo.CurrentUICulture,
                                 FlowDirection.LeftToRight, tb.GetTypeFace(),
                                 tb.FontSize, Brushes.Black);
      return ft.MeasureSize();
    }
    public static double GetFitFontSize(Size bounds, string s, double max_fontsize, Typeface tf, double dpi)
    {
      var ft = new FormattedText(s, CultureInfo.CurrentUICulture,
                   FlowDirection.LeftToRight, tf, max_fontsize, Brushes.Black);

      while (true)
      {
        ft.SetFontSize(max_fontsize);
        Size size = ft.MeasureSize();
        if (size.Width < bounds.Width && size.Height < bounds.Height)
        {
          return max_fontsize;
        }
        max_fontsize--;
        if (max_fontsize <= 0)
          return max_fontsize;
      }
    }

    #region DPI
    public static DpiScale GetDpiScale(this Visual visual)
    {
      PresentationSource source = PresentationSource.FromVisual(visual);
      if (source != null)
      {
        // Get the composition target for the source
        CompositionTarget compositionTarget = source.CompositionTarget;
        // Get the transform from device pixels to independent pixels
        Matrix transform = compositionTarget.TransformToDevice;
        // 96 dpi일때 factor
        return new DpiScale
        {
          DpiScaleX = transform.M11,
          DpiScaleY = transform.M22,
          PixelsPerDip = transform.M11,
          PixelsPerInchX = transform.M11 * 96,
          PixelsPerInchY = transform.M22 * 96,
        };
      }
      // Default to 1.0 (96 DPI) if source is not found
      return new DpiScale();
    }
    public static double GetPixelsPerDip(this Visual v) => GetDpiScale(v).PixelsPerDip;
    #region win32
    /*
    public static double GetPixelsPerDip(this Visual v)
    {
      return GetSystemDpi().PixelsPerDip;
    }
    public static double GetDpi()
    {
      return (int)(typeof(SystemParameters).GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null, null) ?? 120) / 96.0; // 1
    }

    [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("gdi32.dll")] private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
    const int LOGPIXELSX = 88; // Logical pixels per inch in X-axis
    const int LOGPIXELSY = 90; // Logical pixels per inch in Y-axis

    static DpiScale GetSystemDpi()
    {
      IntPtr hdc = GetDC(IntPtr.Zero);
      if (hdc == IntPtr.Zero)
      {
        throw new InvalidOperationException("Could not get device context.");
      }
      try
      {
        int dpiX = GetDeviceCaps(hdc, LOGPIXELSX);  // 96
        int dpiY = GetDeviceCaps(hdc, LOGPIXELSY);  // 96
        // WPF's default DPI is 96
        return new DpiScale
        {
          DpiScaleX = dpiX / 96,
          DpiScaleY = dpiY / 96,
          PixelsPerDip = dpiX / 96.0,
          PixelsPerInchX = dpiX,
          PixelsPerInchY = dpiY
        };
      }
      catch (Exception)
      {
        return new DpiScale();
      }
      finally
      {
        ReleaseDC(IntPtr.Zero, hdc);
      }
    }
    */
    #endregion
    #endregion
  }

  public class DpiScale
  {
    public double DpiScaleX = 1;
    public double DpiScaleY = 1;
    public double PixelsPerDip = 1;
    public double PixelsPerInchX = 96;
    public double PixelsPerInchY = 96;
  }
}
