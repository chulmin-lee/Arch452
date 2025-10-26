using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class DrugContentVM : ContentViewModelBase
  {
    public DrugInformation ITEM { get; set; }

    public DrugContentVM(DrugViewConfig config) : base(config)
    {
      this.ITEM = new DrugInformation(config);
    }

    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;
      switch (m.ServiceId)
      {
        case SERVICE_ID.DRUG:
          {
            return this.ITEM.Update(m.CastTo<DRUG_RESP>());
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
      LOG.ic($"{this.Config.Package}");
      this.ITEM.Close();
    }
  }
}