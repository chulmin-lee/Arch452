using Common;
using System.IO;

namespace EUMC.HospitalService
{

  internal class HospitalServiceConfigurations
  {
    public string HSP_TP_CD { get; set; }
    public DataBaseSetting DataBase { get; set; }
    public DataConfigurations DataExtractor { get; set; }
    public ServiceConfigurations MessageTransformer { get; set; }

    public static HospitalServiceConfigurations Load(string path, string hspCode = "01", bool test_mode = true)
    {
      bool seoul = hspCode == "01";
      HospitalServiceConfigurations config = null;
      if (File.Exists(path))
      {
        config = NewtonJson.Load<HospitalServiceConfigurations>(path);
      }

      if (config == null)
      {
        var db = new DataBaseSetting
        {
          TestMode = test_mode,
          XMED = "data source=ehis02.eumc.ac.kr:1526/HIS; password=E5!his_Xmed; user id=XMED; Incr Pool Size=5; Decr Pool Size=2;",
          XEDP = "data source=ehis02.eumc.ac.kr:1526/HIS; password=E6#dp_Xedp; user id=XEDP; Incr Pool Size=5; Decr Pool Size=2;",
          XSUP = "data source=ehis02.eumc.ac.kr:1526/HIS; password=E5!his_Xsup; user id=XSUP; Incr Pool Size=5; Decr Pool Size=2;",
        };

        if(db.TestMode)
        {
          db.MYSQL = "server=127.0.0.1;uid=root;pwd=dmscksdmsco*963..;database=didmate;SslMode=none;Connect Timeout=5;charset=utf8;";
          db.MSSQL = "data source=127.0.0.1;uid=sa;pwd=3dnjf5dlf;Persist Security Info=true;Initial Catalog=didmate;Connection Timeout=5";
        }
        else
        {
      
          db.MYSQL = seoul ? "server=172.17.10.33;uid=root;pwd=dmscksdmsco*963..;database=didmate;SslMode=none;Connect Timeout=5"
                           : "server=172.17.10.36;uid=root;pwd=dmscksdmsco*963..;database=didmate;SslMode=none;Connect Timeout=5";
          db.MSSQL = seoul ? "data source=172.17.10.33;uid=sa;pwd=neovision0620;Persist Security Info=true;Initial Catalog=didmate;Connection Timeout=5;Max Pool Size=1000"
                           : "data source=172.17.10.36;uid=sa;pwd=neovision0620;Persist Security Info=true;Initial Catalog=didmate;Connection Timeout=5;Max Pool Size=1000";
        }

        config = new HospitalServiceConfigurations()
        {
          DataBase = db,
          DataExtractor = DataConfigurations.Factory(seoul),
          MessageTransformer = ServiceConfigurations.Factory(seoul),
        };
        NewtonJson.Serialize(config, path);
      }
      return config;
    }
  }

  internal class DataBaseSetting
  {
    public bool TestMode { get; set; }
    public string XMED { get; set; } = string.Empty;
    public string XEDP { get; set; } = string.Empty;
    public string XSUP { get; set; } = string.Empty;
    public string MSSQL { get; set; } = string.Empty;
    public string MYSQL { get; set; } = string.Empty;
  }
}