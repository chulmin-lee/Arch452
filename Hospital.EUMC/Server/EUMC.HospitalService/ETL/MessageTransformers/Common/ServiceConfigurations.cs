using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class ServiceConfigurations
  {
    #region Property
    public OFFICE_PT_Service.Config OFFICE_PT { get; set; }
    #endregion Property

    public ServiceConfigurations(bool seoul)
    {
      this.OFFICE_PT = new OFFICE_PT_Service.Config();
      {
        var o = new List<OFFICE_PT_Service.OfficeNameConfig>();
        if (seoul)
        {
          // ccv
          o.Add(new OFFICE_PT_Service.OfficeNameConfig
          {
            DeptCode = "CCV",
            Conditions = new List<string> { "1","2","5","6","7","8","9" },
            NewDeptName = "뇌혈관병원"
          });
          o.Add(new OFFICE_PT_Service.OfficeNameConfig
          {
            DeptCode = "CCV",
            Conditions = new List<string> { "3", "4", "14", "15", "16", "17", "18" },
            NewDeptName = "대동맥혈관병원"
          });
          o.Add(new OFFICE_PT_Service.OfficeNameConfig
          {
            DeptCode = "CCV",
            Conditions = new List<string> { "10", " 11", " 12", " 13" },
            NewDeptName = "심혈관센터"
          });
        }
        else
        {
          o.Add(new OFFICE_PT_Service.OfficeNameConfig
          {
            DeptCode = "WGO",
            Conditions = new List<string> { "4", "5" },
            NewDeptName = "산부인과",
            NewRoomNames = new Dictionary<string, string>
            {
              { "4", "진료실1" },
              { "5", "진료실2" },
            }
          });
        }

        this.OFFICE_PT.NameConfigs.AddRange(o);
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
    public static ServiceConfigurations Factory(bool seoul)
    {
      return new ServiceConfigurations(seoul);
    }
  }
}