using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using WpfScreenHelper;

namespace UIControls
{
  public static class DialogHelper
  {
    public static int GetCurrentDPI()
    {
      return (int)typeof(SystemParameters).GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
    }

    public static double GetCurrentDPIScaleFactor()
    {
      return (double)GetCurrentDPI() / 96;
    }

    public static Point GetCenterOwnerPoint(Window owner, Window child)
    {
      var x = (owner.Width - child.Width) / 2;
      var y = (owner.Height - child.Height) / 2;

      return new Point(owner.Left + x, owner.Top + y);
    }

    public static Rect VirtualScreen()
    {
      var left = SystemParameters.VirtualScreenLeft;
      var top = SystemParameters.VirtualScreenTop;
      var w = SystemParameters.VirtualScreenWidth;
      var h = SystemParameters.VirtualScreenHeight;

      return new Rect(new Point(left, top), new Size(w, h));
    }
    public static Screen GetCurrentScreen(Window w)
    {
      return Screen.FromHandle(new WindowInteropHelper(w).Handle);
    }
    /// <summary>
    /// 윈도우를 r 위치로 이동하고자 할때 가능한가 여부 확인
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public static Point GetAdjustLocation(Rect r)
    {
      int titleHeight = 32;
      var vs = VirtualScreen();
      // titlebar가 화면에 보일수만 있으면 OK

      if (r.Y < 0)
      {
        r.Y = 0;
      }
      else if (r.Y >= (vs.Bottom - titleHeight))
      {
        r.Y = vs.Bottom - r.Height;
      }

      if (r.X >= (vs.Right - 100))
      {
        r.X = vs.Right - 100;
      }
      else if (r.X < vs.Left)
      {
        r.X = vs.Left;
      }

      List<Rect> all = new List<Rect>();
      foreach (var screen in Screen.AllScreens)
      {
        var rect = screen.WorkingArea;
        var r2 = new Point(r.X, 0); // y가 벗어날 수 있다.

        if (rect.Contains(r2))
        {
          //Debug.WriteLine($"screen={rect}, r={r}");
          if (!rect.Contains(r))
          {
            var centerX = rect.X + rect.Width / 2;
            var centerY = rect.Y + rect.Height / 2;

            if (r.X < centerX)
            {
              if (r.X + r.Width > rect.Right)
                r.X = rect.X;
            }
            else
            {
              if (r.X + r.Width > rect.Right)
                r.X = rect.Right - r.Width;
            }

            if (r.Y < centerY)
            {
              if (r.Y + r.Height > rect.Bottom)
                r.Y = rect.Y;
            }
            else
            {
              if (r.Y + r.Height > rect.Bottom)
                r.Y = rect.Bottom - r.Height;
            }
          }
          break;
        }
      }

      return r.Location;
    }
  }
}