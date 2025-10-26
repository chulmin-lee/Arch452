using UIControls;

namespace EUMC.Client
{
  public partial class ContentWindowViewModel : ViewModelBase
  {
    #region test 용 단축키
    /*
    public InputBindingCollection InputBindings { get; private set; }
    void key_binding()
    {
      InputBindings = new InputBindingCollection();
      InputBindings.Add(new KeyBinding(Test1Command, Test1Command.Gesture) { CommandParameter = "test1" });
      InputBindings.Add(new KeyBinding(Test2Command, Test2Command.Gesture) { CommandParameter = "test2" });
      InputBindings.Add(new KeyBinding(Test3Command, Test3Command.Gesture) { CommandParameter = "test3" });
    }

    RelayCommand _test1Command;
    public RelayCommand Test1Command => _test1Command ?? (_test1Command = CommandActivator.Exec(o => Test1CommandAction(o), "test1", "F1"));
    void Test1CommandAction(object o)
    {
      this.Contents.Test1();
    }
    RelayCommand _test2Command;
    public RelayCommand Test2Command => _test2Command ?? (_test2Command = CommandActivator.Exec(o => Test2CommandAction(o), "test2", "F2"));
    void Test2CommandAction(object o)
    {
      this.Contents.Test2();
    }
    RelayCommand _test3Command;
    public RelayCommand Test3Command => _test3Command ?? (_test3Command = CommandActivator.Exec(o => Test3CommandAction(o), "test3", "F3"));
    void Test3CommandAction(object o)
    {
      this.Contents.Test3();
    }
    */
    #endregion test 용 단축키
  }
}