namespace ServiceCommon.HospitalService
{
  public class OPERATION_Loader : ALL_LoaderBase<OPERATION_INFO>
  {
    public OPERATION_Loader() : base(SERVICE_ID.OPERATION) { }

    protected override ServiceMessage create_message() => new OPERATION_RESP(this.Items);
  }
}