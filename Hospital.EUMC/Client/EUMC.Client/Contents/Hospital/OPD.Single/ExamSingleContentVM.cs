using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System.Linq;

namespace EUMC.Client
{
  internal class ExamSingleContentVM : ContentViewModelBase
  {
    public SingleRoomInformation ITEM { get; set; }
    public ExamSingleContentVM(ExamSingleViewConfig config) : base(config)
    {
      this.ITEM = new SingleRoomInformation(config);
    }

    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;

      LOG.dc($"{m.ServiceId}");
      switch (m.ServiceId)
      {
        case SERVICE_ID.EXAM_PT:
          {
            var room = m.CastTo<EXAM_RESP>().Rooms.Where(x => x.GroupKey == this.ITEM.Key).FirstOrDefault();
            if (room != null)
            {
              this.ITEM.Update(room);
              return true;
            }
            return false;
          }
      }
      return false;
    }
  }
}