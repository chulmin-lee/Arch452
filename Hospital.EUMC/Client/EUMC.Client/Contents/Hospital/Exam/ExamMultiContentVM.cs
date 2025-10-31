using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class ExamMultiContentVM : ContentViewModelBase
  {
    public ExamMultiInformation ITEM { get; set; }
    public ExamMultiContentVM(ExamMultiViewConfig o) : base(o)
    {
      this.ITEM = new ExamMultiInformation(o);
    }
    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;
      switch (m.ServiceId)
      {
        case SERVICE_ID.EXAM_PT:
          {
            return this.ITEM.Update(m.CastTo<EXAM_RESP>());
          }
        case SERVICE_ID.SVR_CALL_PATIENT:
          {
            return this.ITEM.CallPatient(m.CastTo<CALL_PATIENT_NOTI>());
          }
      }
      return false;
    }
    protected override void ContentClose()
    {
      this.ITEM.Close();
    }
  }
}