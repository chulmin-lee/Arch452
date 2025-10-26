using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  public abstract class MenuInformationCollectionBase<F>
      //where F: Enum
      where F : struct, IConvertible
  {
    Dictionary<F, MenuInformation<F>> Collections { get; set; }
    public int Count => Collections.Count;

    public MenuInformationCollectionBase()
    {
      this.Collections = CreateCollection();
    }
    protected abstract Dictionary<F, MenuInformation<F>> CreateCollection();

    public MenuInformation<F> GetItem(F funcId)
    {
      Collections.TryGetValue(funcId, out MenuInformation<F> item);
      return item;
    }
    public void Clear()
    {
      Collections.Clear();
    }
  }
}
