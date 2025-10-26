using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace UIControls
{
  public partial class CarouselUniformGrid : Canvas
  {
    object _lock = new object();
    int _currentIndex = 0;
    Storyboard _storyboard;
    bool _storboard_start = false;
    /// <summary>
    /// Rows, Columns가 변경 되었을때
    /// </summary>
    void layout_changed()
    {
      this.itemssource_changed();
    }
    void itemssource_changed()
    {
      _storboard_start = false;
      _delay_timer.Interval = TimeSpan.FromMilliseconds(this.DelayInterval);
      _currentIndex = 0;

      if (this.create_children() && this.ActualWidth > 0 && this.ActualHeight > 0)
        _delay_timer.Start();
    }
    void collection_changed()
    {
      _storboard_start = false;
      if (this.create_children())
        _delay_timer.Start();
    }

    void size_changed()
    {
      if (this.ActualHeight > 0 && this.ActualWidth > 0 && this.Children.Count > 0)
      {
        _delay_timer.Stop();
        _storyboard?.Stop();

        lock (_lock)
        {
          foreach (FrameworkElement p in this.Children)
          {
            p.Width = this.ActualWidth;
            p.Height = this.ActualHeight;
            Canvas.SetLeft(p, this.ActualWidth);
          }

          if (_currentIndex >= this.Children.Count)
          {
            _currentIndex = this.Children.Count - 1;
          }

          var element = this.Children[_currentIndex] as FrameworkElement;
          Canvas.SetLeft(element, 0);

          if (this.Children.Count > 1)
            _delay_timer.Start();
        }
      }
    }
    void story_board(int start)
    {
      int first = start;
      int second = (start + 1) % this.Children.Count;

      var ani_first = new DoubleAnimation()
      {
        From = 0,
        To = -this.ActualWidth,
        Duration = new Duration(new TimeSpan(0, 0, 1)),
      };
      Storyboard.SetTarget(ani_first, GetChild(first));
      Storyboard.SetTargetProperty(ani_first, new PropertyPath(Canvas.LeftProperty));

      var ani_second = new DoubleAnimation()
      {
        From = this.ActualWidth,
        To = 0,
        Duration = new Duration(new TimeSpan(0, 0, 1)),
      };
      Storyboard.SetTarget(ani_second, GetChild(second));
      Storyboard.SetTargetProperty(ani_second, new PropertyPath(Canvas.LeftProperty));

      if (_storyboard == null)
      {
        _storyboard = new Storyboard();
        _storyboard.Completed += Storyboard_Completed;
      }
      else
      {
        _storyboard.Children.Clear();
      }

      //_storyboard = new Storyboard();
      //_storyboard.Completed += Storyboard_Completed;
      _storyboard.Children.Add(ani_first);
      _storyboard.Children.Add(ani_second);
      _storyboard.Begin();

      _storboard_start = true;
    }
    private void Storyboard_Completed(object sender, EventArgs e)
    {
      _delay_timer.Start();
    }
    void DelayTimerExpired(object sender, EventArgs e)
    {
      _delay_timer.Stop();

      lock (_lock)
      {
        if (this.Children.Count > 1)
        {
          if (_storboard_start)
          {
            // 뒤로 옮기기
            var element = GetChild(_currentIndex);
            Canvas.SetLeft(element, this.ActualWidth);

            // 다음번 이동할 index
            _currentIndex = (_currentIndex + 1) % this.Children.Count;
            _delay_timer.Start();

            Debug.WriteLine($"{_currentIndex} scroll");
            story_board(_currentIndex);
          }
          else
          {
            story_board(_currentIndex);
          }
        }
      }
    }

    bool create_children()
    {
      _storyboard?.Stop();
      _delay_timer.Stop();

      lock (_lock)
      {
        this.Children.Clear();

        UniformGrid grid = null;

        foreach (var item in this.ItemsSource)
        {
          if (grid == null || grid.Children.Count >= Columns)
          {
            grid = create_uniformgrid(this.Rows, this.Columns);
            this.Children.Add(grid);
          }
          grid.Children.Add(new ContentControl() { Content = item });
        }

        if (this.Children.Count > 1)
        {
          if (_currentIndex >= this.Children.Count)
          {
            _currentIndex = this.Children.Count - 1;
          }

          var element = this.Children[_currentIndex] as FrameworkElement;
          Canvas.SetLeft(element, 0);
        }
        else
        {
          _currentIndex = 0;
          if (this.Children.Count == 1)
          {
            var element = this.Children[_currentIndex] as FrameworkElement;
            Canvas.SetLeft(element, 0);
          }
        }

        return this.Children.Count > 1;
      }
    }

    UniformGrid create_uniformgrid(int row, int col)
    {
      UniformGrid grid = new UniformGrid()
      {
        Width = this.ActualWidth,
        Height = this.ActualHeight,
        Rows = row,
        Columns = col,
      };
      Canvas.SetTop(grid, 0);
      Canvas.SetLeft(grid, this.ActualWidth);
      return grid;
    }
  }
}