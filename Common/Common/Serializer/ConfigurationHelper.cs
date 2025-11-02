using System.Collections.Generic;
using System.Configuration;

namespace Common
{
  public class ConfigurationHelper
  {
    Configuration _config;
    public ConfigurationHelper(string path)
    {
      var map = new ExeConfigurationFileMap() { ExeConfigFilename = path };
      _config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
    }
    public string GetValue(string key) =>_config.AppSettings.Settings[key]?.Value;
    public List<int> GetIntList(string key)
    {
      var s = _config.AppSettings.Settings[key]?.Value;
      if(!string.IsNullOrEmpty(s))
      {
        return s.ToIntList();
      }
      return new List<int>();
    }
    public List<string> GetStringList(string key)
    {
      var s = _config.AppSettings.Settings[key]?.Value;
      if (!string.IsNullOrEmpty(s))
      {
        return s.ToStringList();
      }
      return new List<string>();
    }
  }
}
