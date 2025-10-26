using System;
using System.Collections.Generic;
using System.Linq;

namespace UIControls
{
  // band는 1개
  public class ContextMenuManagerBase<B,G,F> : FunctionManagerBase<B,G,F>
    //where B : Enum where G : Enum where F : Enum
    where B : struct, IConvertible where G : struct, IConvertible where F : struct, IConvertible
  {
    public List<FunctionItemCore> ContextMenu => BandHeaders.First().Value.Children.ToList();

    public ContextMenuManagerBase(FunctionCollection<G, F> functions, MenuLayoutBase<B, G, F> layout, string name = "ContextMenu")
      : base(functions, layout, name)
    {
    }
  }
}
