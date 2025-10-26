using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  public enum MENU_MEMBER_TYPE
  {
    BAND, SUBMENU, MENU, SEPERATOR
  }

  public class MenuLayoutItem<B, G, F>
    //where B:Enum where G:Enum where F:Enum
    where B : struct, IConvertible
    where G : struct, IConvertible
    where F : struct, IConvertible
  {
    public MENU_MEMBER_TYPE Type { get; private set; }
    public B BandId { get; private set; }
    public F FunctionId { get; set; } = default(F);
    public FunctionCommand<G,F> Command;
    public string Header { get; private set; }
    public bool IsChecked { get; private set; }
    public MenuIconPack Icon { get; private set; }
    public List<MenuLayoutItem<B, G, F>> Children { get; private set; }

    // top header
    public MenuLayoutItem(B bandId, string header, MenuIconPack icon)
    {
      this.Type = MENU_MEMBER_TYPE.BAND;
      this.BandId = bandId;
      this.Header = header;
      this.Icon = icon;
      this.Children = new List<MenuLayoutItem<B, G, F>>();
    }
    // submenu
    public MenuLayoutItem(string header, MenuIconPack icon)
    {
      this.Type  = MENU_MEMBER_TYPE.SUBMENU;
      this.Header = header;
      this.Icon = icon;
      this.Children = new List<MenuLayoutItem<B, G, F>>();
    }
    // seperator
    public MenuLayoutItem()
    {
      this.Type = MENU_MEMBER_TYPE.SEPERATOR;
    }

    // action
    public MenuLayoutItem(FunctionCommand<G,F> command, MenuIconPack icon, bool ischecked = false)
    {
      this.Type = MENU_MEMBER_TYPE.MENU;
      this.FunctionId = command.FunctionId;
      this.Header = command.Header;
      this.Command = command;
      this.Icon = icon;
      this.IsChecked = ischecked;
    }

    public void Add(MenuLayoutItem<B, G,  F> item)
    {
      this.Children.Add(item);
    }
    public void Add(List<MenuLayoutItem<B, G, F>> list)
    {
      foreach(var item in list)
        this.Children.Add(item);
    }
  }
}
