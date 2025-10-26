namespace ServiceCommon.HospitalService
{
  public class INSPECTION_Loader : ALL_LoaderBase<INSPECTION_INFO>
  {
    public INSPECTION_Loader() : base(SERVICE_ID.INSPECTION) { }
    protected override ServiceMessage create_message() => new INSPECTION_RESP(this.Items);
  }
}