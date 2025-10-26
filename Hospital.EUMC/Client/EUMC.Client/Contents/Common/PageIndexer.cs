using Common;
using UIControls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EUMC.Client
{
  public class PageIndexer : ViewModelBase
  {
    public ObservableCollection<PageBullet> Pages { get; private set; } = new ObservableCollection<PageBullet>();
    List<PageBullet> _all_pages = new List<PageBullet>();

    bool _isRotate;
    public bool IsRotate { get => _isRotate; set => Set(ref _isRotate, value); }
    int PageCount => _all_pages.Count;
    int _maxShowCount;
    public PageIndexer(int max = 5)
    {
      _maxShowCount = max;
    }

    public void SetPage(int total_page)
    {
      if (total_page != this.PageCount)
      {
        this.IsRotate = total_page > 1;
        this.Pages.Clear();
        _all_pages.Clear();

        for (int i = 0; i < total_page; i++)
        {
          var o = new PageBullet(i+1);
          _all_pages.Add(o);
        }
        _all_pages[0].Selected(true);

        var count = Math.Min(total_page, _maxShowCount);
        for (int i = 0; i < count; i++)
        {
          this.Pages.Add(_all_pages[i]);
        }
      }
    }
    public int GetCurrentPage(int loop)
    {
      return this.PageCount.GetCurrentPage(loop);
    }
    public int RotatePage(int loop)
    {
      if (this.PageCount > 0)
      {
        int pageIndex = this.PageCount.GetCurrentPage(loop);
        this.SelectPage(pageIndex);
        return pageIndex;
      }
      return 0;
    }
    public void SelectPage(int no)
    {
      if (no < this.PageCount)
      {
        for (int i = 0; i < this.PageCount; i++)
        {
          _all_pages[i].Selected(i == no);
        }

        if (this.PageCount > _maxShowCount)
        {
          int idx = no / _maxShowCount;
          var list = _all_pages.Skip(idx * _maxShowCount).Take(_maxShowCount).ToList();
          this.Pages.Clear();
          list.ForEach(x => this.Pages.Add(x));
        }
      }
    }
  }

  public class PageBullet : ViewModelBase
  {
    bool _isSelected;
    public bool IsSelected
    {
      get => _isSelected;
      set => Set(ref _isSelected, value);
    }
    public int PageNo { get; set; }
    public double FontSize { get; set; } = 40;
    public PageBullet(int no)
    {
      this.PageNo = no;

      if (no < 10)
      {
        this.FontSize = 40;
      }
      else if (no < 100)
      {
        this.FontSize = 25;
      }
      else
      {
        this.FontSize = 20;
      }
    }
    public void Selected(bool isSelected)
    {
      this.IsSelected = isSelected;
    }
  }
}