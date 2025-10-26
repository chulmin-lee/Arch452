using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UIControls
{
  public partial class TableGrid : Grid
  {
    // border thick 만큼 children 크기 조절
    // 선이 두꺼울때 어디서부터
    // 상하좌우 라인은 안쪽으로, 내부 선은 가운데..

    /// <summary>
    /// 정의된 Rows/Columns에 따라 그린다
    /// </summary>
    /// <param name="dc"></param>
    void DrawBorder(DrawingContext dc)
    {
      if (this.RowDefinitions.Count == 0 || this.ColumnDefinitions.Count == 0 || this.ActualHeight == 0 || this.ActualWidth == 0)
        return;

      //=====================================
      // Background
      //=====================================
      this.DrawBackground(dc);

      //=====================================
      // Border line
      //=====================================
      if ((this.BorderThickness <= 0) || (this.BorderBrush == null) || (this.BorderBrush == Brushes.Transparent))
      {
        return;
      }

      Pen pen = new Pen(this.BorderBrush, this.BorderThickness);

      //=====================================
      // Horizontal line
      //=====================================

      // 테두리 그리기
      if (this.DrawOutterBorder && this.BorderThickness > 0)
      {
        var m = this.BorderThickness /2;
        dc.DrawRectangle(null, pen, new Rect(m, m, this.ActualWidth - this.BorderThickness, this.ActualHeight - this.BorderThickness));
      }

      if (this.DrawRowLine && this.BorderThickness > 0 && this.RowCount > 1)
      {
        double width = this.ActualWidth;
        double margin = (width - width * this.RowLineRatio) / 2;
        double x1 = margin + this.BorderThickness;
        double x2 = width - (margin + this.BorderThickness);

        List<double> heights = new List<double>();
        var rows = this.RowDefinitions;
        rows.Skip(1).Take(rows.Count - 1).ToList().ForEach(x => heights.Add(x.Offset));

        foreach (var y in heights)
        {
          dc.DrawLine(pen, new Point(x1, y), new Point(x2, y));
        }
      }
      //=====================================
      // Vertical line
      //=====================================
      if (this.DrawColumnLine && this.BorderThickness > 0 && this.ColumnCount > 1)
      {
        double height = this.ActualHeight;
        double margin = (height - height * this.ColumnLineRatio) /2;

        double y1 = margin + this.BorderThickness;
        double y2 = height - (margin+ this.BorderThickness);

        List<double> widths = new List<double>();
        var columns = this.ColumnDefinitions;
        columns.Skip(1).Take(columns.Count - 1).ToList().ForEach(x => widths.Add(x.Offset));

        foreach (var x in widths)
        {
          dc.DrawLine(pen, new Point(x, y1), new Point(x, y2));
        }
      }
    }
    /// <summary>
    /// Grid 배경을 그린다
    /// BorderThickness를 고려해서 그린다
    /// </summary>
    void DrawBackground(DrawingContext dc)
    {
      if (this.RowBackgroundBrushes.Count > 0)
      {
        var rows = this.RowDefinitions;
        var rowsCount = rows.Count;
        List<Brush> brushes = this.Trim(this.RowBackgroundBrushes, rowsCount);

        double x = Math.Max(this.BorderThickness, 0);
        double width = this.ActualWidth - 2 * this.BorderThickness;

        for (int i = 0; i < rows.Count; i++)
        {
          if (brushes[i] != Brushes.Transparent)
          {
            this.GetBorderMargin(rowsCount, i, this.BorderThickness, out double start, out double end);
            var y = rows[i].Offset + start;
            var height = rows[i].ActualHeight - end;

            if (y < 0 || height <= 0)
              continue;

            dc.DrawRectangle(brushes[i], null, new Rect(x, y, width, height));
          }
        }
      }

      if (this.ColumnBackgroundBrushes.Count > 0)
      {
        var columns = this.ColumnDefinitions;
        var columnCount = columns.Count;
        List<Brush> brushes = this.Trim(this.ColumnBackgroundBrushes, columnCount);

        double y = this.BorderThickness;
        double height = this.ActualHeight - 2 * this.BorderThickness;

        for (int i = 0; i < columns.Count; i++)
        {
          if (brushes[i] != Brushes.Transparent)
          {
            this.GetBorderMargin(columnCount, i, this.BorderThickness, out double start, out double end);
            var x = columns[i].Offset + start;
            var width = columns[i].ActualWidth - end;

            if (x < 0 || width <= 0)
              continue;

            dc.DrawRectangle(brushes[i], null, new Rect(x, y, width, height));
          }
        }
      }
    }
    void GetBorderMargin(int totalCount, int index, double thick, out double start, out double overlap)
    {
      overlap = 0;
      if (index == 0)
      {
        overlap = thick * (totalCount == 1 ? 2 : 1.5);
      }
      else if (index == totalCount - 1)
      {
        overlap = thick * 1.5;
      }
      else
      {
        overlap = thick * 1;
      }
      start = thick * (index == 0 ? 1 : 0.5);
    }

    /// <summary>
    /// 배경색 개수를 지정된 갯수에 맞춘다
    /// </summary>
    List<Brush> Trim(List<BrushValue> colors, int count)
    {
      List<Brush> brushes = new List<Brush>();

      foreach (var p in colors)
      {
        for (int i = 0; i < p.Repeat; i++)
        {
          brushes.Add(p.Color);
        }
      }

      // 넘치는 경우 처리
      brushes = brushes.Take(count).ToList();

      // 색이 모자라는 경우 반복해서 채우기
      if (brushes.Count < count)
      {
        var list = new List<Brush>();
        while (list.Count <= count)
        {
          list.AddRange(brushes);
        }
        brushes = list.Take(count).ToList();
      }
      return brushes;
    }
  }
}