using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EUMC.Client
{
  internal class EndoInformation : ContentInformation
  {
    public EndoViewConfig.ContentConfig CONFIG { get; set; }
    public ObservableCollection<ENDO_INFO> Waiting { get; set; } = new ObservableCollection<ENDO_INFO>();
    public ObservableCollection<ENDO_INFO> Inspecting { get; set; } = new ObservableCollection<ENDO_INFO>();
    public ObservableCollection<ENDO_INFO> Recovering { get; set; } = new ObservableCollection<ENDO_INFO>();

    List<ENDO_INFO> _all_waiting = new List<ENDO_INFO>();
    List<ENDO_INFO> _all_inspecting = new List<ENDO_INFO>();
    List<ENDO_INFO> _all_recovering = new List<ENDO_INFO>();

    int _waiting_count = 0;
    int _inspecting_count = 0;
    int _recovering_count = 0;
    public EndoInformation(EndoViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.CONFIG = o.Config;

      _waiting_count = this.CONFIG.ItemRows;
      _inspecting_count = this.CONFIG.ItemRows;
      _recovering_count = this.CONFIG.ItemRows * 2;
    }
    internal bool Update(List<ENDO_INFO> patients)
    {
      lock (LOCK)
      {
        this.StopTimer();
        LoopCounter = 0;

        LOG.dc($"PatientCount : {patients.Count}");

        // 주의: 미리 그리는 이유는, Refresh()에서 순환할 필요가 없으면 refresh 하지 않는다
        //       미리 그려두지 않았다면, 아무것도 출력되지 않는다

        // 대기중
        {
          _all_waiting.Clear();
          patients.Where(x => x.StateCode == ENDO_INFO.STATE.Waiting).ToList().ForEach(x => _all_waiting.Add(x));

          this.Waiting.Clear();
          _all_waiting.GetPageItems(0, _waiting_count).ForEach(x => this.Waiting.Add(x));
        }
        // 검사중
        {
          _all_inspecting.Clear();
          patients.Where(x => x.StateCode == ENDO_INFO.STATE.Inspecting).ToList().ForEach(x => _all_inspecting.Add(x));

          this.Inspecting.Clear();
          _all_inspecting.GetPageItems(0, _inspecting_count).ForEach(x => this.Inspecting.Add(x));
        }

        // 회복중
        {
          _all_recovering.Clear();
          patients.Where(x => x.StateCode == ENDO_INFO.STATE.Recovering).ToList().ForEach(x => _all_recovering.Add(x));

          this.Recovering.Clear();
          _all_recovering.GetPageItems(0, _recovering_count).ForEach(x => this.Recovering.Add(x));
        }

        this.PAGE.SetPage(this.GetPageCount());

        if (this.PAGE.IsRotate)
        {
          this.StartTimer(this.Refresh);
        }
        return true;
      }
    }
    void Refresh()
    {
      lock (LOCK)
      {
        int page = this.PAGE.RotatePage(this.LoopCounter++);

        if (_all_waiting.Count > _waiting_count)
        {
          this.Waiting.Clear();
          _all_waiting.GetPageItems(page, _waiting_count).ForEach(x => this.Waiting.Add(x));
        }

        if (_all_inspecting.Count > _inspecting_count)
        {
          this.Inspecting.Clear();
          _all_inspecting.GetPageItems(page, _inspecting_count).ForEach(x => this.Inspecting.Add(x));
        }
        if (_all_recovering.Count > _recovering_count)
        {
          this.Recovering.Clear();
          _all_recovering.GetPageItems(page, _recovering_count).ForEach(x => this.Recovering.Add(x));
        }
      }
    }
    protected override int GetPageCount()
    {
      int count = Math.Max(_all_waiting.Count , _all_inspecting.Count);
      count = Math.Max(count, _all_recovering.Count / 2);
      return count.CalcPageCount(this.CONFIG.ItemRows);
    }
  }
}