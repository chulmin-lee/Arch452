using System;

namespace UIControls
{
  public class MenuCommandActivator
  {
    int _id;
    string _header;
    string _shortcut;
    Predicate<object> _can;
    Action<object> _exec;

    public MenuCommandActivator()
    {
    }
    public MenuCommandActivator ID(int id)
    {
      this._id = id;
      return this;
    }
    public MenuCommandActivator Header(string header)
    {
      this._header = header;
      return this;
    }
    public MenuCommandActivator Shortcut(string shortcut)
    {
      this._shortcut = shortcut;
      return this;
    }
    public MenuCommandActivator When(Predicate<object> canExecute)
    {
      this._can = canExecute;
      return this;
    }
    public MenuCommandActivator Exec(Action<object> execute)
    {
      this._exec = execute;
      return this;
    }
    public MenuCommand Build()
    {
      return new MenuCommand(this._id, this._exec, this._can, this._header, this._shortcut);
    }
  }
}
