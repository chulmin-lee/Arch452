using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System.Collections.ObjectModel;
using System.Linq;
using UIControls;

namespace EUMC.Client
{
  internal class WardRoomInformation : ContentInformation
  {
    public ObservableCollection<WardRoom> Rooms { get; set; } = new ObservableCollection<WardRoom>();
    int _itemRows;
    int _itemColumns;

    public int ItemRows { get => _itemRows; set => Set(ref _itemRows, value); }
    public int ItemColumns { get => _itemColumns; set => Set(ref _itemColumns, value); }

    string _key;
    public WardRoomInformation(WardRoomViewConfig o) : base(ClientViewManager.RotationInterval)
    {
      _key = o.Key;
    }
    internal bool Update(AREA_WARD_INFO area_wards)
    {
      lock (LOCK)
      {
        this.Rooms.Clear();

        var list = area_wards.AreaWards; //   new List<AREA_WARD_INFO.AREA_WARD>();
                                         //foreach (var area_ward in area_wards)
                                         //{
                                         //  list.AddRange(area_ward.AreaWards);
                                         //}
        var all = list.OrderBy(x => x.RoomCode).ToList();
        int total_room = all.Count();

        // 1인실:156, 4인실:300
        // 기본 배치로 높이 계산 후 넘치면 5로 변경
        var columns = 4;
        var rows = total_room.CalcPageCount(columns);

        int total_height =0;
        for (int i = 0; i < rows; i++)
        {
          var max_capcity = all.Skip(i * columns)
                             .Take(columns)
                             .Select(x => x.Capacity).Max();
          total_height += max_capcity == 1 ? 156 : 300;
        }

        LOG.dc($"total_room = {total_room}, height={total_height} ");

        this.ItemColumns = total_height <= 912 ? 4 : 5;
        this.ItemRows = total_room.CalcPageCount(this.ItemColumns);

        for (int row = 0; row < this.ItemRows; row++)
        {
          for (int col = 0; col < this.ItemColumns; col++)
          {
            var index = this.ItemColumns * row + col;
            if (index < all.Count)
            {
              this.Rooms.Add(new WardRoom(row, col, all[index]));
            }
          }
        }
        return true;
      }
    }
  }

  internal class WardRoom : ViewModelBase
  {
    public int Capacity { get; set; }
    public string RoomName { get; set; }
    public string RoomCode { get; set; }
    public ObservableCollection<WardPatient> Patients { get; set; } = new ObservableCollection<WardPatient>();
    public string Assistant { get => _assistant; set => Set(ref _assistant, value); }

    public int GridRow { get; set; }
    public int GridColumn { get; set; }
    public WardRoom(int row, int col, AREA_WARD_INFO.AREA_WARD o)
    {
      this.GridRow = row;
      this.GridColumn = col;

      this.Capacity = o.Capacity;
      this.RoomCode = o.RoomCode;
      this.RoomName = o.RoomName;
      this.Assistant = o.Assistant;

      int index = 1;
      foreach (var p in o.Patients)
      {
        this.Patients.Add(new WardPatient(index++, p));
      }

      var remained = this.Capacity - this.Patients.Count;
      for (int i = 0; i < remained; i++)
      {
        this.Patients.Add(new WardPatient(index++));
      }
    }
    string _assistant = string.Empty;
  }
  internal class WardPatient
  {
    public int Index { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string GenderAge { get; set; } = string.Empty;
    public string Fall { get; set; } = string.Empty;
    public string Fire { get; set; } = string.Empty;
    public string Surgery { get; set; } = string.Empty;

    public WardPatient(int index)
    {
      this.Index = index;
    }

    public WardPatient(int index, AREA_WARD_INFO.WARD_PATIENT o)
    {
      this.Index = index;
      this.PatientName = o.PatientName;
      this.GenderAge = $"{(o.IsMale ? "M" : "F")}/{o.Age}";
      this.Fall = o.Fall ? "Y" : "N";
      this.Fire = o.Fire ? "Y" : "N";
      this.Surgery = o.Surgery ? "Y" : "N";
    }
  }
}