using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.ClientServices
{
  public class PlaylistMedical
  {
    public string HospitalCode { get; set; } = string.Empty;
    public List<ICU> Icus { get; set; } = new List<ICU>();
    public List<DEPT_ROOMS> DeptRooms { get; set; } = new List<DEPT_ROOMS>();
    public Dictionary<string, List<ROOM>> CustomRooms { get; set; } = new Dictionary<string, List<ROOM>>();
    public WARD_ROOM WardRooms { get; set; }
    public FUNERAL Funeral { get; set; }
    public string LargeTitle { get; set; } = string.Empty;

    public void AddIcu(string deptCode, string deptName, string title = "")
    {
      if (string.IsNullOrEmpty(deptCode))// || string.IsNullOrEmpty(deptName))
      {
        LOG.w($"Invalid ICU data:{deptCode}:{deptName}");
        return;
      }
      if (this.Icus.Where(x => x.DeptCode == deptCode).Any())
      {
        LOG.w($"Duplicate ICU: {deptCode}:{deptName}");
        return;
      }
      this.Icus.Add(new ICU { DeptCode = deptCode, DeptName = deptName, Title = title });
    }
    public void AddDeptRoom(ROOM p)
    {
      var dept = this.DeptRooms.FirstOrDefault(x => x.DeptCode == p.DeptCode &&
                                                  x.RoomType == p.RoomType);
      if (dept == null)
      {
        dept = new DEPT_ROOMS { DeptCode = p.DeptCode, RoomType = p.RoomType };
        this.DeptRooms.Add(dept);
      }

      if (dept.Rooms.Where(x => x.RoomCode == p.RoomCode).Any())
      {
        LOG.w($"Duplicate room: {p.Key}");
        return;
      }
      dept.Rooms.Add(p);
    }
    public void AddCustomRooms(string name, List<ROOM> rooms)
    {
      if (this.CustomRooms.ContainsKey(name))
      {
        throw new Exception($"{name} is already exist");
      }
      this.CustomRooms.Add(name, rooms);
    }

    public class DEPT_ROOMS
    {
      public string DeptCode = string.Empty;
      public string RoomType = string.Empty;
      public List<ROOM> Rooms = new List<ROOM>();
      public List<string> RoomCodes => this.Rooms.Select(x => x.RoomCode).ToList();
    }

    public class ROOM
    {
      public string RoomType = string.Empty;
      public string DeptCode = string.Empty;
      public string DeptName { get; set; } = string.Empty;
      public string RoomCode = string.Empty;
      public string RoomName = string.Empty;
      public string DurationTime { get; set; } = string.Empty; // 지연 등
      public string RoomTitle { get; set; } = string.Empty;

      public string Key => $"{DeptCode}:{RoomCode}";
    }

    public class ICU
    {
      public string DeptCode { get; set; } = string.Empty;
      public string DeptName { get; set; } = string.Empty;
      public string Title { get; set; } = string.Empty;
    }

    public class WARD_ROOM
    {
      public int Floor { get; set; }
      public string AreaCode { get; set; } = string.Empty;
    }

    public class FUNERAL
    {
      public bool IsAll { get; set; }
      public List<string> RoomCodes { get; set; } = new List<string>();
    }
  }
}