using System;
using System.Collections.Generic;

namespace UIControls
{
  public abstract class IconFactoryBase<F> where F : struct, IConvertible
  {
    public int IconWidth { get; private set; }
    public int IconHeight { get; private set; }
    public int IconCount { get; private set; }
    public string BaseLocation { get; private set; }

    Dictionary<F, MenuIconPack> Icons;

    public IconFactoryBase(string location, int width, int height, int count)
    {
      BaseLocation = location;
      IconWidth = width;
      IconHeight = height;
      IconCount = count;
      Icons = CreateIconPacks();
    }
    public MenuIconPack GetIconPack(F id)
    {
      Icons.TryGetValue(id, out MenuIconPack pack);
      return pack;
    }
    protected abstract List<MenuIcon<F>> GetIcons();
    protected Dictionary<F, MenuIconPack> CreateIconPacks()
    {
      Dictionary<F, MenuIconPack> o = new Dictionary<F, MenuIconPack>();
      var list = GetIcons();

      foreach (var item in list)
      {
        Uri uri = item.Uri;
        if (!string.IsNullOrEmpty(item.Name))
        {
          var pack = $"{BaseLocation}/{item.Name}";
          uri = new Uri(pack);
        }

        if (uri != null)
        {
          var pack = IconLoader.GetIconPack(uri, IconWidth, IconHeight, IconCount);

          if (pack != null)
          {
            o.Add(item.FunctionId, pack);
          }
          else
          {
            throw new Exception("pack is null");
          }
        }
        else
        {
          throw new Exception("uri is null");
        }
      }
      return o;
    }
  }
}
