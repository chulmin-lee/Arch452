using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  public class ToggleCommandActivator
  {
    string _onHeader;
    string _offHeader;
    string _shortcut;
    Predicate<object> _can;
    Action<object> _exec;

    public ToggleCommandActivator()
    {
    }
    public ToggleCommandActivator Header(string header)
    {
      this._onHeader = header;
      return this;
    }
    public ToggleCommandActivator OffHeader(string header)
    {
      this._offHeader = header;
      return this;
    }
    public ToggleCommandActivator Shortcut(string shortcut)
    {
      this._shortcut = shortcut;
      return this;
    }
    public ToggleCommandActivator When(Predicate<object> canExecute)
    {
      this._can = canExecute;
      return this;
    }
    public ToggleCommandActivator Exec(Action<object> execute)
    {
      this._exec = execute;
      return this;
    }

    public ToggleCommand Build()
    {
      return new ToggleCommand(this._exec, this._can, this._onHeader, this._offHeader, this._shortcut);
    }
  }
}
