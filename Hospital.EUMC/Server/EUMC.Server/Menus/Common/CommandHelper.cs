using UIControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.Server
{
  public static class CommandHelper
  {
    static FunctionCommandFactory Factory;
    static Dictionary<MAINMENU_BAND_ID, string> BandNames;

    static CommandHelper()
    {
      BandNames = new Dictionary<MAINMENU_BAND_ID, string>()
      {
        { MAINMENU_BAND_ID.FILES, "Files"},
        { MAINMENU_BAND_ID.MANAGEMENT, "관리"},
        //{ MAINMENU_BAND_ID.SETTINGS, "Settings"},
        //{ MAINMENU_BAND_ID.HELP, "Test"},
      };

    }


    public static void Initialize()
    {
      Factory = new FunctionCommandFactory();
    }

    public static FunctionCommand<GROUP_ID, FUNC_ID> Create(
      FUNC_ID id,
      Action<FUNC_ID, FunctionItemBase<GROUP_ID, FUNC_ID>> exec,
      Predicate<object> when = null
      )
    {
      return Factory.CreateCommand(id, exec, when);
    }


    public static string BandName(MAINMENU_BAND_ID id)
    {
      BandNames.TryGetValue(id, out string name);

      if(string.IsNullOrEmpty(name))
      {
        name = id.ToString().ToLower().Replace("_", " ");
        name = name[0].ToString().ToUpper() + name.Substring(1);
      }
      return name;
    }
    public static string BandName(CONTEXT_BAND_ID id)
    {
      switch (id)
      {
        case CONTEXT_BAND_ID.NONE:
          {
            return null;
          }
        case CONTEXT_BAND_ID.CLIENT_MANAGE:
          {
            return "Client 관리";
          }
        default:
          {
            Debug.WriteLine($"Unknown Type: {id}");
            return id.ToString();
          }
      }
    }
    public static string GetURIName(string assembly, string loc)
    {
      return $"pack://application:,,,/{assembly};component/{loc}";
    }
  }
}
