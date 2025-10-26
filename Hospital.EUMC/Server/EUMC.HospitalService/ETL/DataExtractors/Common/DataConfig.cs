namespace EUMC.HospitalService
{
  internal class DataConfig
  {
    public DATA_ID ID { get; set; }
    public DataConfig() { }
    public DataConfig(DATA_ID id)
    {
      this.ID = id;
    }
  }
}