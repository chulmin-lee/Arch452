using System.Collections.Generic;

namespace EUMC.HospitalService
{
  internal class DataConfigurations
  {
    #region Property
    public bool IsBackup { get; set; } = true;
    public int ScheduleInterval { get; set; } = 10;
    public DR_PHOTO_Extractor.Config DR_PHOTO { get; set; }
    #endregion Property

    public DataConfigurations()
    {
      this.DR_PHOTO = new DR_PHOTO_Extractor.Config()
      {
        PhotoDirName = "Photo",
        UseHttp = false,
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

    public static DataConfigurations Factory()
    {
      return new DataConfigurations();
    }
  }
}