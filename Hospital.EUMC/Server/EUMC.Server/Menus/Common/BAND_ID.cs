using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.Server
{
  public enum MAINMENU_BAND_ID
  {
    NONE,  // 항상 있어야 한다
    FILES,
    MANAGEMENT,
    //HELP,
  }

  public enum CONTEXT_BAND_ID
  {
    NONE,
    CLIENT_MANAGE,
  }
}
