using Framework.Network.HTTP;
using System.IO;
using System.Threading.Tasks;

namespace ServiceCommon.ClientServices
{
  public interface IPlaylistUpdater
  {
    Task<PackageConfigurations> DownloadAsync();
    Task<string> GetPlaylistXml();
  }

  public abstract class PlaylistUpdaterBase : IPlaylistUpdater
  {
    protected ILocation Location;
    public PlaylistUpdaterBase(ILocation location)
    {
      this.Location = location;
    }

    public async Task<PackageConfigurations> DownloadAsync()
    {
      var xml = await this.GetPlaylistXml();
      return this.CreatePackageConfigurationsImpl(xml);
    }
    protected abstract PackageConfigurations CreatePackageConfigurationsImpl(string path);
    /// <summary>
    /// 원격 playlist.xml을 읽는다.
    /// - local 파일과 다른 경우 저장한다
    /// 실패하는 경우 local playlist.xml 을 돌려준다
    /// </summary>
    public async Task<string> GetPlaylistXml()
    {
      var local_path = this.Location.PlaylistXmlPath;
      var remote = await HttpDownloader.GetStringAsync(this.Location.PlaylistXmlURL);

      if (string.IsNullOrEmpty(remote))
      {
        if (File.Exists(local_path))
        {
          return File.ReadAllText(local_path);
        }
        else
        {
          return string.Empty;
        }
      }
      else
      {
        if (File.Exists(local_path))
        {
          if (remote != File.ReadAllText(local_path))
          {
            File.WriteAllText(local_path, remote);
          }
        }
        else
        {
          File.WriteAllText(local_path, remote);
        }
        return remote;
      }
    }
  }
}