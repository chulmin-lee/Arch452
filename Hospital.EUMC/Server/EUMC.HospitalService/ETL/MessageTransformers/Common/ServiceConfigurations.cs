using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class ServiceConfigurations
  {
    #region Property
    public DR_PHOTO_Service.Config DR_PHOTO { get; set; }
    #endregion Property

    public ServiceConfigurations()
    {
      this.DR_PHOTO = new DR_PHOTO_Service.Config()
      {
        ExceptDoctors = new List<string>(),
      };
    }
    public List<ServiceConfig> GetConfigs()
    {
      var list = new List<ServiceConfig>();

      foreach (var pi in this.GetType().GetProperties())
      {
        var o = (ServiceConfig)pi.GetValue(this);
        if (o != null)
        {
          list.Add(o);
        }
      }
      return list;
    }
    public static ServiceConfigurations Factory()
    {
      return new ServiceConfigurations();
    }
  }
}