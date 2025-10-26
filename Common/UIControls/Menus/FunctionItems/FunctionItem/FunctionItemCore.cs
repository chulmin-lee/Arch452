using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace UIControls
{
  public enum FUNCTION_ITEM_ROLE
  {
    TOP_HEADER, SUB_MENU, FUNCTION, SEPERATOR
  }

  public class FunctionItemCore : ViewModelBase
  {
    public FUNCTION_ITEM_ROLE Role { get; protected set; }
    public bool IsGroupMember { get; protected set; }

    public FunctionItemCore() { }

    public FunctionItemCore(string header, ICommand command)
    {
      this.Role = FUNCTION_ITEM_ROLE.FUNCTION;
      this.Header = header;
      this.Command = command;
    }
    public FunctionItemCore(RelayCommand command)
    {
      this.Role = FUNCTION_ITEM_ROLE.FUNCTION;
      this.Header = command.Header;
      this.Command = command;
    }

    public void CheckedUIUpdate(bool select)
    {
      _isChecked = select;
      OnPropertyChanged(nameof(this.IsChecked));
    }

    public ICommand Command { get; protected set; }

    #region Binding Property
    public bool IsCheckable { get; set; }
    protected bool _isChecked;
    public virtual bool IsChecked { get { return _isChecked; } set { Set(ref _isChecked, value); } }
    protected bool _isMenuPressed;
    public virtual bool IsMenuPressed { get { return _isMenuPressed; } set { Set(ref _isMenuPressed, value); } }

    public bool IsSeparator { get; set; }

    protected string _header;
    public virtual string Header { get { return _header; } set { Set(ref _header, value); } }

    protected string _shortcuts;
    public virtual string Shortcuts { get { return _shortcuts; } set { Set(ref _shortcuts, value); } }

    string _toolTip;
    public string ToolTip { get { return _toolTip; } set { Set(ref _toolTip, value); } }

    bool _isEnabled = true;
    public bool IsEnabled { get { return _isEnabled; } set { Set(ref _isEnabled, value); } }
    bool _isVisible = true;
    public bool IsVisible { get { return _isVisible; } set { Set(ref _isVisible, value); } }
    #endregion

    #region ICON
    public MenuIconPack Icons { get; protected set; }
    public string IconPath { get; protected set; }
    public ImageSource IconImage { get; protected set; }
    #endregion

    #region Children
    public ObservableCollection<FunctionItemCore> Children { get; set; } = new ObservableCollection<FunctionItemCore>();

    public void SetVisible(bool on)
    {
      IsVisible = on;
      foreach (var item in Children)
      {
        item.IsVisible = on;
      }
    }
    public void SetEnable(bool on)
    {
      IsEnabled = on;
      foreach (var item in Children)
      {
        item.IsEnabled = on;
      }
    }
    public FunctionItemCore Find(string name)
    {
      if (Header == name)
      {
        return this;
      }

      foreach (var item in Children)
      {
        var o = item.Find(name);
        if (o != null)
        {
          return o;
        }
      }
      return null;
    }
    public void Clear()
    {
      Children.Clear();
    }
    #endregion

    #region Test
    public void ExecuteCommand()
    {
      this.Command.Execute(this);
    }

    #endregion
  }
}
