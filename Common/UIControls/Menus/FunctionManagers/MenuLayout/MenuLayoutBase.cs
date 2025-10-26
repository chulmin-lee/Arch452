using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  public abstract class MenuLayoutBase<B,G,F>
    //where B:Enum where G : Enum where F : Enum
    where B : struct, IConvertible
    where G : struct, IConvertible where F : struct, IConvertible
  {
    bool CanHasSubmenu = true;
    bool CanHasMultiBand = true;
    public List<MenuLayoutItem<B,G,F>> Bands { get; private set; } = new List<MenuLayoutItem<B,G,F>>();

    public MenuLayoutBase(bool has_submenu = true, bool multi_band = true)
    {
      this.CanHasSubmenu = has_submenu;
      this.CanHasMultiBand = multi_band;
    }

    public void AddBand(MenuLayoutItem<B, G, F> header)
    {
      if (header.Type == MENU_MEMBER_TYPE.BAND)
      {
        Bands.Add(header);
      }
    }

    public MenuLayoutItem<B,G,F> CreateBandItem(B bandId, string header)
    {
      if(!CanHasMultiBand && this.Bands.Count > 0)
      {
        return null;
      }

      var band = (from d in Bands where d.BandId.AreEquals(bandId) select d).FirstOrDefault();
      if(band == null)
      {
        band = new MenuLayoutItem<B,G,F>(bandId, header, null);
      }
      return band;
    }
    public MenuLayoutItem<B,G,F> Submenu(string header, MenuIconPack icon = null)
    {
      return this.CanHasSubmenu ? new MenuLayoutItem<B, G, F>(header, icon): null;
    }
    public MenuLayoutItem<B,G,F> Seperator()
    {
      return new MenuLayoutItem<B,G,F>();
    }
    public MenuLayoutItem<B,G,F> MenuItem(FunctionCommand<G,F> command, MenuIconPack icon=null)
    {
      return new MenuLayoutItem<B,G,F>(command, icon);
    }
  }
}
