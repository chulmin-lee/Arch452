using ServiceCommon;
using ServiceCommon.ClientServices;
using EUMC.ClientServices;
using System.Windows.Input;
using UIControls;
using System.Collections.Generic;
using System;

namespace EUMC.Client
{
  internal class InfoButtonBar
  {
    public event EventHandler<INFO_TYPE> ButtonSelected;
    public List<InfoButton> Buttons { get; set; } = new List<InfoButton>();
    bool _use_disable;
    public InfoButtonBar(List<INFO_TYPE> list, bool use_disable = false)
    {
      _use_disable = use_disable;
      foreach (var p in list)
      {
        var button = new InfoButton(p);
        button.ButtonSelected += MenuSelected;  // += (s, e) => this.ButtonSelected?.Invoke(this, e);
        this.Buttons.Add(button);
      }
    }
    private void MenuSelected(object sender, INFO_TYPE e)
    {
      //this.Selected(e);
      this.ButtonSelected?.Invoke(this, e);
    }

    internal void Selected(INFO_TYPE type)
    {
      foreach (var p in this.Buttons)
      {
        p.MenuSelected = (p.ID == type) ? true : false;

        if (_use_disable)
        {
          p.IsEnabled = !p.MenuSelected;
        }
      }
    }
  }
}