using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UIControls
{
  // old 버전. 사용하지말것
  public static class CommandActivator
  {
    public class Condition
    {
      private Predicate<object> _canExecute;
      public Condition(Predicate<object> canExecute)
      {
        _canExecute = canExecute;
      }
      public RelayCommand Exec(Action<object> execute, string header = null, string shortcut = null)
      {
        return new RelayCommand(execute, _canExecute, header, shortcut);
      }
      public RelayCommand Toggle(Action<object> execute, string on, string off, string shortcut = null)
      {
        return new ToggleCommand(execute, _canExecute, on, off, shortcut);
      }
    }
    #region SINGLE_WHEN
    public static Condition When(Predicate<object> canExecute)
    {
      return new Condition(canExecute);
    }
    #endregion

    #region SINGLE_EXEC
    public static RelayCommand Exec(Action<object> execute, string header = null, string shortcut = null)
    {
      return new RelayCommand(execute, header, shortcut);
    }
    public static RelayCommand Toggle(Action<object> execute, string on, string off, string shortcut = null)
    {
      return new ToggleCommand(execute, on, off);
    }
    #endregion
  }
}
