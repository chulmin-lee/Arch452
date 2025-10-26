using Common;

namespace EUMC.ServerServices
{
  public partial class ServerService
  {
    /*
    public void RunPackage(string package)
    {
      var o = _playlists[package];

      XmlHelper.Save(o, _playlistPath);

      this.SendAll(ServerMessage.ClientConfigUpdate);
    }

    Dictionary<string, PlaylistXmlFile> _playlists = new Dictionary<string, PlaylistXmlFile>();
    string _playlistPath = @"c:\APM_Setup\didmate\playlist\didmate\2\playlist.xml";
    void playlist_initialize()
    {
      _playlists.Clear();

      // h0
      #region h0
      {
        string package = "h0";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center
        {
          middle = new xml_med_middle
          {
            DeptCode = "WGO",
            DeptName = "WGO",
            RoomCode = "1",
            RoomName = "진료실1",
            RoomType = "A",
            DurationTime = "10",
            strTitle = "WGO 진료실1",
          }
        };
        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "WGO 진료실1 대기중입니다.",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion h0

      // h1
      #region h1
      {
        string package = "h1";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center()
        {
          large = new xml_med_large()
        };

        var large = o.medical_center.large;
        large.groups.Add(new xml_med_large.xml_med_large_group()
        {
          DeptCode = "WGO",
          DeptName = "WGO",
          middles = new List<xml_med_middle>
          {
            new xml_med_middle()
            {
              DeptCode = "WGO",
              DeptName = "WGO",
              RoomCode = "1",
              RoomName = "진료실1",
              RoomType = "A",
              DurationTime = "10",
              strTitle = "WGO 진료실1",
            },
            new xml_med_middle()
            {
              DeptCode = "WGO",
              DeptName = "WGO",
              RoomCode = "2",
              RoomName = "진료실2",
              RoomType = "A",
              DurationTime = "10",
              strTitle = "WGO 진료실2",
            },
            new xml_med_middle()
            {
              DeptCode = "WGO",
              DeptName = "WGO",
              RoomCode = "3",
              RoomName = "진료실3",
              RoomType = "A",
              DurationTime = "10",
              strTitle = "WGO 진료실3",
            },
            new xml_med_middle()
            {
              DeptCode = "WGO",
              DeptName = "WGO",
              RoomCode = "4",
              RoomName = "진료실4",
              RoomType = "A",
              DurationTime = "10",
              strTitle = "WGO 진료실4",
            }
          }
        });

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "WGO 진료실 대대합.",
            Package = $"{package}",
            delay_person = "7"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion h1

      // h2
      #region h2
      {
        string package = "h2";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center
        {
          middle = new xml_med_middle
          {
            DeptCode = "CCC",
            DeptName = "CCC",
            RoomCode = "1",
            RoomName = "검사실1",
            RoomType = "B",
            DurationTime = "10",
            strTitle = "CCC 검사실1",
          }
        };
        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "CCC 검사실1 대기중입니다.",
            Package = $"{package}",
            delay_person = "4"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion h2

      // h3
      #region h3
      {
        string package = "h3";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center()
        {
          large = new xml_med_large()
        };

        var large = o.medical_center.large;
        large.groups.Add(new xml_med_large.xml_med_large_group()
        {
          DeptCode = "CCC",
          DeptName = "CCC",
          middles = new List<xml_med_middle>
          {
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "1",
              RoomName = "검사실1",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실1",
            },
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "2",
              RoomName = "검사실2",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실2",
            },
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "3",
              RoomName = "검사실3",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실3",
            },
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "4",
              RoomName = "검사실4",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실4",
            }
          }
        });

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "CCC 검사실 대대합.",
            Package = $"{package}",
            delay_person = "4"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion h3

      // h4 (PET)
      #region h4
      {
        string package = "h4";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center()
        {
          large = new xml_med_large()
        };

        var large = o.medical_center.large;
        large.groups.Add(new xml_med_large.xml_med_large_group()
        {
          DeptCode = "CCC",
          DeptName = "CCC",
          middles = new List<xml_med_middle>
          {
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "1",
              RoomName = "검사실1",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실1",
            },
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "2",
              RoomName = "검사실2",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실2",
            },
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "3",
              RoomName = "검사실3",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실3",
            },
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "4",
              RoomName = "검사실4",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실4",
            }
          }
        });

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "CCC 검사실 대대합.",
            Package = $"{package}",
            delay_person = "4"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion h4

      // h5 (exam/office mixed)
      #region h5
      {
        string package = "h5";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center()
        {
          large = new xml_med_large()
        };

        var large = o.medical_center.large;

        large.groups.Add(new xml_med_large.xml_med_large_group()
        {
          DeptCode = "WGO",
          DeptName = "WGO",
          middles = new List<xml_med_middle>
          {
            new xml_med_middle()
            {
              DeptCode = "WGO",
              DeptName = "WGO",
              RoomCode = "1",
              RoomName = "진료실1",
              RoomType = "A",
              DurationTime = "10",
              strTitle = "WGO 진료실1",
            },
            new xml_med_middle()
            {
              DeptCode = "WGO",
              DeptName = "WGO",
              RoomCode = "2",
              RoomName = "진료실2",
              RoomType = "A",
              DurationTime = "10",
              strTitle = "WGO 진료실2",
            },
            new xml_med_middle()
            {
              DeptCode = "WGO",
              DeptName = "WGO",
              RoomCode = "3",
              RoomName = "진료실3",
              RoomType = "A",
              DurationTime = "10",
              strTitle = "WGO 진료실3",
            },
            new xml_med_middle()
            {
              DeptCode = "WGO",
              DeptName = "WGO",
              RoomCode = "4",
              RoomName = "진료실4",
              RoomType = "A",
              DurationTime = "10",
              strTitle = "WGO 진료실4",
            }
          }
        });

        large.groups.Add(new xml_med_large.xml_med_large_group()
        {
          DeptCode = "CCC",
          DeptName = "CCC",
          middles = new List<xml_med_middle>
          {
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "1",
              RoomName = "검사실1",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실1",
            },
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "2",
              RoomName = "검사실2",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실2",
            },
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "3",
              RoomName = "검사실3",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실3",
            },
            new xml_med_middle()
            {
              DeptCode = "CCC",
              DeptName = "CCC",
              RoomCode = "4",
              RoomName = "검사실4",
              RoomType = "B",
              DurationTime = "10",
              strTitle = "CCC 검사실4",
            }
          }
        });

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "CCC 검사실 대대합.",
            Package = $"{package}",
            delay_person = "4"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion h5

      #region i0 (중환자실 staff)
      {
        string package = "i0";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center
        {
          icus = new List<xml_med_icu>()
          {
                      new xml_med_icu()
            {
              DeptCode = "A019",
              DeptName = "외과계 중환자실 (SICU)",
            },
            new xml_med_icu()
            {
              DeptCode = "A016",
              DeptName = "내과계 중환자실 (MICU)",
            },
            new xml_med_icu()
            {
              DeptCode = "A034",
              DeptName = "심장계 중환자실 (CCU)",
            }
          }
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "중환자실 staff",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion i0 (중환자실 staff)

      #region i1 (중환자실 보호자용)
      {
        string package = "i1";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center
        {
          icus = new List<xml_med_icu>()
          {
                      new xml_med_icu()
            {
              DeptCode = "A019",
              DeptName = "외과계 중환자실 (SICU)",
            },
            new xml_med_icu()
            {
              DeptCode = "A016",
              DeptName = "내과계 중환자실 (MICU)",
            },
            new xml_med_icu()
            {
              DeptCode = "A034",
              DeptName = "심장계 중환자실 (CCU)",
            }
          }
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "중환자실 보호자용",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion i1 (중환자실 보호자용)

      #region i5 (중환자실 신생아)
      {
        string package = "i5";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center
        {
          icus = new List<xml_med_icu>()
          {
            new xml_med_icu()
            {
              DeptCode = "A019",
              DeptName = "외과계 중환자실 (SICU)",
            },
            new xml_med_icu()
            {
              DeptCode = "A016",
              DeptName = "내과계 중환자실 (MICU)",
            },
            new xml_med_icu()
            {
              DeptCode = "A034",
              DeptName = "심장계 중환자실 (CCU)",
            }
          }
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "중환자실 신생아",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion i5 (중환자실 신생아)

      #region i2 (수술실)
      {
        string package = "i2";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "수술실",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion i2 (수술실)

      #region i3 (응급실/성인)
      {
        string package = "i3";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "응급실/성인",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion i3 (응급실/성인)

      #region i4 (응급실/소아)
      {
        string package = "i4";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "응급실/소아",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion i4 (응급실/소아)

      #region i6 (응급실 진료실(호출)
      {
        string package = "i6";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center
        {
          emergency_isolation = new List<xml_med_middle>()
          {
            new xml_med_middle()
            {
              DeptCode = "A001",
              DeptName = "응급실 격리실",
              RoomCode = "1",
              RoomName = "격리실1",
              RoomType = "C",
              DurationTime = "10",
              strTitle = "응급실 격리실1",
            },
            new xml_med_middle()
            {
              DeptCode = "A002",
              DeptName = "응급실 격리실",
              RoomCode = "2",
              RoomName = "격리실2",
              RoomType = "C",
              DurationTime = "10",
              strTitle = "응급실 격리실2",
            }
          }
        };
        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "응급실 진료실(호출)",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion i6 (응급실 진료실(호출)

      #region i7 (응급실 격리실)
      {
        string package = "i7";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center
        {
          emergency_isolation = new List<xml_med_middle>()
          {
            new xml_med_middle()
            {
              RoomCode = "A001",
              RoomName = "신경 진료실",
            },
            new xml_med_middle()
            {
              RoomCode = "A002",
              RoomName = "신경 진료실",
            }
          }
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "응급실 격리실",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion i7 (응급실 격리실)

      #region i8 (응급실 격리실)
      {
        string package = "i8";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };
        o.medical_center = new xml_medical_center
        {
          emergency_isolation = new List<xml_med_middle>()
          {
          new xml_med_middle()
            {
              RoomCode = "A001",
              RoomName = "신경 진료실",
            },
            new xml_med_middle()
            {
              RoomCode = "A002",
              RoomName = "신경 진료실",
            }
          }
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "응급실 격리실",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion i8 (응급실 격리실)

      #region w0 (약제과)
      {
        string package = "w0";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "3",
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "약제과",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion w0 (약제과)

      #region w1 (진단검사(환자별))
      {
        string package = "w1";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "진단검사(환자별)",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion w1 (진단검사(환자별))

      #region w2 (진단검사(시간별))
      {
        string package = "w2";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "진단검사(시간별)",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion w2 (진단검사(시간별))

      #region w3 (진료스케쥴)
      {
        string package = "w3";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "10",
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "Y",
            NoticeMessage = "진료스케쥴",
            Package = $"{package}",
            delay_person = "5"
          },
        });

        _playlists.Add(package, o);
      }
      #endregion w3 (진료스케쥴)

      #region a0 (홍보)
      {
        string package = "a0";
        var o = new PlaylistXmlFile();
        o.dspconfig = new xml_dspconfig
        {
          week_value = "1,2,3,4,5,6,0",
          duration = "3",
        };

        o.playlist = new xml_playlist();
        o.playlist.Schedules.Add(new xml_schedule()
        {
          no = "0",
          mode = "A",
          config = new xml_schedule.xml_schedule_config()
          {
            UseNotice = "N",
            Package = $"{package}",
            delay_person = "5"
          },
          layout = new xml_schedule.xml_schedule_content()
          {
            no = "0",
            files = new List<xml_schedule.xml_schedule_content.xml_file_info>
            {
              new xml_schedule.xml_schedule_content.xml_file_info() { size=56915,  file="2/img.png" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=18155,  file="2/doctor-default.jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=102440, file="36/1층원무접수·수납창구업무시간안내_220523대대합용001.jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=190630, file="36/의무기록순번발권기_210902001.jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=157673, file="36/의무기록제증명발급구비서류_220107.jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=173239, file="36/의무기록제증명발급구비서류_220107(2).jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=127984, file="36/24_일반진단서·소견서_원무창구이용안내001.jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=152649, file="36/예약검사(채혈)비수납안내001.jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=145878, file="36/알림톡진료비수납안내250522.jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=158211, file="6/현금ic카드결제시혜택001.jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=160618, file="36/1_가로형1_1920x1080px-수정본.png" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=116100, file="36/본인확인신분증제시안내(수정)240430.jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=154499, file="36/탄소중립.jpg" },
              new xml_schedule.xml_schedule_content.xml_file_info() { size=73484,  file="36/고객응대근로자보호포스터(가로형)(수정본).jpg" },
            }
          },
        });

        _playlists.Add(package, o);
      }
      #endregion a0 (홍보)
    }
    */
  }
}