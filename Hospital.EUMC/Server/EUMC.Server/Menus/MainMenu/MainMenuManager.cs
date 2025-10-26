using UIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.Server
{
  public class MainMenuManager : FunctionManagerBase<MAINMENU_BAND_ID, GROUP_ID, FUNC_ID>
  {
    public MainMenuManager(FunctionCollection<GROUP_ID, FUNC_ID> functions, MainMenuLayout layout,
                          string name = "MainMenu")
      : base(functions, layout, name)
    {
    }
  }


  public class MainMenuLayout : MenuLayoutBase<MAINMENU_BAND_ID, GROUP_ID, FUNC_ID>
  {
  }
}
