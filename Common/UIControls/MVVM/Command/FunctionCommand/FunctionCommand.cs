using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  public class FunctionCommand<F> : RelayCommand where F : Enum
  {
    public F FunctionId { get; private set; }
    public FunctionCommand(F functionId, Action<object> execute, string header, string shortcut = null)
      : base(execute, header, shortcut)
    {
      this.FunctionId = functionId;
    }

    public FunctionCommand(F functionId, Action<object> execute, Predicate<object> canExecute, string header, string shortcut = null)
      : base(execute, canExecute, header, shortcut)
    {
      this.FunctionId = functionId;
    }
  }
}
