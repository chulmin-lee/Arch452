using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.DataSource
{
  public class ParamBuilder
  {
    Dictionary<string, string> _dic = new Dictionary<string, string>();

    public Dictionary<string, string> Build()
    {
      return _dic;
    }
    public ParamBuilder Add(string name, string value = null)
    {
      _dic.Add(name, value ?? string.Empty);
      return this;
    }
  }
}
