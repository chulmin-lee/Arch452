using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  public class RelayCommand : GenericCommand<object>
  {
    public RelayCommand(Action<object> execute, string header = null, string shortcut = null) : this(execute, null, header, shortcut) { }
    public RelayCommand(Action<object> execute, Predicate<object> canExecute, string header = null, string shortcut = null)
      : base(execute, canExecute, header, shortcut)
    {
    }
  }
}
