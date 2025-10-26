using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EUMC.Client
{
  internal class OperationInformation : ContentInformation
  {
    public OperationViewConfig.ContentConfig CONFIG { get; set; }
    public ObservableCollection<OPERATION_INFO> Patients { get; set; }
    public OperationInformation(OperationViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.InitCallMessage(o.IsWideContent);
      this.CONFIG = o.Config;
      this.Patients = new ObservableCollection<OPERATION_INFO>();
      this.PageItemCount = this.CONFIG.ItemRows;
    }
    public bool Update(OPERATION_RESP o)
    {
      lock (LOCK)
      {
        this.StopTimer();
        LOG.dc($"OperationPatientCount : {o.Patients.Count}");

        // update & draw
        {
          _all_items.Clear();
          _all_items.AddRange(o.Patients);
          this.PAGE.SetPage(this.GetPageCount());
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
    List<OPERATION_INFO> _all_items = new List<OPERATION_INFO>();
  }
}