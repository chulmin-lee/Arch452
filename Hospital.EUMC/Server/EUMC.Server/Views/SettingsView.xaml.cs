using UIControls;

namespace EUMC.Server
{
  /// <summary>
  /// Interaction logic for SettingLogView.xaml
  /// </summary>
  public partial class SettingsView : CustomPopupWindow
  {
    public SettingsView()
    {
      InitializeComponent();
      this.DataContext = new SettingsViewModel();
    }

    private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
