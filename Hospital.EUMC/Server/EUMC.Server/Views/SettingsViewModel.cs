using Common;
using UIControls;

namespace EUMC.Server
{
  public class SettingsViewModel : ViewModelBase
  {
    LOG_LEVEL _logLevel;
    public LOG_LEVEL LogLevel
    {
      get { return _logLevel; }
      set { if (Set(ref _logLevel, value)) { LOG.ChangeLogLevel(value); } }
    }

    public SettingsViewModel()
    {
      this._logLevel = LOG.LogLevel;
    }
  }
}