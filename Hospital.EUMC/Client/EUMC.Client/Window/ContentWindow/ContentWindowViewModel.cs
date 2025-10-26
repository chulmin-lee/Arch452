using Common;
using ServiceCommon;
using ServiceCommon.ClientServices;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using UIControls;

namespace EUMC.Client
{
  public partial class ContentWindowViewModel : ViewModelBase, IWindowViewModel
  {
    public event EventHandler ContentChanged;
    public bool IsCustomWindow => false;
    IContentViewModel _contents;
    public IContentViewModel Contents
    {
      get => _contents;
      set
      {
        if (Set(ref _contents, value))
        {
          if (value != null)
          {
            this.ContentChanged?.Invoke(this, EventArgs.Empty);
          }
        }
      }
    }
    public void ConfigChanged(IPackageViewConfig config)
    {
      LOG.ic($"package : {config.Package}");
      this.Contents?.Close();

      var con = ContentViewFactory.Create(config);
      if (con != null)
      {
        this.Contents = con;
      }
      else
      {
        LOG.e($"not support : package= {config.Package}");
      }
    }
    public List<KeyBinding> KeyBindings()
    {
      return this.Contents?.KeyBindings() ?? new List<KeyBinding>();
    }

    public void ReceiveMessage(ServiceMessage m)
    {
      if (!this.Contents?.MessageReceived(m) ?? false)
      {
        LOG.wc($"{m.ServiceId} not processed");
      }
    }

    public void Close()
    {
      this.Contents?.Close();
    }
  }
}