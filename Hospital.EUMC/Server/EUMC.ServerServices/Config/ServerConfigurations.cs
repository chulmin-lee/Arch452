using Common;
using System.IO;

namespace EUMC.ServerServices
{
  internal class ServerConfigurations
  {
    public string HspCode { get; set; }
    public bool TestMode { get; set; }
    public int HTTP_PORT { get; set; } = 40007;
    public string HTTP_HOME { get; set; } = @"C:\APM_Setup\didmate";
    public int SERVER_PORT { get; set; } = 30000;
    public bool GRPC_ENABLED { get; set; } = true;

    public static ServerConfigurations Load(string path)
    {
      ServerConfigurations config = null;
      if (File.Exists(path))
      {
        config = NewtonJson.Load<ServerConfigurations>(path);
      }

      if (config == null)
      {
        config = new ServerConfigurations();
        NewtonJson.Serialize(config, path);
      }
      return config;
    }

    //public List<DataConfig> GetDataConfigs()
    //{
    //  var list = new List<DataConfig>();

    //  var s = this.DataExtractor;
    //  foreach (var pi in s.GetType().GetProperties())
    //  {
    //    DataConfig? o = (DataConfig?)pi?.GetValue(s);
    //    if (o != null)
    //    {
    //      list.Add(o);
    //    }
    //  }
    //  return list;
    //}
    //public List<ServiceConfig> GetServiceConfigs()
    //{
    //  var list = new List<ServiceConfig>();

    //  var s = this.ServiceTransformer;
    //  foreach (var pi in s.GetType().GetProperties())
    //  {
    //    ServiceConfig? o = (ServiceConfig?)pi?.GetValue(s);
    //    if (o != null)
    //    {
    //      list.Add(o);
    //    }
    //  }
    //  return list;
    //}
  }
}