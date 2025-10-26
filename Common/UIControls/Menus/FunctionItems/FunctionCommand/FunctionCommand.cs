using System;
using System.ComponentModel;
using System.Windows.Input;

namespace UIControls
{
  public class FunctionCommand<G, F> : ICommand, INotifyPropertyChanged, IDisposable
    //where G : Enum where F : Enum
  where G : struct, IConvertible where F : struct, IConvertible
  {
    protected readonly Predicate<object> _canExecute;
    protected readonly Action<F, FunctionItemBase<G,F>> _execute;

    static KeyGestureConverter _converter = new KeyGestureConverter();
    public string Shortcut { get; set; }
    public KeyGesture Gesture { get { return (Shortcut != null) ? (KeyGesture)_converter.ConvertFromString(Shortcut) : null; } }

    string _header;
    public string Header { get { return _header; } set { _header = value; OnPropertyChanged("Header"); } }
    public F FunctionId { get; private set; }
    MenuInformation<F> functionInfo;

    public FunctionCommand(MenuInformation<F> info, Action<F, FunctionItemBase<G, F>> execute)
      : this(info, execute, null)
    {
      this.functionInfo = info;
      this.functionInfo.PropertyChanged += FunctionInfo_PropertyChanged;
    }
    private void FunctionInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case nameof(MenuInformation<F>.Name):
          {
            this.Header = functionInfo.Name;
            break;
          }
        case nameof(MenuInformation<F>.Shortcuts):
          {
            this.Shortcut = functionInfo.Shortcuts;
            break;
          }
      }
    }

    public FunctionCommand(MenuInformation<F> info, Action<F, FunctionItemBase<G, F>> execute, Predicate<object> canExecute)
      : this(info.FunctionId, execute, canExecute, info.Name, info.Shortcuts)
    {
    }
    public FunctionCommand(F functionId, Action<F, FunctionItemBase<G, F>> execute, string header, string shortcut = null)
      : this(functionId, execute, null, header, shortcut)
    {
    }

    public FunctionCommand(F functionId, Action<F, FunctionItemBase<G, F>> execute, Predicate<object> canExecute, string header, string shortcut = null)
    {
      this.FunctionId = functionId;

      _execute = execute ?? throw new ArgumentNullException("execute");
      _canExecute = canExecute;
      this.Header = header;
      this.Shortcut = shortcut;
    }

    #region Interface
    //[DebuggerStepThrough]
    public virtual bool CanExecute(object parameter)
    {
      // parameter: CommandParameter로 설정된 값
      // 단축키로 실행시 FunctionID가 전달된다.
      return _canExecute == null ? true : _canExecute(parameter);
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
    public void Execute(object parameter)
    {
      // 단축키로 실행되는 경우 parameter는 FunctionId 이다.
      // FunctioItemBase로 변환할 수 없으므로 null이 전달된다.
      var o = parameter as FunctionItemBase<G,F>;
      _execute(FunctionId, o);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(String propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public void Dispose()
    {
      if (this.functionInfo != null)
      {
        this.functionInfo.PropertyChanged -= FunctionInfo_PropertyChanged;
      }
    }
    #endregion Interface
  }
}