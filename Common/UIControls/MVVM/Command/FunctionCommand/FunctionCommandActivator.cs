using System;

namespace UIControls
{
  public class FunctionCommandActivator<F> where F : Enum
  {
    F _id;
    string _header;
    string _shortcut;
    Predicate<object> _can;
    Action<object> _exec;

    public FunctionCommandActivator(F id)
    {
      this._id = id;
    }
    public FunctionCommandActivator<F> Header(string header)
    {
      this._header = header;
      return this;
    }
    public FunctionCommandActivator<F> Shortcut(string shortcut)
    {
      this._shortcut = shortcut;
      return this;
    }
    public FunctionCommandActivator<F> When(Predicate<object> canExecute)
    {
      this._can = canExecute;
      return this;
    }
    public FunctionCommandActivator<F> Exec(Action<object> execute)
    {
      this._exec = execute;
      return this;
    }
    public FunctionCommand<F> Build()
    {
      return new FunctionCommand<F>(this._id, this._exec, this._can, this._header, this._shortcut);
    }
  }
}