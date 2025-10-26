using System;

namespace UIControls
{
  public class FunctionCommandActivator<G, F>
    //where G : Enum where F : Enum
  where G : struct, IConvertible where F : struct, IConvertible
  {
    F _functionId;
    string _header;
    string _shortcut;
    Predicate<object> _can;
    Action<F, FunctionItemBase<G,F>> _exec;
    MenuInformation<F> _info;

    // 한꺼번에 설정
    public FunctionCommandActivator(MenuInformation<F> info)
    {
      this._info = info;
    }

    // 개별 설정
    public FunctionCommandActivator(F functionId)
    {
      this._functionId = functionId;
    }
    public FunctionCommandActivator<G, F> Header(string header)
    {
      this._header = header;
      return this;
    }
    public FunctionCommandActivator<G, F> Shortcut(string shortcut)
    {
      this._shortcut = shortcut;
      return this;
    }

    public FunctionCommandActivator<G, F> When(Predicate<object> canExecute)
    {
      this._can = canExecute;
      return this;
    }
    public FunctionCommandActivator<G, F> Exec(Action<F, FunctionItemBase<G, F>> execute)
    {
      this._exec = execute;
      return this;
    }
    public FunctionCommand<G, F> Build()
    {
      if (_info != null)
      {
        return new FunctionCommand<G, F>(this._info, this._exec, this._can);
      }
      else
      {
        if (string.IsNullOrEmpty(_header))
          throw new ArgumentNullException("header");

        return new FunctionCommand<G, F>(this._functionId, this._exec, this._can, this._header, this._shortcut);
      }
    }
  }
}