using Common;
using ServiceCommon.ClientServices;
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml.Serialization;

namespace EUMC.ClientServices
{

  [XmlRoot(ElementName = "Root")]
  public class PlaylistXmlFile
  {
    [XmlElement("dspconfig")] public xml_dspconfig dspconfig { get; set; }
    [XmlElement("medical_center")] public xml_medical_center medical_center { get; set; }
    [XmlElement("playlist")] public xml_playlist playlist { get; set; }
    [XmlElement("ticket_center")] public xml_ticket_center ticket { get; set; }
  }

  #region dspconfig
  public class xml_dspconfig
  {
    [XmlElement("holiday_value")] public string holiday_value { get; set; } = string.Empty; // "2021-02-11, 2021-02-11"
    [XmlElement("time_value")] public string time_value { get; set; } = string.Empty;    // ",07:00~20:00"
    [XmlElement("week_value")] public string week_value { get; set; } = string.Empty;    // 1,2,3,...
    [XmlElement("duration")] public string duration { get; set; } = string.Empty;
    [XmlElement("transition")] public string transition { get; set; } = string.Empty;
    [XmlElement("ticker_speed")] public string ticker_speed { get; set; } = string.Empty;
    [XmlElement("ticker_location")] public string ticker_location { get; set; } = string.Empty;
    [XmlElement("ticker_font_color")] public string ticker_font_color { get; set; } = string.Empty;
    [XmlElement("ticker_bg_color")] public string ticker_bg_color { get; set; } = string.Empty;
    [XmlElement("ticker_font_size")] public string ticker_font_size { get; set; } = string.Empty;
    [XmlElement("ticker_rss_use")] public string ticker_rss_use { get; set; } = string.Empty;
    [XmlElement("ticker_rss_url")] public string ticker_rss_url { get; set; } = string.Empty;
    [XmlElement("volume")] public string volume { get; set; } = string.Empty;

    // 순천향
    [XmlElement("common_ticker_use")] public string common_ticker_use { get; set; } = string.Empty;
    [XmlElement("common_ticker_msg")] public string common_ticker_msg { get; set; } = string.Empty;

    /// <summary>
    /// volume (0~31)값을 0~100 값으로 변환
    /// </summary>
    /// <returns></returns>
    public double MediaVolumn()
    {
      if (!double.TryParse(this.volume, out double vol))
      {
        vol = 0d;
      }
      if (vol > 31)
      {
        vol = 31d;
      }
      return vol / 31d;
    }

    public NoticeConfig GetNoticeSetting()
    {
      int speed = 12;
      if (!string.IsNullOrEmpty(this.ticker_speed))
      {
        switch (this.ticker_speed.ToLower())
        {
          case "veryfast": speed = 5; break;
          case "quick":
          case "fast": speed = 10; break;
          case "normal": speed = 12; break;
          case "slow": speed = 15; break;
          case "veryslow": speed = 20; break;
        }
      }

      // color
      var s = this.ticker_font_color;
      if (string.IsNullOrEmpty(s)) s = "white";
      var foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(s));

      s = this.ticker_bg_color;
      if (string.IsNullOrEmpty(s) || s == "transparent") s = "#16B1A9";
      var background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(s));

      // fontsize
      var size = 44;
      if (!string.IsNullOrEmpty(this.ticker_font_size))
      {
        switch (this.ticker_font_size.ToLower())
        {
          case "small": size = 35; break;
          case "normal": size = 44; break;
          case "big": size = 50; break;
        }
      }

      var common_notice = this.common_ticker_use.ToBoolean();
      var common_message = this.common_ticker_msg;

      return new NoticeConfig
      {
        ScrollSpeed = speed,
        Foreground = foreground,
        Background = background,
        FontSize = size,

        UseCommonNotice = common_notice,
        CommonNoticeMessage = common_message,
      };
    }
  }
  #endregion dspconfig

  #region medical
  public class xml_medical_center
  {
    [XmlElement("hsp_tp_cd")] public string HospitalCode { get; set; } = string.Empty;
    [XmlElement("large")] public xml_med_large large { get; set; }
    [XmlElement("middle")] public xml_med_middle middle { get; set; }
    [XmlElement("icu")] public List<xml_med_icu> icus { get; set; } = new List<xml_med_icu>();
    [XmlElement("strTitle")] public xml_med_title large_title { get; set; }
    [XmlElement("emergency")] public List<xml_med_middle> emergency { get; set; } = new List<xml_med_middle>(); // 응급 격리실
    [XmlElement("common")] public xml_med_common common { get; set; } // 약제과
    [XmlElement("ward_room")] public xml_ward_room ward_room { get; set; }
    [XmlElement("funeral")] public xml_funeral funeral { get; set; }
  }
  public class xml_med_middle
  {
    [XmlElement("dept_cd")] public string DeptCode = string.Empty;
    [XmlElement("dept_nm")] public string DeptName = string.Empty;
    [XmlElement("med_room_cd")] public string RoomCode = string.Empty;  // A 일때는 숫자, B일때는 NR001
    [XmlElement("room_no")] public string RoomName = string.Empty;
    [XmlElement("type")] public string RoomType = string.Empty;  // A: 진료실, B: 검사실, C:
    [XmlElement("dur_tm")] public string DurationTime = string.Empty;
    [XmlElement("strTitle")] public string Title = string.Empty;
  }
  public class xml_med_large
  {
    [XmlElement("group")] public List<xml_med_large_group> groups { get; set; } = new List<xml_med_large_group>();

    public class xml_med_large_group
    {
      [XmlElement("dept_cd")] public string DeptCode = string.Empty;
      [XmlElement("dept_nm")] public string DeptName = string.Empty;
      [XmlElement("middle")] public List<xml_med_middle> middles { get; set; } = new List<xml_med_middle>();
    }
  }
  public class xml_med_icu
  {
    [XmlElement("dept_cd")] public string DeptCode = string.Empty;
    [XmlElement("dept_nm")] public string DeptName = string.Empty;
    [XmlElement("strTitle")] public string strTitle = string.Empty;
  }
  public class xml_med_common
  {
    [XmlElement("strTitle")] public string strTitle = string.Empty;
  }
  public class xml_med_title
  {
    [XmlElement("strTitle")] public string strTitle { get; set; } = string.Empty;
  }
  public class xml_ward_room
  {
    [XmlElement("floor")] public string floor = string.Empty;
    [XmlElement("area")] public string area = string.Empty;
  }
  public class xml_funeral
  {
    [XmlElement("room_code")] public List<string> room_code = new List<string>();
  }
  #endregion medical

  #region playlist
  public class xml_playlist
  {
    [XmlElement("schedule")] public List<xml_schedule> Schedules { get; set; } = new List<xml_schedule>();
  }
  public class xml_schedule
  {
    [XmlAttribute] public string no { get; set; } = string.Empty;
    [XmlAttribute("mode")] public string mode { get; set; } = string.Empty;
    [XmlElement("config")] public xml_schedule_config config { get; set; } = new xml_schedule_config();
    [XmlElement("layout")] public xml_schedule_content layout { get; set; } = new xml_schedule_content();

    public bool UseTicker => this.config.ticker_use.ToBoolean();
    public string TickerMessage => this.config.ticker_msg;

    public TVSetting GetTVSetting() => this.config.GetTVSetting();
    public List<REMOTE_FILE> GetRemoteFiles() => this.layout.GetRemoteFiles();
    public PLAYLIST_SOUND_TYPE GetSoundType() => this.config.GetSoundType();

    public class xml_schedule_config
    {
      [XmlElement("s_date")] public string s_date { get; set; } = string.Empty;
      [XmlElement("e_date")] public string e_date { get; set; } = string.Empty;
      [XmlElement("s_time")] public string s_time { get; set; } = string.Empty;
      [XmlElement("e_time")] public string e_time { get; set; } = string.Empty;
      [XmlElement("week_value")] public string week_value { get; set; } = string.Empty;
      //[XmlElement("bgm")] public string bgm { get; set; } = string.Empty;
      [XmlElement("ticker_use")] public string ticker_use { get; set; } = string.Empty;
      [XmlElement("ticker_msg")] public string ticker_msg { get; set; } = string.Empty;
      [XmlElement("package")] public string package { get; set; } = string.Empty;
      //[XmlElement("perA")] public string perA { get; set; } = string.Empty;
      //[XmlElement("kiosk_use")] public string kiosk_use { get; set; } = string.Empty;
      //[XmlElement("kiosk_layout")] public string kiosk_layout { get; set; } = string.Empty;
      //[XmlElement("kiosk_time")] public string kiosk_time { get; set; } = string.Empty;
      //[XmlElement("kiosk_name")] public xml_kiosk_file kiosk_name { get; set; } = new xml_kiosk_file();
      //[XmlElement("text_use")] public string text_use { get; set; } = string.Empty;
      [XmlElement("delaytime_use")] public string delaytime_use { get; set; } = string.Empty;
      [XmlElement("sound_type")] public string sound_type { get; set; } = string.Empty;
      [XmlElement("tv_use")] public string tv_use { get; set; } = string.Empty;
      [XmlElement("channel")] public string tv_channel { get; set; } = string.Empty;
      // 순천향
      [XmlElement("contents_use")] public string contents_use { get; set; } = string.Empty;
      [XmlElement("delay_person")] public string delay_person { get; set; } = string.Empty;

      public TVSetting GetTVSetting()
      {
        bool use_tv = this.tv_use.ToBoolean();
        int channel_no = 0;
        string channel_name = string.Empty;

        // 형식: "111,KBS1"
        var arr = this.tv_channel.ToList(',');
        for (int i = 0; i < arr.Count; i++)
        {
          switch (i)
          {
            case 0: channel_no = arr[0].ToInt(); break;
            case 1: channel_name = arr[1]; break;
          }
        }
        use_tv = use_tv && channel_no > 0;

        return new TVSetting
        {
          UseTV = use_tv,
          TvChannelNo = channel_no,
          TvChannelName = channel_name,
        };
      }

      public PLAYLIST_SOUND_TYPE GetSoundType()
      {
        switch (this.sound_type)
        {
          case "1": return PLAYLIST_SOUND_TYPE.BELL;
          case "2": return PLAYLIST_SOUND_TYPE.SPEECH;
          case "3": return PLAYLIST_SOUND_TYPE.BELL_SPEECH;
          default: return PLAYLIST_SOUND_TYPE.NONE;
        }
      }
    }

    public class xml_schedule_content
    {
      [XmlAttribute] public string no { get; set; } = string.Empty;
      [XmlElement("file")] public List<xml_file_info> files { get; set; } = new List<xml_file_info>();

      public class xml_file_info
      {
        [XmlAttribute] public long size { get; set; }
        [XmlAttribute] public int id { get; set; }
        [XmlText] public string file { get; set; } = string.Empty;
      }

      public List<REMOTE_FILE> GetRemoteFiles()
      {
        var list = new List<REMOTE_FILE>();
        foreach (var file in this.files)
        {
          if (file.size > 0 && !string.IsNullOrEmpty(file.file))
          {
            list.Add(new REMOTE_FILE(file.file.Trim(), file.size, file.id));
          }
        }
        return list;
      }
    }
  }
  #endregion playlist

  #region ticket
  public class xml_ticket_center
  {
    [XmlElement("generator")] public ticket_client generator { get; set; }
    [XmlElement("dislay")] public ticket_client dislay { get; set; }
    [XmlElement("caller")] public ticket_client caller { get; set; }
    [XmlElement("large")] public ticket_client large { get; set; }
    public class ticket_client
    {
      [XmlElement("div")] public List<xml_division> divisions { get; set; } = new List<xml_division>();
    }

    public class xml_division
    {
      [XmlElement("div_id")] public string div_id { get; set; } = string.Empty;
      [XmlElement("div_nm")] public string div_nm { get; set; } = string.Empty;
      [XmlElement("div_desc")] public string div_desc { get; set; } = string.Empty;
      [XmlElement("wnd")] public List<xml_window> windows { get; set; } = new List<xml_window>();
    }
    public class xml_window
    {
      [XmlElement("div_id")] public string div_id { get; set; } = string.Empty;
      [XmlElement("wnd_no")] public string wnd_no { get; set; } = string.Empty;
      [XmlElement("wnd_nm")] public string wnd_nm { get; set; } = string.Empty;
      [XmlElement("position")] public string position { get; set; } = string.Empty;
    }
  }

  #endregion ticket
}