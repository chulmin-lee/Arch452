using System.Windows.Controls;

namespace UIControls
{
  public partial class TableGrid : Grid
  {
    /*
  /// <summary>
  /// Span을 고려해서 그린다.
  /// </summary>
  /// <param name="dc"></param>
  private void DrawSpanBorder(DrawingContext dc)
  {
    RowDefinitionCollection rows = this.RowDefinitions;
    ColumnDefinitionCollection columns = this.ColumnDefinitions;
    int rowCount = rows.Count;
    int columnCount = columns.Count;
    const byte BORDER_LEFT = 0x08;
    const byte BORDER_TOP = 0x04;
    const byte BORDER_RIGHT = 0x02;
    const byte BORDER_BOTTOM = 0x01;
    byte[,] borderState = new byte[rowCount, columnCount];
    int column = columnCount - 1;
    int columnSpan;
    int row = rowCount - 1;
    int rowSpan;

    #region generate main border data
    if (this.DrawOutterBorder)
    {
      // header mode가 아니고, 외곽 표시 일때
      for (int i = 0; i < rowCount; i++)
      {
        borderState[i, 0] = BORDER_LEFT;       // 맨 왼쪽 라인
        borderState[i, column] = BORDER_RIGHT; // 맨 오른쪽 라인
      }

      for (int i = 0; i < columnCount; i++)
      {
        borderState[0, i] |= BORDER_TOP;       // 맨위쪽 라인
        borderState[row, i] |= BORDER_BOTTOM;  // 맨 아래쪽 라인 (그냥 한줄로??)
      }
    }
    #endregion generate main border data

    #region generate child border data

    foreach (UIElement child in this.InternalChildren)
    {
      this.GetChildLayout(child, out row, out rowSpan, out column, out columnSpan);
      for (int i = 0; i < rowSpan; i++)
      {
        borderState[row + i, column] |= BORDER_LEFT;
        borderState[row + i, column + columnSpan - 1] |= BORDER_RIGHT;
      }
      for (int i = 0; i < columnSpan; i++)
      {
        borderState[row, column + i] |= BORDER_TOP;
        borderState[row + rowSpan - 1, column + i] |= BORDER_BOTTOM;
      }
    }
    #endregion generate child border data

    #region draw border
    Pen pen = new Pen(this.BorderBrush, this.BorderThickness);
    double left;
    double top;
    double width, height;

    for (int r = 0; r < rowCount; r++)
    {
      RowDefinition v = rows[r];

      height = v.ActualHeight;

      double h_step = (height - height * this.ColumnLineRatio) /2;
      top = v.Offset + h_step;
      height = height - h_step * 2;

      for (int c = 0; c < columnCount; c++)
      {
        byte state = borderState[r, c];

        ColumnDefinition h = columns[c];

        width = h.ActualWidth;

        double w_step = (width - width * this.RowLineRatio) /2;
        left = h.Offset + w_step;
        w_step = width - w_step * 2;

        if ((state & BORDER_LEFT) == BORDER_LEFT)
        {
          dc.DrawLine(pen, new Point(left, top), new Point(left, top + height));
        }
        if ((state & BORDER_TOP) == BORDER_TOP)
        {
          dc.DrawLine(pen, new Point(left, top), new Point(left + width, top));
        }
        if ((state & BORDER_RIGHT) == BORDER_RIGHT && (c + 1 >= columnCount || (borderState[r, c + 1] & BORDER_LEFT) == 0))
        {
          dc.DrawLine(pen, new Point(left + width, top), new Point(left + width, top + height));
        }
        if ((state & BORDER_BOTTOM) == BORDER_BOTTOM && (r + 1 >= rowCount || (borderState[r + 1, c] & BORDER_TOP) == 0))
        {
          dc.DrawLine(pen, new Point(left, top + height), new Point(left + width, top + height));
        }
      }
    }
    #endregion draw border
  }

  private void GetChildLayout(UIElement o, out int r, out int rs, out int c, out int cs)
  {
    int rowCount = this.RowDefinitions.Count;

    r = Math.Min((int)o.GetValue(Grid.RowProperty), rowCount - 1);
    rs = (int)o.GetValue(Grid.RowSpanProperty);
    if (r + rs > rowCount)
    {
      rs = rowCount - r;
    }

    int columnCount = this.ColumnDefinitions.Count;
    c = Math.Min((int)o.GetValue(Grid.ColumnProperty), columnCount - 1);
    cs = (int)o.GetValue(Grid.ColumnSpanProperty);
    if (c + cs > columnCount)
    {
      cs = columnCount - c;
    }
  }
  */

    /*
    /// <summary>
    /// 박스형 (Span 고려)
    /// </summary>
    /// <param name="dc"></param>
    private void DrawSeperatedBorder(DrawingContext dc)
    {
      double spacing = this.CellSpacing;
      double halfSpacing = spacing * 0.5D;

      #region draw border
      Pen pen = new Pen(this.BorderBrush, this.BorderThickness);
      UIElementCollection children = this.InternalChildren;
      foreach (UIElement child in children)
      {
        this.GetChildBounds(child, out double left, out double top, out double width, out double height);
        left += halfSpacing;
        top += halfSpacing;
        width -= spacing;
        height -= spacing;

        dc.DrawRectangle(null, pen, new Rect(left, top, width, height));
      }
      #endregion draw border
    }
    /// <summary>
    /// 박스형 (Span 무시)
    /// </summary>
    /// <param name="dc"></param>
    private void RawSeperatedBorder(DrawingContext dc)
    {
      double spacing = this.CellSpacing;
      double halfSpacing = spacing * 0.5D;

      #region draw border
      Pen pen = new Pen(this.BorderBrush, this.BorderThickness);

      ColumnDefinitionCollection columns = this.ColumnDefinitions;
      RowDefinitionCollection rows = this.RowDefinitions;
      int rowCount = rows.Count;
      int columnCount = columns.Count;

      for (int row = 0; row < rowCount; row++)
      {
        for (int column = 0; column < columnCount; column++)
        {
          double left = columns[column].Offset;
          double top = rows[row].Offset;
          ColumnDefinition right = columns[column];
          double width = right.Offset + right.ActualWidth - left;
          RowDefinition bottom = rows[row];
          double height = bottom.Offset + bottom.ActualHeight - top;

          left += halfSpacing;
          top += halfSpacing;
          width -= spacing;
          height -= spacing;

          dc.DrawRectangle(null, pen, new Rect(left, top, width, height));
        }
      }
      #endregion draw border
    }
    */
  }
}