using ServiceCommon;
using System.Collections.Generic;

namespace EUMC.Client
{
  internal class InformationContentVM : InfoMenuBaseVM
  {
    public override INFO_TYPE ID => this.Contents?.ID ?? INFO_TYPE.NONE;

    IInformationContent _contents;
    public IInformationContent Contents { get { return _contents; } set { Set(ref _contents, value); } }
    public InformationContentVM(INFO_TYPE s)
    {
      var list = new List<INFO_TYPE>
      {
        INFO_TYPE.RECEPT,
        INFO_TYPE.FLOOR_MAP,
        INFO_TYPE.AMENITIES,
        INFO_TYPE.FIND_CAR,
        INFO_TYPE.TRAFFIC_INFO,
        INFO_TYPE.HOME,
      };
      this.Initialize(list, true);
      this.RowCount = 1;
      this.ColumnCount = this.Buttons.Count;
      this.ChangeContent(s);
    }

    public override void ChangeContent(INFO_TYPE type)
    {
      if (type == INFO_TYPE.HOME)
      {
        this.ChangeMenu(type);
      }
      else
      {
        if (this.Contents != null && this.Contents.ID == type)
        {
          return;
        }
        this.Contents = this.Create(type);
        this.ButtonBar?.Selected(type);
      }
    }

    public override void MessageReceived(ServiceMessage m)
    {
      this.Contents?.MessageReceived(m);
    }

    IInformationContent Create(INFO_TYPE type)
    {
      switch (type)
      {
        case INFO_TYPE.RECEPT: return new ReceptionVM();
        case INFO_TYPE.FLOOR_MAP: return new FloorMapVM();
        case INFO_TYPE.AMENITIES: return new AmenitiesVM();
        case INFO_TYPE.FIND_CAR: return new FindCarVM();
        case INFO_TYPE.TRAFFIC_INFO: return new TrafficVM();

        default: return null;
      }
    }
  }
}