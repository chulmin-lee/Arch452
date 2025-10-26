using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace UIControls
{
  // Type 해당하는 Control을 생성해서 돌려준다.
  // 한번 생성된 control은 재활용된다.
  public class ControlCommand<T> : GenericCommand<T> where T : UserControl, new()
  {
    object _instance;
    public ControlCommand(Action<T> execute, string header = null, string shortcut = null) : this(execute, null, header, shortcut) { }
    public ControlCommand(Action<T> execute, Predicate<T> canExecute, string header = null, string shortcut = null)
      :base(execute, canExecute, header, shortcut)
    {
    }

    #region ICommand
    public override void Execute(object parameter)
    {
      if (_instance == null)
      {
        _instance = new T();
        var c = (_instance as UserControl);
        if (c != null) c.Tag = Header;
      }
      _execute((T)_instance);
    }
    #endregion ICommand
  }
}
