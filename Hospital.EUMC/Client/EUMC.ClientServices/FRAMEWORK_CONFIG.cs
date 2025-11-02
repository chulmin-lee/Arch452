using Common;
using System.Collections.Generic;

namespace EUMC.ClientServices
{
  public class FrameworkConfig
  {
    public List<int> DivsionIds { get; set; } = new List<int>();
    public List<int> CounterNumbers { get; set; } = new List<int>();
    public string CounterName { get; set; }
  }

  public static class FRAMEWORK_CONFIG
  {
    public static FrameworkConfig Config { get; private set; }
    public static void Initialize(string path)
    {
      var o = new ConfigurationHelper(path);
      Config = new FrameworkConfig()
      {
        DivsionIds = o.GetIntList("DIV_ID"),
        CounterNumbers = o.GetIntList("TERMINAL_WINDOW_NO"),
        CounterName = o.GetValue("TERMINAL_NAME")
      };

    }
  }
}
