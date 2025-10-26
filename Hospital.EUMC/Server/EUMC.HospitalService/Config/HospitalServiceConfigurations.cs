using Common;
using System.IO;

namespace EUMC.HospitalService
{
  internal class HospitalServiceConfigurations
  {
    public DataBaseSetting DataBase { get; set; } = new DataBaseSetting();
    public DataConfigurations DataExtractor { get; set; } = new DataConfigurations();
    public ServiceConfigurations MessageTransformer { get; set; } = new ServiceConfigurations();

    public static HospitalServiceConfigurations Load(string path)
    {
      HospitalServiceConfigurations config = null;
      if (File.Exists(path))
      {
        config = NewtonJson.Load<HospitalServiceConfigurations>(path);
      }

      if (config == null)
      {
        config = new HospitalServiceConfigurations()
        {
          DataExtractor = DataConfigurations.Factory(),
          MessageTransformer = ServiceConfigurations.Factory(),
        };
        NewtonJson.Serialize(config, path);
      }
      return config;
    }
  }

  internal class DataBaseSetting
  {
    public string Database { get; set; } = string.Empty;
  }
}