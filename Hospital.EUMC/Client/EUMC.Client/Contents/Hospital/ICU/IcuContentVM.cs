using Common;
using EUMC.ClientServices;
using ServiceCommon;

namespace EUMC.Client
{
  internal class IcuContentVM : ContentViewModelBase
  {
    public IcuInformation ITEM { get; set; }

    public IcuContentVM(IcuViewConfig config) : base(config)
    {
      this.ITEM = new IcuInformation(config);
      this.ITEM.TitleChanged += (s, e) => this.ContentTitle = e;
    }

    public override bool MessageReceived(ServiceMessage m)
    {
      if (base.MessageReceived(m)) return true;
      switch (m.ServiceId)
      {
        case SERVICE_ID.ICU:
          {
            return this.ITEM.Update(m.CastTo<ICU_RESP>());
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

  internal class IcuStaffContentVM : IcuContentVM
  {
    public IcuStaffContentVM(IcuViewConfig config) : base(config)
    {
    }
  }

  internal class IcuGuardContentVM : IcuContentVM
  {
    public IcuGuardContentVM(IcuViewConfig config) : base(config)
    {
    }
  }

  internal class IcuBabyContentVM : IcuContentVM
  {
    public IcuBabyContentVM(IcuViewConfig config) : base(config)
    {
    }
  }
}