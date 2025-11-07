using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System.Collections.ObjectModel;
using ServiceCommon;
using ServiceCommon.ClientServices;
using EUMC.ClientServices;
using System.Windows.Input;
using UIControls;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EUMC.Client
{
  internal class OfficeMultiInformation : ContentInformation
  {
    public OfficeRoomViewModel RoomPanel { get; set; }
    public ObservableCollection<OfficeRoomViewModel> Rooms { get; set; } = new ObservableCollection<OfficeRoomViewModel>();
    List<OfficeRoomViewModel> _all_rooms { get; set; } = new List<OfficeRoomViewModel>();
    List<OfficeRoomViewModel> _active_rooms { get; set; } = new List<OfficeRoomViewModel>();
    public OfficeMultiViewConfig.ContentConfig CONFIG { get; set; }
    public OfficeMultiInformation(OfficeMultiViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.InitCallMessage(o.IsWideContent);
      this.CONFIG = o.Config;

      int rows = o.Config.ItemRows;

      this.RoomPanel = new OfficeRoomViewModel(rows);

      foreach (var room in o.OfficeRooms)
      {
        var p = new OfficeRoomViewModel(room.GetKey(), rows, room.RoomName);
        p.RoomPatientChanged += (s, e) =>
        {
          this.call_message(e);
        };
        _all_rooms.Add(p);
        _active_rooms.Add(p);
      }
      this.PAGE.SetPage(this.GetPageCount());
      this.GetPageItems().ForEach(x => this.Rooms.Add(x));
    }

    public bool Update(OFFICE_RESP o)
    {
      lock (LOCK)
      {
        //this.StopTimer();

        foreach (var p in o.Rooms)
        {
          var find = _all_rooms.Where(x => x.Key == p.GroupKey).FirstOrDefault();

          if (find != null)
          {
            LOG.dc($"OfficePatient: {p.GroupKey} - {p.WaitPatients.Count}, InRoom: {((p.RoomPatient != null) ? "1" : "0")}");
            find.Update(p);
          }
          else
          {
            LOG.ec($"{p.GroupKey} not found");
          }
        }

        _active_rooms = _all_rooms.Where(x => x.UseRoom).ToList();
        this.PAGE.SetPage(this.GetPageCount());
        this.Refresh();

        //if (this.PAGE.IsRotate)
        //{
        //  this.StartTimer(this.Refresh);
        //}
        //else
        //{
        //  loopCounter = 0;
        //}
        return true;
      }
    }
    void Refresh()
    {
      lock (LOCK)
      {
        this.Rooms.Clear();
        this.GetPageItems().ForEach(x => this.Rooms.Add(x));
      }
    }
    protected override int GetPageCount()
    {
      return _active_rooms.CalcPageCount(this.CONFIG.RoomPerPage);
    }
    List<OfficeRoomViewModel> GetPageItems()
    {
      int pageIndex = this.PAGE.RotatePage(this.LoopCounter++);
      return _active_rooms.GetPageItems(pageIndex, this.CONFIG.RoomPerPage);
    }
    internal bool CallPatient(CALL_PATIENT_NOTI o)
    {
      this.call_message(o.PatientNameTTS, o.CallMessage, o.Speech);
      return true;
    }
    internal bool CallAnnounce(CALL_ANNOUNCE_NOTI o)
    {
      this.call_message(o.Message);
      return true;
    }
  }

  
}