using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon
{
  #region emergency
  public class EmergencyPackage
  {
    public bool IsChild { get; set; }
  }
  public class EmergencyOfficePackage
  {
    public string RoomCode { get; set; } = string.Empty;
  }
  public class EmergencyIsolationPackage
  {
    public List<string> BedCodes { get; set; } = new List<string>();
  }
  #endregion emergency

  #region IPD
  public class IcuPackage
  {
    public List<string> WardCodes { get; set; } = new List<string>();
  }
  public class WardRoomPackage
  {
    public int Floor { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string GetKey()
    {
      return $"{this.Floor}:{this.AreaCode}";
    }
  }
  #endregion IPD

  #region OPD
  public class InspectionPackage
  {
    public int Type { get; set; }
  }
  /// <summary>
  /// 진료실/검사실 모두 포함 가능
  /// DEPT_ROOMS 단위로 달라진다
  /// </summary>
  public class OpdRoomPackage
  {
    public bool IsSingle { get; set; }
    public List<DEPT_ROOMS> DeptRooms { get; set; } = new List<DEPT_ROOMS>();
    public class DEPT_ROOMS
    {
      public string DeptCode { get; set; } = string.Empty;
      public string RoomType { get; set; } = string.Empty;
      public List<string> RoomCodes { get; set; } = new List<string>();
      public string RoomCode => this.RoomCodes.First();
      public List<string> GetKeys()
      {
        return this.RoomCodes.Select(x => $"{this.DeptCode}:{x}").ToList();
      }
    }
    public void AddSingleRoom(string deptCode, string roomCode, string roomType)
    {
      this.IsSingle = true;
      this.DeptRooms.Add(new DEPT_ROOMS()
      {
        RoomType = roomType,
        DeptCode = deptCode,
        RoomCodes = new List<string>() { roomCode }
      });
    }
    public void AddMultiRooms(string deptCode, List<string> roomCodes, string roomType)
    {
      this.DeptRooms.Add(new DEPT_ROOMS()
      {
        RoomType = roomType,
        DeptCode = deptCode,
        RoomCodes = roomCodes
      });
    }
    public string CheckError()
    {
      if (this.DeptRooms.Count == 0)
      {
        return "No Data";
      }

      foreach (var dept in this.DeptRooms)
      {
        if (dept.RoomCodes.Count == 0)
        {
          return $"{dept.DeptCode} has no room";
        }
      }
      return string.Empty;
    }
  }

  public class AngPackage
  {
    // 1: 혈관조영실
    // 2: 혈관조영실 3층
    // 3: 심뇌혈관 조영실
    public ANG_TYPE Type { get; set; }
  }
  public class EndoPackage
  {
    public ENDO_TYPE Type { get; set; }
  }

  #endregion OPD
}