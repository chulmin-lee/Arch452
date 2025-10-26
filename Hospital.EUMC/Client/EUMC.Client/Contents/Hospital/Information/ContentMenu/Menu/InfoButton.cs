using Common;
using UIControls;
using ServiceCommon;
using ServiceCommon.ClientServices;
using EUMC.ClientServices;
using System.Windows.Input;
using UIControls;
using System.Collections.Generic;
using System;

namespace EUMC.Client
{
  public class InfoButton : ViewModelBase
  {
    public event EventHandler<INFO_TYPE> ButtonSelected;
    public INFO_TYPE ID { get; set; }

    bool _isEnabled = true;
    public bool IsEnabled { get { return _isEnabled; } set { Set(ref _isEnabled, value); } }
    bool _isSelected;
    public bool MenuSelected { get { return _isSelected; } set { Set(ref _isSelected, value); } }

    public InfoButton(INFO_TYPE id)
    {
      this.ID = id;

      var attr = id.GetAttribute<InformationAttribute>();
      if (attr == null) throw new ArgumentException($"{id} not supported");

      this.Title = attr.Title;
      this.Description = attr.Description;
    }

    //public InfoButton(INFO_TYPE id, ImageSource icon, bool selected = false) : this(id)
    //{
    //  this.Icon = icon;
    //  this.IsSelected = selected;
    //}

    //public ImageSource? Icon { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    #region selected
    RelayCommand _buttonCommand;
    public RelayCommand ButtonCommand => _buttonCommand ?? (_buttonCommand = CommandActivator.Exec(o => CommandAction(o)));
    void CommandAction(object o)
    {
      this.ButtonSelected?.Invoke(this, this.ID);
    }
    #endregion selected
  }
}