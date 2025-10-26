using UIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.Server
{
  public class MenuInformationCollection : MenuInformationCollectionBase<FUNC_ID>
  {
    protected override Dictionary<FUNC_ID, MenuInformation<FUNC_ID>> CreateCollection()
    {
      var list = new List<MenuInformation<FUNC_ID>>();
      // files
      {
        list.Add(new MenuInformation<FUNC_ID>(FUNC_ID.EXIT, "Exit", "Alt+F4"));
      }

      // 관리
      {
        list.Add(new MenuInformation<FUNC_ID>(FUNC_ID.CLIENT_UPDATE_ALL, "All Client Update"));
        list.Add(new MenuInformation<FUNC_ID>(FUNC_ID.CLIENT_VERSION_MATCH, "All Client Version Match"));
        list.Add(new MenuInformation<FUNC_ID>(FUNC_ID.SETTINGS, "Settings", "F9"));
      }

      var infos = new Dictionary<FUNC_ID, MenuInformation<FUNC_ID>>();

      foreach (var item in list)
      {
        infos.Add(item.FunctionId, item);
      }
      return infos;
    }
  }
}
