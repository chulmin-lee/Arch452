using UIControls;
using ServiceCommon;
using ServiceCommon;
using ServiceCommon.ClientServices;
using EUMC.ClientServices;
using System.Windows.Input;
using UIControls;
using System.Collections.Generic;
using System;

namespace EUMC.Client
{
  internal interface IInformationMenu : IInformationContent
  {
    event EventHandler<INFO_TYPE> MenuSelected;
    void ChangeContent(INFO_TYPE type);
  }

  internal abstract class InfoMenuBaseVM : ViewModelBase, IInformationMenu
  {
    public virtual INFO_TYPE ID { get; protected set; }
    public event EventHandler<INFO_TYPE> MenuSelected;
    public int RowCount { get; protected set; }
    public int ColumnCount { get; protected set; }
    public List<InfoButton> Buttons => this.ButtonBar?.Buttons ?? throw new NotImplementedException();
    protected InfoButtonBar ButtonBar;

    public abstract void ChangeContent(INFO_TYPE type);

    public virtual void MessageReceived(ServiceMessage m)
    {
    }
    protected void Initialize(List<INFO_TYPE> items, bool use_disable = false)
    {
      ButtonBar = new InfoButtonBar(items, use_disable);
      ButtonBar.ButtonSelected += (s, e) => this.ChangeContent(e);
    }
    protected void ChangeMenu(INFO_TYPE type)
    {
      this.MenuSelected?.Invoke(this, type);
    }
    protected override void Dispose(bool disposing)
    {
      MenuSelected = null;
    }
  }
}