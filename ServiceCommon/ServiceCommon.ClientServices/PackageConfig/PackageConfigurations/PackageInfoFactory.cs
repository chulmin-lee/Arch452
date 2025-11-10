using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using static ServiceCommon.ClientServices.PlaylistMedical;

namespace ServiceCommon.ClientServices
{
  public static class PackageInfoFactory
  {
    #region Emergency
    public static PackageInfo ER_PATIENT(PlaylistSchedule s, bool child)
    {
      return new PackageInfo(s.PackageName, PACKAGE.ER_PATIENT)
      {
        Emergency = new EmergencyPackage { IsChild = child },
      };
    }
    public static PackageInfo ER_OFFICE(PlaylistSchedule s, string key)
    {
      var dic = s.Medical?.CustomRooms;
      if (dic != null)
      {
        var rooms = find_custom_rooms(dic, key);
        if (rooms.Any())
        {
          return new PackageInfo(s.PackageName, PACKAGE.ER_OFFICE)
          {
            EmergencyOffice = new EmergencyOfficePackage { RoomCode = rooms.First().RoomCode },
          };
        }
        else
        {
          return InvalidValue(s, $"unknown key: {key}");
        }
      }
      else
      {
        return ElementNotFound(s, $"No CustomRooms");
      }
    }
    public static PackageInfo ER_ISOLATION(PlaylistSchedule s, string key)
    {
      var dic = s.Medical?.CustomRooms;
      if (dic != null)
      {
        var rooms = find_custom_rooms(dic, key);
        if (rooms.Any())
        {
          return new PackageInfo(s.PackageName, PACKAGE.ER_ISOLATION)
          {
            EmergencyIsolation = new EmergencyIsolationPackage { BedCodes = rooms.Select(x => x.RoomCode).ToList() },
          };
        }
        else
        {
          return InvalidValue(s, $"unknown key: {key}");
        }
      }
      else
      {
        return ElementNotFound(s, $"No CustomRooms");
      }
    }
    static List<PlaylistMedical.ROOM> find_custom_rooms(Dictionary<string, List<PlaylistMedical.ROOM>> dic, string key)
    {
      if (dic.TryGetValue(key, out var value))
      {
        return value;
      }
      return new List<PlaylistMedical.ROOM>();
    }
    #endregion Emergency

    #region IPD
    public static PackageInfo ICU(PlaylistSchedule s)
    {
      var icus = s.Medical?.Icus;
      if (icus == null) return ElementNotFound(s, $"No Icus");

      return new PackageInfo(s.PackageName, PACKAGE.ICU)
      {
        Icu = new IcuPackage { WardCodes = icus.Select(x => x.DeptCode).ToList() },
      };
    }
    public static PackageInfo OPERATION(PlaylistSchedule s) => new PackageInfo(s.PackageName, PACKAGE.OPERATION);
    public static PackageInfo DELIVERY_ROOM(PlaylistSchedule s) => new PackageInfo(s.PackageName, PACKAGE.DELIVERY_ROOM);
    public static PackageInfo WARD_ROOMS(PlaylistSchedule s)
    {
      var wards = s.Medical?.WardRooms;
      if (wards == null) return ElementNotFound(s, $"No WardRooms");

      return new PackageInfo(s.PackageName, PACKAGE.WARD_ROOMS)
      {
        WardRoom = new WardRoomPackage
        {
          Floor = wards.Floor,
          AreaCode = wards.AreaCode,
        }
      };
    }
    #endregion IPD

    #region OPD
    public static PackageInfo OFFICE_SINGLE(PlaylistSchedule s) => single_opd_package(s, PACKAGE.OFFICE_SINGLE);
    public static PackageInfo EXAM_SINGLE(PlaylistSchedule s) => single_opd_package(s, PACKAGE.EXAM_SINGLE);
    static PackageInfo single_opd_package(PlaylistSchedule s, PACKAGE package)
    {
      var room = s.Medical?.DeptRooms?.FirstOrDefault()?.Rooms.FirstOrDefault();
      if (room == null) return ElementNotFound(s, "No DeptRooms");

      string room_type = "A";
      switch (package)
      {
        case PACKAGE.OFFICE_SINGLE: room_type = "A"; break;
        case PACKAGE.EXAM_SINGLE: room_type = "B"; break;
        default: throw new Exception($"Unknown package: {package}");
      }
      if (room.RoomType != room_type) return InvalidValue(s, $"Room type is not {room_type}");

      var rc = new PackageRoomConfig
      {
        DeptCode = room.DeptCode,
        DeptName = room.DeptName,
        RoomCode = room.RoomCode,
        RoomName = room.RoomName,
        RoomType = room.RoomType,
      };
      return new PackageInfo(s.PackageName, package)
      {
        OpdRoom = new OpdRoomPackage(rc),
        RoomConfigs = new List<PackageRoomConfig> { rc }
      };
    }

    public static PackageInfo OFFICE_MULTI(PlaylistSchedule s) => multi_opd_package(s, PACKAGE.OFFICE_MULTI);
    public static PackageInfo EXAM_MULTI(PlaylistSchedule s) => multi_opd_package(s, PACKAGE.EXAM_MULTI);
    public static PackageInfo EXAM_OFFICE_MIX(PlaylistSchedule s) => multi_opd_package(s, PACKAGE.EXAM_OFFICE_MIX);
    static PackageInfo multi_opd_package(PlaylistSchedule s, PACKAGE package)
    {
      var depts = s.Medical?.DeptRooms;
      if (depts == null) return ElementNotFound(s, "No DeptRooms");

      var dic = new Dictionary<string, List<PackageRoomConfig>>()
      {
        { "A", new List<PackageRoomConfig>() },
        { "B", new List<PackageRoomConfig>() },
      };
      var list = new List<PackageRoomConfig>();
      foreach (var dept in depts)
      {
        foreach (var room in dept.Rooms)
        {
          list.Add(new PackageRoomConfig
          {
            DeptCode = room.DeptCode,
            DeptName = room.DeptName,
            RoomCode = room.RoomCode,
            RoomName = room.RoomName,
            RoomType = room.RoomType,
          });
        }
      }

      switch (package)
      {
        case PACKAGE.OFFICE_MULTI:
          list = list.Where(x => x.RoomType == "A").ToList();
          break;
        case PACKAGE.EXAM_MULTI:
          list = list.Where(x => x.RoomType == "A").ToList();
          break;
        case PACKAGE.EXAM_OFFICE_MIX:
          break;

        default: throw new Exception($"Unknown package: {package}");
      }

      var opd = new OpdRoomPackage(list);
      var err = opd.CheckError();
      if (!string.IsNullOrEmpty(err))
      {
        return InvalidValue(s, err);
      }

      return new PackageInfo(s.PackageName, package)
      {
        OpdRoom = opd,
        RoomConfigs = list
      };
    }

    public static PackageInfo ENDO(PlaylistSchedule s) => new PackageInfo(s.PackageName, PACKAGE.ENDO);
    public static PackageInfo INSPECTION(PlaylistSchedule s, int type)
    {
      return new PackageInfo(s.PackageName, PACKAGE.INSPECTION)
      {
        Inspection = new InspectionPackage { Type = type }
      };
    }
    #endregion OPD

    #region ETC
    public static PackageInfo DRUG(PlaylistSchedule s) => new PackageInfo(s.PackageName, PACKAGE.DRUG);
    public static PackageInfo DR_SCH(PlaylistSchedule s) => new PackageInfo(s.PackageName, PACKAGE.DR_SCH);
    public static PackageInfo PROMOTION(PlaylistSchedule s) => new PackageInfo(s.PackageName, PACKAGE.PROMOTION);
    #endregion ETC

    #region Funeral
    public static PackageInfo FUNERAL_SINGLE(PlaylistSchedule s)
    {
      var funeral = s.Medical?.Funeral;
      if (funeral == null) return ElementNotFound(s, "No Funeral");

      return new PackageInfo(s.PackageName, PACKAGE.FUNERAL_SINGLE)
      {
        Funeral = new FuneralPackage()
        {
          RoomCodes = funeral.RoomCodes.Take(1).ToList()
        }
      };
    }
    public static PackageInfo FUNERAL_MULTI(PlaylistSchedule s)
    {
      var funeral = s.Medical?.Funeral;
      if (funeral == null) return ElementNotFound(s, "No Funeral");

      return new PackageInfo(s.PackageName, PACKAGE.FUNERAL_MULTI)
      {
        Funeral = new FuneralPackage()
        {
          RoomCodes = funeral.RoomCodes
        }
      };
    }
    #endregion Funeral

    static PackageInfo ElementNotFound(PlaylistSchedule s, string error)
    {
      return new PackageInfo(s.PackageName, PACKAGE_ERROR.ElementNotFound, error);
    }
    static PackageInfo InvalidValue(PlaylistSchedule s, string error)
    {
      return new PackageInfo(s.PackageName, PACKAGE_ERROR.InvalidValue, error);
    }
    public static PackageInfo UnknownPackage(PlaylistSchedule s)
    {
      return new PackageInfo(s.PackageName, PACKAGE_ERROR.UnknownPackage, $"{s.PackageName} not supported");
    }
    static PackageInfo NoSchedule()
    {
      return new PackageInfo("no", PACKAGE.NO_SCHEDULE, false);
    }
  }
}