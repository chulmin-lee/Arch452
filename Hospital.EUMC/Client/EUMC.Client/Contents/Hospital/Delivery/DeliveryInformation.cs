using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EUMC.Client
{
  internal class DeliveryInformation : ContentInformation
  {
    public DeliveryRoomViewConfig.ContentConfig CONFIG { get; set; }
    public DeliveryCall DeliveryCall { get; set; } = new DeliveryCall();

    List<DELIVERY_INFO> _all_items = new List<DELIVERY_INFO>();
    public ObservableCollection<DELIVERY_INFO> Patients { get; set; }
    public DeliveryInformation(DeliveryRoomViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.CONFIG = o.Config;
      this.Patients = new ObservableCollection<DELIVERY_INFO>();
      this.PageItemCount = this.CONFIG.ItemRows;
    }

    internal bool Update(List<DELIVERY_INFO> patients)
    {
      lock (LOCK)
      {
        this.StopTimer();
        LOG.dc($"Count : {patients.Count}");

        // update & draw
        {
          _all_items.Clear();
          _all_items.AddRange(patients);
          this.DeliveryCall.Update(_all_items);
          this.PAGE.SetPage(this.GetPageCount()); // 페이지 설정을 먼저 해야한다
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
        this.Patients.Clear();
        _all_items.GetPageItems(page, this.PageItemCount)
                  .ForEach(x => this.Patients.Add(x));
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
    internal bool OperationCall(CALL_OPERATION_NOTI o)
    {
      this.call_message(o.CallMessage1, o.CallMessage2);
      return true;
    }
    protected override void CloseImpl()
    {
      LOG.dc("close delivery call");
      this.DeliveryCall.Close();
    }
  }

  internal class DeliveryCall : ContentInformation
  {
    public bool IsCalling { get => _isCalling; set => Set(ref _isCalling, value); }
    public string Message1 { get => _message1; set => Set(ref _message1, value); }
    public string Message2 { get => _message2; set => Set(ref _message2, value); }

    Dictionary<string, DeliveryCallInfo> _called = new Dictionary<string, DeliveryCallInfo>();
    Dictionary<string, DeliveryCallInfo> _calling = new Dictionary<string, DeliveryCallInfo>();
    public DeliveryCall() : base(5)
    {
    }
    public void Update(List<DELIVERY_INFO> patients)
    {
      lock (LOCK)
      {
        this.StopTimer();

        // 1시간 지난 알림 정보 삭제
        this.clear_old_called();

        patients = patients.Where(x => x.IsChildBirth).ToList();

        // 알림 목록에서 삭제된 조제 정보 삭제
        foreach (var pt_no in _calling.Keys)
        {
          if (patients.Where(x => x.PatientNo == pt_no).Count() == 0)
          {
            // 알림 목록에 있지만, 조제 목록에는 없는 경우
            _called[pt_no] = _calling[pt_no];
            _calling.Remove(pt_no);
          }
        }
        LOG.dc($"calling count (removed): {_calling.Count}");

        // 새로운 알림 정보 추가
        foreach (var p in patients)
        {
          if (_called.ContainsKey(p.PatientNo) || _calling.ContainsKey(p.PatientNo))
          {
            // 알림이 완료됬거나, 알림 목록에 있는 경우는 무시
            continue;
          }
          _calling[p.PatientNo] = new DeliveryCallInfo(p);
        }
        LOG.dc($"calling count: {_calling.Count}");

        this.IsCalling = _calling.Count > 0;
        this.StartTimer(Refresh);
        this.Refresh();
      }
    }

    void Refresh()
    {
      lock (LOCK)
      {
        if (_calling.Count == 0)
        {
          this.IsCalling = false;
          this.Message1 = string.Empty;
          this.Message2 = string.Empty;
          this.StopTimer();
        }
        else
        {
          var call = _calling.First().Value;
          _called[call.PatientNo] = call;
          _calling.Remove(call.PatientNo);

          LOG.dc($"remain alarm: {_calling.Count}");
          this.Message1 = call.Message1;
          this.Message2 = call.Message2;
        }

        // 1시간 지난 알림 정보 삭제
        this.clear_old_called();
      }
    }

    void clear_old_called()
    {
      // 1시간 지난 알림 정보 삭제
      _called.Values.Where(x => (DateTime.Now - x.CallTime).TotalMinutes > 60).ToList()
                    .ForEach(x => _called.Remove(x.PatientNo));
    }

    string _message1 = string.Empty;
    string _message2 = string.Empty;
    bool _isCalling;

    protected override void CloseImpl()
    {
      LOG.dc("deliverycall close");
    }

    class DeliveryCallInfo
    {
      public string PatientNo { get; set; } = string.Empty;
      public string PatientName { get; set; } = string.Empty;

      public string Message1 { get; set; }
      public string Message2 { get; set; }

      public DateTime CallTime { get; set; } = DateTime.Now;

      public DeliveryCallInfo(DELIVERY_INFO o)
      {
        this.PatientNo = o.PatientNo;
        this.PatientName = o.PatientName;

        this.Message1 = $"{o.PatientName} 님이";

        var m = o.IsMale ? "건강한 남아를" : "예쁜 여아를";
        this.Message2 = $"{m} 출산하셨습니다.";
      }
    }
  }
}