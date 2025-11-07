using Common;
using EUMC.ClientServices;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EUMC.Client
{
  internal class ExamSingleContentVM : ContentViewModelBase
  {
    public ExamSingleInformation ITEM { get; set; }
    public ExamSingleContentVM(ExamSingleViewConfig config) : base(config)
    {
      this.ITEM = new ExamSingleInformation(config);
    }

    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;

      LOG.dc($"{m.ServiceId}");
      switch (m.ServiceId)
      {
        case SERVICE_ID.EXAM_PT:
          return this.ITEM.Update(m.CastTo<EXAM_RESP>());
      }
      return false;
    }
    protected override void ContentClose()
    {
      this.ITEM.Close();
    }
  }
}
