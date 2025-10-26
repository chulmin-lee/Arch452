using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UIControls
{
  public class ToggleCommand : RelayCommand
  {
    Timer _timer = null;
    bool _isExpired = true;
    string _onMessage;
    string _offMessage;

    bool _isChecked;
    public bool IsChecked
    {
      get { return _isChecked; }
      set
      {
        _isChecked = value;
        OnPropertyChanged("IsChecked");
        this.Header = (value) ? _offMessage : _onMessage;
      }
    }

    public ToggleCommand(Action<object> execute, string onMessage, string offMessage, string shortcut = null)
      : this(execute, null, onMessage, offMessage, shortcut) { }
    public ToggleCommand(Action<object> execute, Predicate<object> canExecute, string onMessage, string offMessage, string shortcut = null)
      : base(execute, canExecute, onMessage, shortcut)
    {
      _onMessage = onMessage;
      _offMessage = offMessage;
      _timer = new Timer(timerTick, null, Timeout.Infinite, Timeout.Infinite);
    }

    void timerTick(Object state)
    {
      _isExpired = true;
      _timer.Change(Timeout.Infinite, Timeout.Infinite); // timer off

      // CanExecute 다시 부르기
      if(UIContextHelper.UIDispatcher != null)
      {
        UIContextHelper.CheckBeginInvokeOnUI(() => { CommandManager.InvalidateRequerySuggested(); });
      }
    }

    #region ICommand
    [DebuggerStepThrough]
    public override bool CanExecute(object parameter)
    {
      if (_isExpired)
      {
        return _canExecute == null ? true : _canExecute(parameter);
      }
      else
      {
        return false;  // timer 종료전까지는 무조건 false
      }
    }
    public override void Execute(object parameter)
    {
      if (_isExpired)
      {
        _isExpired = false;
        _timer.Change(1000, 1000);
        _execute(parameter);
      }
    }
    #endregion ICommand
  }
}
