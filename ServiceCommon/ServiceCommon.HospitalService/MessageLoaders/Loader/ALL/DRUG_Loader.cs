namespace ServiceCommon.HospitalService
{
  public class DRUG_Loader : ALL_LoaderBase<DRUG_INFO>
  {
    public DRUG_Loader() : base(SERVICE_ID.DRUG) { }
    protected override ServiceMessage create_message() => new DRUG_RESP(this.Items);
  }
}