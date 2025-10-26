using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class ExamSingleContentVM : ContentViewModelBase
  {
    public ExamSingleInformation ITEM { get; set; }
    public ExamSingleContentVM(ExamSingleViewConfig config) : base(config)
    {
      this.ITEM = new ExamSingleInformation(this, config);
    }
    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;
      switch (m.ServiceId)
      {
        case SERVICE_ID.EXAM_ROOM:
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