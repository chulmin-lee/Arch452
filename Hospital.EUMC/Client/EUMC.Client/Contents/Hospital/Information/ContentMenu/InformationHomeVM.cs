using System.Collections.Generic;

namespace EUMC.Client
{
  internal class InformationHomeVM : InfoMenuBaseVM
  {
    public InformationHomeVM()
    {
      var list = new List<INFO_TYPE>
      {
        INFO_TYPE.RECEPT,
        INFO_TYPE.FLOOR_MAP,
        INFO_TYPE.AMENITIES,
        INFO_TYPE.FIND_CAR,
        INFO_TYPE.TRAFFIC_INFO
      };
      this.ID = INFO_TYPE.HOME;
      this.Initialize(list);
      this.RowCount = this.Buttons.Count;
      this.ColumnCount = 1;
    }

    public override void ChangeContent(INFO_TYPE type)
    {
      this.ChangeMenu(type);
    }
  }
}