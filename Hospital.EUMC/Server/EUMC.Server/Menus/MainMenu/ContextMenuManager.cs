using UIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.Server
{
  public class ContextMenuManager : ContextMenuManagerBase<CONTEXT_BAND_ID, GROUP_ID, FUNC_ID>
  {
    public ContextMenuManager(FunctionCollection<GROUP_ID, FUNC_ID> functions,
                              ContextMenuLayout layout, string name = "ContextMenu")
      : base(functions, layout, name)
    {

    }
  }

  public class ContextMenuLayout : MenuLayoutBase<CONTEXT_BAND_ID, GROUP_ID, FUNC_ID>
  {
    public ContextMenuLayout() : base(true, false) { }
  }
}
