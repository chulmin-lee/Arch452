using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{


  /// <summary>
  /// FunctionCommand를 생성하기 위한 helper
  /// 내부에 FunctionId에 대한 이름/단축키 정보를 가지고 있다.
  /// 상속받은 클래스에서 구현해야 한다.
  /// </summary>
  /// <typeparam name="G"></typeparam>
  /// <typeparam name="F"></typeparam>
  public abstract class FunctionCommandFactoryBase<G, F>
    //where G : Enum where F : Enum
  where G : struct, IConvertible where F : struct, IConvertible
  {
    MenuInformationCollectionBase<F> MenuCollection;

    public FunctionCommandFactoryBase()
    {
      MenuCollection = this.GetMenuInformationCollection();
    }
    protected abstract MenuInformationCollectionBase<F> GetMenuInformationCollection();

    public FunctionCommand<G, F> CreateCommand(F id, Action<F, FunctionItemBase<G, F>> exec, Predicate<object> when = null)
    {
      MenuInformation<F> info = MenuCollection.GetItem(id);
      if (info != null)
      {
        return new FunctionCommandActivator<G, F>(info)
                             .When(when)
                             .Exec(exec)
                             .Build();
      }
      else
      {
        return new FunctionCommandActivator<G, F>(id)
                             .When(when)
                             .Exec(exec)
                             .Header(this.FunctionName(id))
                             .Shortcut(this.FunctionShortcut(id))
                             .Build();
      }
    }

    public virtual string FunctionName(F id)
    {
      return MenuCollection.GetItem(id)?.Name;
    }
    public virtual string FunctionShortcut(F id)
    {
      return MenuCollection.GetItem(id)?.Shortcuts;
    }
  }
}
