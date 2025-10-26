using System.Windows;

namespace UIControls
{
  public class CheckedTextBox : CustomTextBoxBase
  {
    static CheckedTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckedTextBox),
       new FrameworkPropertyMetadata(typeof(CheckedTextBox)));
    }

    public bool IsChecked
    {
      get { return (bool)GetValue(IsCheckedProperty); }
      set { SetValue(IsCheckedProperty, value); }
    }

    public static readonly DependencyProperty IsCheckedProperty =
    DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckedTextBox), new PropertyMetadata(false));
  }
}
