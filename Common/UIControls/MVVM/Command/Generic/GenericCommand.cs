using System;
using System.ComponentModel;
using System.Windows.Input;

namespace UIControls
{
  public class GenericCommand<T> : ICommand, INotifyPropertyChanged
  {
    protected readonly Action<T> _execute;
    protected readonly Predicate<T> _canExecute;

    static KeyGestureConverter _converter = new KeyGestureConverter();
    public string Shortcut { get; set; }
    public KeyGesture Gesture { get { return (Shortcut != null) ? (KeyGesture)_converter.ConvertFromString(Shortcut) : null; } }

    string _header;
    public string Header { get { return _header; } set { _header = value; OnPropertyChanged("Header"); } }
    public int ID { get; set; } = -1;
    public GenericCommand(Action<T> execute, string header = null, string shortcut = null) : this(execute, null, header, shortcut) { }
    public GenericCommand(Action<T> execute, Predicate<T> canExecute, string header = null, string shortcut = null)
    {
      _execute = execute ?? throw new ArgumentNullException("execute");
      _canExecute = canExecute;
      this.Header = header;
      this.Shortcut = shortcut;
    }

    #region ICommand
    //[DebuggerStepThrough]
    public virtual bool CanExecute(object parameter)
    {
      return _canExecute == null ? true : _canExecute((T)parameter);
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
        if (_canExecute != null)
        {
          CommandManager.RequerySuggested += value;
        }
      }

      remove
      {
        if (_canExecute != null)
        {
          CommandManager.RequerySuggested -= value;
        }
      }
    }

    // CommandParameter로 설정한것이 전달
    public virtual void Execute(object parameter)
    {
      _execute((T)parameter);
    }
    #endregion ICommand

    #region PropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(String propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}
