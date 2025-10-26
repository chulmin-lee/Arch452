using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EUMC.Client
{
  internal class DrugInformation : ContentInformation
  {
    public DrugViewConfig.ContentConfig CONFIG { get; set; }
    public DrugCall DrugCall { get; set; } = new DrugCall();
    public ObservableCollection<DRUG_INFO> Items { get; set; } = new ObservableCollection<DRUG_INFO>();
    List<DRUG_INFO> _all_items = new List<DRUG_INFO>();
    public DrugInformation(DrugViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.InitCallMessage(o.IsWideContent, true);
      this.CONFIG = o.Config;
      this.PageItemCount = this.CONFIG.ItemRows * this.CONFIG.ItemColumns;
    }

    public bool Update(DRUG_RESP o)
    {
      lock (LOCK)
      {
        this.StopTimer();
        // update & draw
        {
          _all_items.Clear();
          o.Drugs.Where(x => x.IsDone).ToList().ForEach(x => _all_items.Add(x));
          LOG.dc($"count: {_all_items.Count}");
          this.DrugCall.Update(_all_items);
          this.PAGE.SetPage(this.GetPageCount());
          this.Refresh();
        }

        if (this.PAGE.IsRotate)
        {
          this.StartTimer(this.Refresh);
        }
        else
        {
          this.LoopCounter = 0;
        }
        return true;
      }
    }
    void Refresh()
    {
      lock (LOCK)
      {
        int page = this.PAGE.RotatePage(this.LoopCounter++);
        this.Items.Clear();
        _all_items.GetPageItems(page, this.PageItemCount)
                  .ForEach(x => this.Items.Add(x));
      }
    }
    protected override int GetPageCount()
    {
      return _all_items.CalcPageCount(this.PageItemCount);
    }
    internal bool CallPatient(CALL_PATIENT_NOTI o)
    {
      this.call_message(o.PatientNameTTS, o.CallMessage, o.Speech);
      return true;
    }
    protected override void CloseImpl()
    {
      LOG.dc("close drugcall");
      this.DrugCall.Close();
    }
  }

  internal class DrugCall : ContentInformation
  {
    public bool IsCalling { get => _isCalling; set => Set(ref _isCalling, value); }
    public string PatientName { get => _patientName; set => Set(ref _patientName, value); }

    Dictionary<string, DrugCallInfo> _done = new Dictionary<string, DrugCallInfo>();
    Dictionary<string, DrugCallInfo> _waiting = new Dictionary<string, DrugCallInfo>();
    public DrugCall() : base(5)
    {
    }
    public void Update(List<DRUG_INFO> drugs)
    {
      // 원본을 수정하면 안됨
      var items = drugs.ToList();
      lock (LOCK)
      {
        this.StopTimer();

        // 1시간 지난 알림 정보 삭제
        this.clear_old_called();

        // waiting 목록과 비교
        foreach (var wait_drug_no in _waiting.Keys)
        {
          var find = items.Where(x => x.DrugNo == wait_drug_no).FirstOrDefault();
          if (find != null)
          {
            // 이미 등록되어있으면 목록에서 제거
            items.Remove(find);
          }
          else
          {
            // 조제목록에 알림 번호가 없음.
            _done[wait_drug_no] = _waiting[wait_drug_no];
            _waiting.Remove(wait_drug_no);
          }
        }
        // done목록과 비교
        foreach (var done_drug_no in _done.Keys)
        {
          var find = items.Where(x => x.DrugNo == done_drug_no).FirstOrDefault();
          if (find != null)
          {
            // 이미 알림이 끝났으면 목록에서 제거
            items.Remove(find);
          }
        }

        // 새로운 알림 정보 추가
        foreach (var p in items)
        {
          _waiting[p.DrugNo] = new DrugCallInfo(p);
        }

        this.IsCalling = _waiting.Count > 0;
        this.StartTimer(Refresh);
        this.Refresh();
      }
    }

    void Refresh()
    {
      lock (LOCK)
      {
        if (_waiting.Count == 0)
        {
          this.IsCalling = false;
          this.PatientName = string.Empty;
          this.StopTimer();
        }
        else
        {
          var call = _waiting.First().Value;
          _done[call.DrugNo] = call;
          _waiting.Remove(call.DrugNo);

          LOG.dc($"alarm no: {call.DrugNo}, remain alarm: {_waiting.Count}");
          this.PatientName = call.PatientName;
        }

        // 1시간 지난 알림 정보 삭제
        this.clear_old_called();
      }
    }

    void clear_old_called()
    {
      // 1시간 지난 알림 정보 삭제
      _done.Values.Where(x => (DateTime.Now - x.CallTime).TotalMinutes > 60).ToList()
                    .ForEach(x => _done.Remove(x.DrugNo));
    }

    string _patientName = string.Empty;
    bool _isCalling;

    protected override void CloseImpl()
    {
      LOG.dc("drugcall close");
    }

    class DrugCallInfo
    {
      public string DrugNo { get; set; } = string.Empty;
      public string PatientNo { get; set; } = string.Empty;
      public string PatientName { get; set; } = string.Empty;
      public DateTime CallTime { get; set; } = DateTime.Now;

      public DrugCallInfo(DRUG_INFO d)
      {
        this.DrugNo = d.DrugNo;
        this.PatientNo = d.PatientNo;
        this.PatientName = d.PatientName;
      }
    }
  }
}