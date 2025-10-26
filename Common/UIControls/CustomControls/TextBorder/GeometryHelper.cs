using System;
using System.Windows;
using System.Windows.Media;

namespace UIControls
{
  public static class GeometryHelper
  {
    public static StreamGeometry GenerateGeometry(Rect rect, Radii radii)
    {
      StreamGeometry geo = new StreamGeometry();

      using (StreamGeometryContext ctx = geo.Open())
      {
        //
        //  compute the coordinates of the key points
        //

        Point topLeft = new Point(radii.LeftTop, 0);
        Point topRight = new Point(rect.Width - radii.RightTop, 0);
        Point rightTop = new Point(rect.Width, radii.TopRight);
        Point rightBottom = new Point(rect.Width, rect.Height - radii.BottomRight);
        Point bottomRight = new Point(rect.Width - radii.RightBottom, rect.Height);
        Point bottomLeft = new Point(radii.LeftBottom, rect.Height);
        Point leftBottom = new Point(0, rect.Height - radii.BottomLeft);
        Point leftTop = new Point(0, radii.TopLeft);

        //
        //  check keypoints for overlap and resolve by partitioning radii according to
        //  the percentage of each one.
        //

        //  top edge is handled here
        if (topLeft.X > topRight.X)
        {
          double v = (radii.LeftTop) / (radii.LeftTop + radii.RightTop) * rect.Width;
          topLeft.X = v;
          topRight.X = v;
        }

        //  right edge
        if (rightTop.Y > rightBottom.Y)
        {
          double v = (radii.TopRight) / (radii.TopRight + radii.BottomRight) * rect.Height;
          rightTop.Y = v;
          rightBottom.Y = v;
        }

        //  bottom edge
        if (bottomRight.X < bottomLeft.X)
        {
          double v = (radii.LeftBottom) / (radii.LeftBottom + radii.RightBottom) * rect.Width;
          bottomRight.X = v;
          bottomLeft.X = v;
        }

        // left edge
        if (leftBottom.Y < leftTop.Y)
        {
          double v = (radii.TopLeft) / (radii.TopLeft + radii.BottomLeft) * rect.Height;
          leftBottom.Y = v;
          leftTop.Y = v;
        }

        //
        //  add on offsets
        //

        Vector offset = new Vector(rect.TopLeft.X, rect.TopLeft.Y);
        topLeft += offset;
        topRight += offset;
        rightTop += offset;
        rightBottom += offset;
        bottomRight += offset;
        bottomLeft += offset;
        leftBottom += offset;
        leftTop += offset;

        //
        //  create the border geometry
        //
        ctx.BeginFigure(topLeft, true /* is filled */, true /* is closed */);

        // Top line
        ctx.LineTo(topRight, true /* is stroked */, false /* is smooth join */);

        // Upper-right corner
        double radiusX = rect.TopRight.X - topRight.X;
        double radiusY = rightTop.Y - rect.TopRight.Y;
        if (!DoubleUtil.IsZero(radiusX)
            || !DoubleUtil.IsZero(radiusY))
        {
          ctx.ArcTo(rightTop, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
        }

        // Right line
        ctx.LineTo(rightBottom, true /* is stroked */, false /* is smooth join */);

        // Lower-right corner
        radiusX = rect.BottomRight.X - bottomRight.X;
        radiusY = rect.BottomRight.Y - rightBottom.Y;
        if (!DoubleUtil.IsZero(radiusX)
            || !DoubleUtil.IsZero(radiusY))
        {
          ctx.ArcTo(bottomRight, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
        }

        // Bottom line
        ctx.LineTo(bottomLeft, true /* is stroked */, false /* is smooth join */);

        // Lower-left corner
        radiusX = bottomLeft.X - rect.BottomLeft.X;
        radiusY = rect.BottomLeft.Y - leftBottom.Y;
        if (!DoubleUtil.IsZero(radiusX)
            || !DoubleUtil.IsZero(radiusY))
        {
          ctx.ArcTo(leftBottom, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
        }

        // Left line
        ctx.LineTo(leftTop, true /* is stroked */, false /* is smooth join */);

        // Upper-left corner
        radiusX = topLeft.X - rect.TopLeft.X;
        radiusY = leftTop.Y - rect.TopLeft.Y;
        if (!DoubleUtil.IsZero(radiusX)
            || !DoubleUtil.IsZero(radiusY))
        {
          ctx.ArcTo(topLeft, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
        }
      }
      geo.Freeze();
      return geo;
    }
  }

  public struct Radii
  {
    public Radii(CornerRadius radii, Thickness borders, bool outer)
    {
      double left     = 0.5 * borders.Left;
      double top      = 0.5 * borders.Top;
      double right    = 0.5 * borders.Right;
      double bottom   = 0.5 * borders.Bottom;

      if (outer)
      {
        if (DoubleUtil.IsZero(radii.TopLeft))
        {
          LeftTop = TopLeft = 0.0;
        }
        else
        {
          LeftTop = radii.TopLeft + left;
          TopLeft = radii.TopLeft + top;
        }
        if (DoubleUtil.IsZero(radii.TopRight))
        {
          TopRight = RightTop = 0.0;
        }
        else
        {
          TopRight = radii.TopRight + top;
          RightTop = radii.TopRight + right;
        }
        if (DoubleUtil.IsZero(radii.BottomRight))
        {
          RightBottom = BottomRight = 0.0;
        }
        else
        {
          RightBottom = radii.BottomRight + right;
          BottomRight = radii.BottomRight + bottom;
        }
        if (DoubleUtil.IsZero(radii.BottomLeft))
        {
          BottomLeft = LeftBottom = 0.0;
        }
        else
        {
          BottomLeft = radii.BottomLeft + bottom;
          LeftBottom = radii.BottomLeft + left;
        }
      }
      else
      {
        LeftTop = Math.Max(0.0, radii.TopLeft - left);
        TopLeft = Math.Max(0.0, radii.TopLeft - top);
        TopRight = Math.Max(0.0, radii.TopRight - top);
        RightTop = Math.Max(0.0, radii.TopRight - right);
        RightBottom = Math.Max(0.0, radii.BottomRight - right);
        BottomRight = Math.Max(0.0, radii.BottomRight - bottom);
        BottomLeft = Math.Max(0.0, radii.BottomLeft - bottom);
        LeftBottom = Math.Max(0.0, radii.BottomLeft - left);
      }
    }

    internal double LeftTop;
    internal double TopLeft;
    internal double TopRight;
    internal double RightTop;
    internal double RightBottom;
    internal double BottomRight;
    internal double BottomLeft;
    internal double LeftBottom;
  }

  public static class DoubleUtil
  {
    // Const values come from sdk\inc\crt\float.h
    internal const double DBL_EPSILON  =   2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */
    internal const float  FLT_MIN      =   1.175494351e-38F; /* Number close to zero, where float.MinValue is -float.MaxValue */

    /// <summary>
    /// AreClose - Returns whether or not two doubles are "close".  That is, whether or
    /// not they are within epsilon of each other.  Note that this epsilon is proportional
    /// to the numbers themselves to that AreClose survives scalar multiplication.
    /// There are plenty of ways for this to return false even for numbers which
    /// are theoretically identical, so no code calling this should fail to work if this
    /// returns false.  This is important enough to repeat:
    /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
    /// used for optimizations *only*.
    /// </summary>
    /// <returns>
    /// bool - the result of the AreClose comparision.
    /// </returns>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool AreClose(double value1, double value2)
    {
      //in case they are Infinities (then epsilon check does not work)
      if (value1 == value2) return true;
      // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
      double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
      double delta = value1 - value2;
      return (-eps < delta) && (eps > delta);
    }

    /// <summary>
    /// LessThan - Returns whether or not the first double is less than the second double.
    /// That is, whether or not the first is strictly less than *and* not within epsilon of
    /// the other number.  Note that this epsilon is proportional to the numbers themselves
    /// to that AreClose survives scalar multiplication.  Note,
    /// There are plenty of ways for this to return false even for numbers which
    /// are theoretically identical, so no code calling this should fail to work if this
    /// returns false.  This is important enough to repeat:
    /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
    /// used for optimizations *only*.
    /// </summary>
    /// <returns>
    /// bool - the result of the LessThan comparision.
    /// </returns>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool LessThan(double value1, double value2)
    {
      return (value1 < value2) && !AreClose(value1, value2);
    }

    /// <summary>
    /// GreaterThan - Returns whether or not the first double is greater than the second double.
    /// That is, whether or not the first is strictly greater than *and* not within epsilon of
    /// the other number.  Note that this epsilon is proportional to the numbers themselves
    /// to that AreClose survives scalar multiplication.  Note,
    /// There are plenty of ways for this to return false even for numbers which
    /// are theoretically identical, so no code calling this should fail to work if this
    /// returns false.  This is important enough to repeat:
    /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
    /// used for optimizations *only*.
    /// </summary>
    /// <returns>
    /// bool - the result of the GreaterThan comparision.
    /// </returns>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool GreaterThan(double value1, double value2)
    {
      return (value1 > value2) && !AreClose(value1, value2);
    }

    /// <summary>
    /// GreaterThanZero - Returns whether or not the value is greater than zero
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GreaterThanZero(double value)
    {
      // return value > 0 && !IsZero(value)
      // = value > 0 && !(Math.Abs(value) < 10.0 * DBL_EPSILON)
      // = !(value < 10.0 * DBL_EPSILON)
      return value >= 10.0 * DBL_EPSILON;
    }

    /// <summary>
    /// LessThanOrClose - Returns whether or not the first double is less than or close to
    /// the second double.  That is, whether or not the first is strictly less than or within
    /// epsilon of the other number.  Note that this epsilon is proportional to the numbers
    /// themselves to that AreClose survives scalar multiplication.  Note,
    /// There are plenty of ways for this to return false even for numbers which
    /// are theoretically identical, so no code calling this should fail to work if this
    /// returns false.  This is important enough to repeat:
    /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
    /// used for optimizations *only*.
    /// </summary>
    /// <returns>
    /// bool - the result of the LessThanOrClose comparision.
    /// </returns>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool LessThanOrClose(double value1, double value2)
    {
      return (value1 < value2) || AreClose(value1, value2);
    }

    /// <summary>
    /// GreaterThanOrClose - Returns whether or not the first double is greater than or close to
    /// the second double.  That is, whether or not the first is strictly greater than or within
    /// epsilon of the other number.  Note that this epsilon is proportional to the numbers
    /// themselves to that AreClose survives scalar multiplication.  Note,
    /// There are plenty of ways for this to return false even for numbers which
    /// are theoretically identical, so no code calling this should fail to work if this
    /// returns false.  This is important enough to repeat:
    /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
    /// used for optimizations *only*.
    /// </summary>
    /// <returns>
    /// bool - the result of the GreaterThanOrClose comparision.
    /// </returns>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool GreaterThanOrClose(double value1, double value2)
    {
      return (value1 > value2) || AreClose(value1, value2);
    }

    /// <summary>
    /// IsOne - Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
    /// but this is faster.
    /// </summary>
    /// <returns>
    /// bool - the result of the AreClose comparision.
    /// </returns>
    /// <param name="value"> The double to compare to 1. </param>
    public static bool IsOne(double value)
    {
      return Math.Abs(value - 1.0) < 10.0 * DBL_EPSILON;
    }

    /// <summary>
    /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
    /// but this is faster.
    /// </summary>
    /// <returns>
    /// bool - the result of the AreClose comparision.
    /// </returns>
    /// <param name="value"> The double to compare to 0. </param>
    public static bool IsZero(double value)
    {
      return Math.Abs(value) < 10.0 * DBL_EPSILON;
    }

    // The Point, Size, Rect and Matrix class have moved to WinCorLib.  However, we provide
    // internal AreClose methods for our own use here.

    /// <summary>
    /// Compares two points for fuzzy equality.  This function
    /// helps compensate for the fact that double values can
    /// acquire error when operated upon
    /// </summary>
    /// <param name='point1'>The first point to compare</param>
    /// <param name='point2'>The second point to compare</param>
    /// <returns>Whether or not the two points are equal</returns>
    public static bool AreClose(Point point1, Point point2)
    {
      return DoubleUtil.AreClose(point1.X, point2.X) &&
      DoubleUtil.AreClose(point1.Y, point2.Y);
    }

    /// <summary>
    /// Compares two Size instances for fuzzy equality.  This function
    /// helps compensate for the fact that double values can
    /// acquire error when operated upon
    /// </summary>
    /// <param name='size1'>The first size to compare</param>
    /// <param name='size2'>The second size to compare</param>
    /// <returns>Whether or not the two Size instances are equal</returns>
    public static bool AreClose(Size size1, Size size2)
    {
      return DoubleUtil.AreClose(size1.Width, size2.Width) &&
             DoubleUtil.AreClose(size1.Height, size2.Height);
    }

    /// <summary>
    /// Compares two Vector instances for fuzzy equality.  This function
    /// helps compensate for the fact that double values can
    /// acquire error when operated upon
    /// </summary>
    /// <param name='vector1'>The first Vector to compare</param>
    /// <param name='vector2'>The second Vector to compare</param>
    /// <returns>Whether or not the two Vector instances are equal</returns>
    public static bool AreClose(System.Windows.Vector vector1, System.Windows.Vector vector2)
    {
      return DoubleUtil.AreClose(vector1.X, vector2.X) &&
             DoubleUtil.AreClose(vector1.Y, vector2.Y);
    }

    /// <summary>
    /// Compares two rectangles for fuzzy equality.  This function
    /// helps compensate for the fact that double values can
    /// acquire error when operated upon
    /// </summary>
    /// <param name='rect1'>The first rectangle to compare</param>
    /// <param name='rect2'>The second rectangle to compare</param>
    /// <returns>Whether or not the two rectangles are equal</returns>
    public static bool AreClose(Rect rect1, Rect rect2)
    {
      // If they're both empty, don't bother with the double logic.
      if (rect1.IsEmpty)
      {
        return rect2.IsEmpty;
      }

      // At this point, rect1 isn't empty, so the first thing we can test is
      // rect2.IsEmpty, followed by property-wise compares.

      return (!rect2.IsEmpty) &&
          DoubleUtil.AreClose(rect1.X, rect2.X) &&
          DoubleUtil.AreClose(rect1.Y, rect2.Y) &&
          DoubleUtil.AreClose(rect1.Height, rect2.Height) &&
          DoubleUtil.AreClose(rect1.Width, rect2.Width);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static bool IsBetweenZeroAndOne(double val)
    {
      return (GreaterThanOrClose(val, 0) && LessThanOrClose(val, 1));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static int DoubleToInt(double val)
    {
      return (0 < val) ? (int)(val + 0.5) : (int)(val - 0.5);
    }

    /// <summary>
    /// rectHasNaN - this returns true if this rect has X, Y , Height or Width as NaN.
    /// </summary>
    /// <param name='r'>The rectangle to test</param>
    /// <returns>returns whether the Rect has NaN</returns>
    public static bool RectHasNaN(Rect r)
    {
      if (double.IsNaN(r.X)
           || double.IsNaN(r.Y)
           || double.IsNaN(r.Height)
           || double.IsNaN(r.Width))
      {
        return true;
      }
      return false;
    }
  }
}