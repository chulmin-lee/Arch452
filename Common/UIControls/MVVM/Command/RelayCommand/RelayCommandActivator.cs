using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  public class RelayCommandActivator
  {
    string _header;
    string _shortcut;
    Predicate<object> _can;
    Action<object> _exec;

    public RelayCommandActivator()
    {
    }
    public RelayCommandActivator Header(string header)
    {
      this._header = header;
      return this;
    }
    public RelayCommandActivator Shortcut(string shortcut)
    {
      this._shortcut = shortcut;
      return this;
    }
    public RelayCommandActivator When(Predicate<object> canExecute)
    {
      this._can = canExecute;
      return this;
    }
    public RelayCommandActivator Exec(Action<object> execute)
    {
      this._exec = execute;
      return this;
    }

    public RelayCommand Build()
    {
      return new RelayCommand(this._exec, this._can, this._header, this._shortcut);
    }
  }
}
