namespace ServiceCommon.HospitalService
{
  public class ENDO_Loader : ALL_LoaderBase<ENDO_INFO>
  {
    public ENDO_Loader() : base(SERVICE_ID.ENDO) { }
    protected override ServiceMessage create_message() => new ENDO_RESP(this.Items);
  }
}