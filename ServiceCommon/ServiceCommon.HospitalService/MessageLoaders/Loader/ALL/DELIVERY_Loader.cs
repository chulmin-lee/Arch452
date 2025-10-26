namespace ServiceCommon.HospitalService
{
  public class DELIVERY_Loader : ALL_LoaderBase<DELIVERY_INFO>
  {
    public DELIVERY_Loader() : base(SERVICE_ID.DELIVERY_ROOM) { }
    protected override ServiceMessage create_message() => new DELIVERY_ROOM_RESP(this.Items);
  }
}