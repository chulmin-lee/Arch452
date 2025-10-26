using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  /// <summary>
  /// Function의 이름/단축키에 대한 정보. 이름/단축키는 동적으로 변경 가능하다.
  /// 이 객체를 참조한 객체들에게 변경을 통지하기위해서 ViewModelBase 상속
  /// </summary>
  /// <typeparam name="F"></typeparam>
  public class MenuInformation<F> : ViewModelBase
    //where F : Enum
    where F : struct, IConvertible
  {
    public F FunctionId { get; private set; }
    //public MenuIconPack Icon { get; set; }  // menu/toolbar에서 사용하는 icon이 달라서 이렇게 하면 안된다.
    // 차라리 Icon ID라면 괜찮다.  // menu menu용 icon과 toolbar 이미지는 같은 모양을 가져야한다.
    // 이미 FunctionId가 있으니까 그걸쓰는게 낫겠다

    string _name;
    public string Name { get { return _name; } set { Set(ref _name, value); } }

    string _shortcuts;
    public string Shortcuts { get { return _shortcuts; } set { Set(ref _shortcuts, value); } }

    public MenuInformation(F id, string name, string shortcut = null)
    {
      this.FunctionId = id;
      this.Name = name;
      this.Shortcuts = shortcut;
      //this.Icon = icon;
    }
  }
}
