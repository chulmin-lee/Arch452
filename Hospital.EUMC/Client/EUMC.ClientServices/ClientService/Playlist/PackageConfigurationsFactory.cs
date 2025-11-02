using Common;
using ServiceCommon;
using ServiceCommon.ClientServices;
using System.Collections.Generic;

namespace EUMC.ClientServices
{
  internal static class PackageConfigurationsFactory
  {
    public static PackageConfigurations Create(string xml, ILocation location)
    {
      if (string.IsNullOrEmpty(xml))
      {
        return new PackageConfigurations(PACKAGE_ERROR.FileNotFound);
      }

      var playlist = XmlHelper.Deserialize<PlaylistXmlFile>(xml);
      if (playlist == null)
      {
        LOG.ec("Playlist parsing failed");
        return new PackageConfigurations(PACKAGE_ERROR.FileNotFound);
      }
      if (playlist.dspconfig == null)
      {
        LOG.ec("Missing dspconfig element.");
        return new PackageConfigurations(PACKAGE_ERROR.MissingDisplayConfig);
      }

      var schedules = playlist.playlist?.Schedules;
      if (schedules == null)
      {
        LOG.ec("Missing Schedules element.");
        return new PackageConfigurations(PACKAGE_ERROR.MissingScheduleConfig);
      }

      var dsp = create_dsp(playlist.dspconfig);
      var medical = create_medical(playlist.medical_center);
      var scheduler = create_scheduler(dsp, schedules, medical);
      return new PackageConfigurations(scheduler, location);
    }

    #region parser
    static PackageScheduler create_scheduler(PlaylistDspConfig dsp, List<xml_schedule> schedules, PlaylistMedical medical)
    {
      var list = new List<PlaylistSchedule>();

      // mode: A,C 는 dspconfig의 시간을 사용한다. 그러므로 C는 A를 덮어쓴다. (같은 시간이므로)

      foreach (var sch in schedules)
      {
        var ranges = sch.mode == "B" ? TimeRange.ConvertToTimeRanges(sch.config.s_time, sch.config.e_time)
                                   : dsp.TimeRanges;
        foreach (var tr in ranges)
        {
          list.Add(create_schedule(sch, tr, dsp, medical));
        }
      }
      return new PackageScheduler(dsp.Holidays, list);
    }
    static PlaylistSchedule create_schedule(xml_schedule sch, TimeRange tr, PlaylistDspConfig dsp, PlaylistMedical medical)
    {
      var o = new PlaylistSchedule(sch.config.package, tr, medical);
      o.HospitalCode = medical.HospitalCode;
      o.No = sch.no.ToInt();
      o.Mode = sch.mode;

      switch (o.Mode)
      {
        case "A":
          {
            o.IsDefaultPackage = true;
            o.WeekdayRange = dsp.WeekdayRange;
          }
          break;
        case "B":
          {
            o.WeekdayRange = new WeekdayRange(sch.config.week_value.ToIntList());
          }
          break;
        case "C":
          {
            o.DateRange = new DateRange(sch.config.s_date, sch.config.e_date);
          }
          break;
      }

      //================================================
      // Notice
      //================================================
      {
        o.NoticeConfig = dsp.GetNoticeConfigClone(sch.UseTicker, sch.TickerMessage);
      }
      //================================================
      // TV
      //================================================
      {
        o.TVSetting = sch.GetTVSetting();
      }
      //================================================
      // contents
      //================================================
      o.RemoteContents = sch.GetRemoteFiles();

      //================================================
      // ETC
      //================================================
      o.MediaVolume = dsp.MediaVolume;
      o.SoundType = sch.GetSoundType();
      o.Duration = dsp.Duration;
      o.ShowDelayTime = sch.ShowDelayTime;
      return o;
    }

    static PlaylistMedical create_medical(xml_medical_center m)
    {
      if (m == null) return null;

      var medical = new PlaylistMedical()
      {
        HospitalCode = m.HospitalCode,
      };
      if (m.icus != null)
      {
        foreach (var p in m.icus.icu)
        {
          medical.AddIcu(p.IcuCode, p.IcuName);
        }
      }
      if (m.middle != null)
      {
        medical.AddDeptRoom(Room_Convert(m.middle));
      }
      if (m.large != null)
      {
        foreach (var p in m.large.middles)
        {
          medical.AddDeptRoom(Room_Convert(p));
        }
      }
      return medical;
    }
    static PlaylistMedical.ROOM Room_Convert(xml_med_middle p)
    {
      /*
          A, // 진료실 palyer : M, GET_PC_MED_ALL_RM_PT_INFO
          B, // 검사실 palyer : I, GET_PC_EXAM_STATE_PT_LIST
          C, // 검사실(일반촬영실) palyer : C, GET_PC_RAD_EXAM_PT_INFO_SEOUL
          D, // 검사실(초음파) palyer : D, GET_PC_EXAM_ROOM_PT_LIST
          E, // 진료실(CT/MRI) palyer : E, GET_PC_RAD_EXAM_PT_INFO_SEOUL
      */
      return new PlaylistMedical.ROOM
      {
        DeptCode = p.DeptCode,
        RoomCode = p.RoomCode,
        RoomName = p.RoomName,
        RoomType = p.RoomType == "A" ? "A" : "B",
      };
    }
    static PlaylistDspConfig create_dsp(xml_dspconfig o)
    {
      var dsp = new PlaylistDspConfig()
      {
        TimeRanges = TimeRange.ConvertToTimeRanges(o.time_value),
        WeekdayRange = new WeekdayRange(o.week_value.ToIntList()),
        Holidays = new Holidays(o.holiday_value.ToStringList(',')),
        Duration = o.duration.ToInt(10),
        MediaVolume = o.MediaVolumn(),
      };

      dsp.SetNoticeConfig(o.GetNoticeSetting());

      return dsp;
    }
    #endregion parser
  }
}