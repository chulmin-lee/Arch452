using ServiceCommon;

namespace EUMC.HospitalService
{
  internal class ServiceConfig
  {
    public SERVICE_ID ID { get; set; }
    public ServiceConfig() { }
    public ServiceConfig(SERVICE_ID s)
    {
      this.ID = s;
    }
  }
}