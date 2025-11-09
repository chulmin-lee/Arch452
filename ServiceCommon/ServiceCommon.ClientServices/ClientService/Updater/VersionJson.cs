using Common;

namespace ServiceCommon.ClientServices
{
  /// <summary>
  /// client update 정보를 가지고 있는 version.json 포멧
  /// </summary>
  public class VersionJson
  {
    public string ClientVersion { get; set; } = string.Empty;
    public string ClientFileName { get; set; } = string.Empty;

    public string UpdaterVersion { get; set; } = string.Empty;
    public string UpdaterFileName { get; set; } = string.Empty;

    public bool IsClientUpdated { get; private set; }
    public bool IsUpdaterUpdated { get; private set; }
    public bool IsUpdated => IsClientUpdated || IsUpdaterUpdated;

    public bool CheckVersion(VersionJson o)
    {
      this.IsClientUpdated = this.ClientVersion != o.ClientVersion && !string.IsNullOrEmpty(o.ClientFileName);
      this.IsUpdaterUpdated = this.UpdaterVersion != o.UpdaterVersion && !string.IsNullOrEmpty(o.UpdaterFileName);
      return IsClientUpdated || IsUpdaterUpdated;
    }

    public bool IsValid()
    {
      return !string.IsNullOrEmpty(this.ClientVersion) &&
             !string.IsNullOrEmpty(this.ClientFileName) &&
             !string.IsNullOrEmpty(this.UpdaterVersion) &&
             !string.IsNullOrEmpty(this.UpdaterFileName);
    }

    public static VersionJson Load(string path)
    {
      return NewtonJson.Load<VersionJson>(path) ?? new VersionJson();
    }
    public static VersionJson Deserialize(string json)
    {
      return NewtonJson.Deserialize<VersionJson>(json) ?? new VersionJson();
    }
  }
}