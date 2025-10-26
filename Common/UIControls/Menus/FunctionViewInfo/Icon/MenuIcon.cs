using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  /// <summary>
  /// menu에서 사용할 Icon을 설정한다.
  /// 메뉴 타입에 따라 사용하는 Icon 크기가 다르므로 메뉴별로 생성해야 한다.
  /// </summary>
  /// <typeparam name="F"></typeparam>
  public class MenuIcon<F>
  //where F : Enum
  where F : struct, IConvertible
  {
    public F FunctionId { get; private set; }
    public string Name{ get; set; }
    public Uri Uri { get; set; }

    public MenuIcon(F id, string name)
    {
      this.FunctionId = id;
      this.Name = name;
    }
    public MenuIcon(F id, Uri uri)
    {
      this.FunctionId = id;
      this.Uri = uri;
    }
  }
}
