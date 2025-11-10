using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EUMC.Client
{
  internal class MultiRoomInformation : ContentInformation
  {
    public MultiRoomVM RoomPanel { get; set; }
    public ObservableCollection<MultiRoomVM> Rooms { get; set; } = new ObservableCollection<MultiRoomVM>();
    List<MultiRoomVM> _all_rooms { get; set; } = new List<MultiRoomVM>();
    public int RoomPerPage { get; set; }

    public MultiRoomInformation(OpdMultiViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      this.RoomPerPage = o.Config.RoomPerPage;
      this.RoomPanel = new MultiRoomVM(o, o.PanelConfig);

      // 남는 영역은 빈 진료실로 채운다.
      bool is_full = false;

      if (is_full)
      {
        int max_room = o.Rooms.CalcMaxItemCount(this.RoomPerPage);
        for (int i = 0; i < max_room; i++)
        {
          if (o.Rooms.Count > i)
          {
            _all_rooms.Add(new MultiRoomVM(o, o.Rooms[i]));
          }
          else
          {
            _all_rooms.Add(new MultiRoomVM(o));
          }
        }
      }
      else
      {
        for (int i = 0; i < o.Rooms.Count; i++)
        {
          _all_rooms.Add(new MultiRoomVM(o, o.Rooms[i]));
        }
      }

      // 페이지 설정
      this.PAGE.SetPage(this.GetPageCount());
      this.Refresh();
      if (this.PAGE.IsRotate)
      {
        this.StartTimer(this.Refresh);
      }
    }

    public void Update(List<OPD_ROOM_INFO> rooms)
    {
      foreach (var p in rooms)
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
      return _all_rooms.CalcPageCount(this.RoomPerPage);
    }
    List<MultiRoomVM> GetPageItems()
    {
      int pageIndex = this.PAGE.RotatePage(this.LoopCounter++);
      return _all_rooms.GetPageItems(pageIndex, this.RoomPerPage);
    }
  }
}