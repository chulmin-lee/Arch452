using ServiceCommon;
using ServiceCommon.ClientServices;
using System;

namespace EUMC.Client
{
  internal class InfomationMainVM : ContentViewModelBase
  {
    IInformationMenu _contents;
    public IInformationMenu Contents { get { return _contents; } set { Set(ref _contents, value); } }

    public InfomationMainVM(IPackageViewConfig config) : base(config)
    {
      this.assign_content(new InformationHomeVM());
    }

    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;
      this.Contents?.MessageReceived(m);
      return true;
    }

    void change_content(INFO_TYPE type)
    {
      INFO_TYPE current = Contents?.ID ?? INFO_TYPE.NONE;

      if (current != type)
      {
        switch (type)
        {
          case INFO_TYPE.HOME:
            this.assign_content(new InformationHomeVM());
            break;
          case INFO_TYPE.RECEPT:
          case INFO_TYPE.AMENITIES:
          case INFO_TYPE.FLOOR_MAP:
          case INFO_TYPE.FIND_CAR:
          case INFO_TYPE.TRAFFIC_INFO:
            {
              if (current == INFO_TYPE.HOME)
              {
                this.assign_content(new InformationContentVM(type));
              }
              else
              {
                this.Contents?.ChangeContent(type);
              }
            }
            break;
          default: throw new Exception($"{type} not supported");
        }
      }
    }
    void assign_content(IInformationMenu o)
    {
      this.Contents = o;
      this.Contents.MenuSelected += (s, e) => this.change_content(e);
    }
    protected override void ContentClose()
    {
    }
  }
}