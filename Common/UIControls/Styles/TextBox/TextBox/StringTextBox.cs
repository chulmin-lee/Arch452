using System.Windows;
using System.Windows.Input;

namespace UIControls
{
  public class StringTextBox : CustomTextBoxBase
  {
    static StringTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StringTextBox),
       new FrameworkPropertyMetadata(typeof(StringTextBox)));

      //AcceptsReturnProperty.OverrideMetadata(typeof(StringTextBox),
      //  new FrameworkPropertyMetadata(new PropertyChangedCallback(AcceptsReturnPropertyChanged)));
    }
    /// <summary>
    /// TextWrapping 모드에서는 Enter key 무시
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //protected static void AcceptsReturnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    //{
    //  var tb = (StringTextBox)sender;
    //  if (!tb.IsFocused)
    //  {
    //    tb.isAcceptsReturn = (bool)e.NewValue;
    //  }
    //}

    //bool isAcceptsReturn = false;

    //protected override void on_key_down(Key key)
    //{
    //  if (this.EnterKey && !isAcceptsReturn && key == Key.Enter)
    //  {
    //    this.update();
    //  }
    //  else if (this.EscKey && key == Key.Escape)
    //  {
    //    this.cancel();
    //  }
    //}
  }
}