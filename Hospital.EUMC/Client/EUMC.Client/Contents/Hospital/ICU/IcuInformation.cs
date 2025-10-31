using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EUMC.Client
{
  internal class IcuInformation : ContentInformation
  {
    public event EventHandler<string> TitleChanged;
    public IcuViewConfig.ContentConfig CONFIG { get; set; }

    List<ICU_GROUP> _groups = new List<ICU_GROUP>();
    public IcuInformation(IcuViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.InitCallMessage(o.IsWideContent);
      this.CONFIG = o.Config;

      foreach (var p in o.Rooms)
      {
        _groups.Add(new ICU_GROUP(p.IcuCode, p.IcuName, p.Title, this.CONFIG.ItemRows));
      }
    }

    public bool Update(ICU_RESP o)
    {
      lock (LOCK)
      {
        this.StopTimer();

        // update & draw
        {
          foreach (var d in o.Datas)
          {
            var find = _groups.Where(x => x.IcuCode == d.IcuCode).FirstOrDefault();
            if (find != null)
            {
              find.Update(d);
            }
            else
            {
              LOG.ec($"{d.IcuCode} not found");
            }
          }
          // 모든 icu 를 통합한 전체 페이지수
          var total_page = _groups.Select(x => x.PageCount).Sum();
          this.PAGE.SetPage(total_page);
          this.Refresh();
        }

        if (this.PAGE.IsRotate)
        {
          this.StartTimer(this.Refresh);
        }
        else
        {
          LoopCounter = 0;
        }
        return true;
      }
    }
    void Refresh()
    {
      lock (LOCK)
      {
        int page = this.PAGE.RotatePage(this.LoopCounter++);
        foreach (var icu in _groups)
        {
          if (icu.PageCount > page)
          {
            this.Content = icu[page];
            return;
          }
          page -= icu.PageCount; // icu 단위로 페이지 skip
        }
        this.Content = null;
      }
    }
    internal bool CallPatient(CALL_PATIENT_NOTI o)
    {
      this.call_message(o.PatientNameTTS, o.CallMessage, o.Speech);
      return true;
    }

    ICU_CONTENT _content = null;
    public ICU_CONTENT Content
    {
      get => _content;
      set
      {
        if (Set(ref _content, value))
        {
          this.TitleChanged?.Invoke(this, value?.ContentTile ?? string.Empty);
        }
      }
    }
  }
}