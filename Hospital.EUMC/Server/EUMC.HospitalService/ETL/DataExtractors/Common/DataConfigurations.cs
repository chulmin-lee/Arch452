using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class DataConfigurations
  {
    #region Property
    public bool IsBackup { get; set; } = true;
    public int ScheduleInterval { get; set; } = 10;
    public ICU_Extractor.Config ICU { get; set; }
    public DR_PHOTO_Extractor.Config DR_PHOTO { get; set; }


    #endregion Property

    public DataConfigurations(bool seoul)
    {
      #region 중환자실
      this.ICU = new ICU_Extractor.Config
      {
        IcuDeptNames = new List<string> { "중환자", "뇌졸중집중치료실", "55병동" },
        IcuDeptCodes = seoul ? new List<string>()
                             : new List<string> { "MICU1", "MICU2", "MICU3", }
      };
      #endregion

      this.DR_PHOTO = new DR_PHOTO_Extractor.Config()
      {
        PhotoDir= @"c:\APM_Setup\didmate\dr_photos",
        PhotoUrl=@"http://192.168.0.30/dr_photos",
        Interval = 60
      };
    }

    public List<DataConfig> GetConfigs()
    {
      var list = new List<DataConfig>();
      foreach (var pi in this.GetType().GetProperties())
      {
        var o = (DataConfig)pi?.GetValue(this);
        if (o != null)
        {
          list.Add(o);
        }
      }
      return list;
    }

    public static DataConfigurations Factory(bool seoul)
    {
      return new DataConfigurations(seoul);
    }
  }
}