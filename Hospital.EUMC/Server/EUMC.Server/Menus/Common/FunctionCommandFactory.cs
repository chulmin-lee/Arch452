using UIControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.Server
{
  public class FunctionCommandFactory : FunctionCommandFactoryBase<GROUP_ID, FUNC_ID>
  {
    protected override MenuInformationCollectionBase<FUNC_ID> GetMenuInformationCollection()
    {
      return new MenuInformationCollection();
    }
    public override string FunctionName(FUNC_ID id)
    {
      var name = base.FunctionName(id);
      if (string.IsNullOrEmpty(name))
      {
        Debug.WriteLine($"{id} not found");
        return id.ToString();
      }
      return name;
    }


  }
}
