using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.HospitalService
{
  internal static partial class DataMapper
  {
    #region Emergency
    /// <summary>
    /// 응급실 구역이름으로 child 여부 판단
    /// </summary>
    /// <param name="er_area_name">성인/소아</param>
    /// <returns></returns>
    static bool ErIsChild(string er_area_name)
    {
      return er_area_name == "소아" ? true : false;
    }
    /// <summary>
    /// 응급 CT실 상태
    /// </summary>
    /// <param name="s">A:검사중, B: 처방받음</param>
    /// <returns></returns>
    static bool ErIsExam(string s)
    {
      return s == "A" ? true : false;
    }
    #endregion

    #region IPD
    static OPERATION_STATE OperationState(string s)
    {
      switch(s)
      {
        case "1": return OPERATION_STATE.Waiting;
        case "2": return OPERATION_STATE.Operating;
        case "3": return OPERATION_STATE.RecoveryRoom;
        case "4": return OPERATION_STATE.WardRoom;
        case "5": return OPERATION_STATE.IcuRoom;
      }
      return OPERATION_STATE.None;
    }
    #endregion

    #region Office
    static string OfficeFloor(string s)
    {
      string floor = null;
      if (!string.IsNullOrEmpty(s))
      {
        if (int.TryParse(s, out int value))
        {
          floor = $"{value}층";
        }
        else if (s.StartsWith("B"))
        {
          if (int.TryParse(s.Substring(1), out int v))
          {
            floor = $"지하{v}층";
          }
        }
        else if (s.StartsWith("W"))
        {
          if (int.TryParse(s.Substring(1), out int v))
          {
            floor = $"{v}층";
          }
        }
      }
      return floor;
    }
    #endregion


    // EUMC
    static ANG_STATE AngState(int state)
    {
      switch (state)
      {
        case 0: return ANG_STATE.Init;
        case 1: return ANG_STATE.Preparing;
        case 2: return ANG_STATE.Prepared;
        case 3: return ANG_STATE.CallPatient;
        case 4: return ANG_STATE.Scheduled;
        case 5: return ANG_STATE.During;
        case 6: return ANG_STATE.Completed;
        case 7: return ANG_STATE.Cancelled;
        case 8: return ANG_STATE.WardPreparation;
        case 9: return ANG_STATE.ScheduledToEnd;
      }
      return ANG_STATE.Init;
    }

    static string CommaStringToNewLineString(string s)
    {
      return s.Replace(",", Environment.NewLine);
    }

    static bool StringToBoolean(string source)
    {
      return source == "y" || source == "Y" | source == "1";
    }

    static DayOfWeek ToDayOfWeek(int i)
    {
      switch (i)
      {
        case 0: return DayOfWeek.Sunday;
        case 1: return DayOfWeek.Monday;
        case 2: return DayOfWeek.Tuesday;
        case 3: return DayOfWeek.Wednesday;
        case 4: return DayOfWeek.Thursday;
        case 5: return DayOfWeek.Friday;
        case 6: return DayOfWeek.Saturday;
        default:
          throw new ArgumentOutOfRangeException(nameof(i), "Invalid day of week value");
      }
    }
  }
}
