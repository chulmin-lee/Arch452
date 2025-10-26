namespace ServiceCommon.HospitalService
{
  public class DR_SCH_Loader : ALL_LoaderBase<DR_SCH_INFO>
  {
    public DR_SCH_Loader() : base(SERVICE_ID.DR_SCH) { }

    protected override ServiceMessage create_message() => new DR_SCH_RESP(this.Items);
  }
}