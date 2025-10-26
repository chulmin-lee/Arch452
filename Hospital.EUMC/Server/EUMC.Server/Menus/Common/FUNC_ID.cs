using UIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.Server
{
  public enum GROUP_ID
  {
    NONE,
  }

  public enum FUNC_ID
  {
    NONE,  // 항상 있어야 한다.

    // files
    [Function(FUNCTION_TYPE.ACTION)]
    EXIT,

    // 관리
    [Function(FUNCTION_TYPE.ACTION)]
    CLIENT_UPDATE_ALL,
    [Function(FUNCTION_TYPE.ACTION)]
    CLIENT_VERSION_MATCH,
    [Function(FUNCTION_TYPE.ACTION)]
    SETTINGS,
  }
}
