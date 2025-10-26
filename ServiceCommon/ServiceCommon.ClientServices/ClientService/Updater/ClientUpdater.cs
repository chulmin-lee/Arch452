using Common;
using Framework.Network.HTTP;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceCommon.ClientServices
{
  public interface IClientUpdater
  {
    event EventHandler<string> BackgoundClientUpdated;
    bool IsBackgroundRunning { get; }
    Task<VersionJson> ClientVersionCheck(bool forced = false);
    Task<string> ClientUpdate(bool forced);
  }

  public class ClientUpdater : IClientUpdater
  {
    public event EventHandler<string> BackgoundClientUpdated;
    InterlockWrapper Interlock = new InterlockWrapper();
    public bool IsBackgroundRunning => Interlock.IsRunning;
    CancellationTokenSource _cts;

    ILocation Location;
    bool _extract_zip_file;
    public ClientUpdater(ILocation location, bool extract_zip_file)
    {
      Location = location;
      _extract_zip_file = extract_zip_file;
    }
    /// <summary>
    /// 원격 version.json을 읽어온다
    /// 실패하는 경우 Null을 리턴한다.
    /// </summary>
    /// <param name="forced">local version과 비교하지 않고 그대로 전달</param>
    /// <returns></returns>
    public async Task<VersionJson> ClientVersionCheck(bool forced = false)
    {
      var remote_json = await HttpDownloader.GetStringAsync(this.Location.VersionJsonURL);
      if (string.IsNullOrEmpty(remote_json))
      {
        return null;
      }

      var remote_version = VersionJson.Deserialize(remote_json);
      if (remote_version == null) return null;

      if (forced)
      {
        return remote_version;
      }
      var local_version = VersionJson.Load(this.Location.VersionJsonLocal);
      remote_version.CheckVersion(local_version);
      return remote_version;
    }
    public async Task<string> ClientUpdate(bool forced)
    {
      var version = await this.ClientVersionCheck(forced);
      if (version == null) return string.Empty;

      // updater
      if (forced || version.IsUpdaterUpdated)
      {
        var filename = version.UpdaterFileName;
        var local = this.Location.DownloadFilePath(filename);
        var remote = this.Location.UpdateFileUrl(filename);

        var updater = new DownloadFile(remote, local, true);

        if (await HttpDownloader.FileDownloadAsync(updater))
        {
          var ext_dir = this.Location.ExtractDir;
          if (this.extract(local, ext_dir))
          {
            FileHelper.CopyDirectory(ext_dir, this.Location.UpdaterPath);
            FileHelper.ClearDirectory(ext_dir); // 임시 폴더 정리
          }
        }
      }

      string patch_location = string.Empty;
      // client patch
      if (forced || version.IsClientUpdated)
      {
        var filename = version.ClientFileName;
        var local = this.Location.DownloadFilePath(filename);
        var remote = this.Location.UpdateFileUrl(filename);

        var down = new DownloadFile(remote, local, true);
        var dest = await this.client_patch_download(down);

        if (!string.IsNullOrEmpty(dest))
        {
          patch_location = dest;
        }
        else
        {
          _cts = new CancellationTokenSource();
          // background update
          this.background_update(down, _cts.Token);
          return string.Empty;
        }
      }

      if (forced || version.IsUpdated)
      {
        var v = new VersionJson
        {
          ClientFileName = version.ClientFileName,
          ClientVersion = version.ClientVersion,
          UpdaterFileName = version.UpdaterFileName,
          UpdaterVersion = version.UpdaterVersion,
        };
        NewtonJson.Serialize<VersionJson>(v, this.Location.VersionJsonLocal);
      }

      return patch_location;
    }

    /// <summary>
    /// 지정된 파일을 다운로드 한다.
    /// 성공하면 파일 경로 또는 저장 위치를 리턴한다
    /// </summary>
    /// <param name="down"></param>
    /// <returns></returns>
    async Task<string> client_patch_download(DownloadFile down)
    {
      if (await HttpDownloader.FileDownloadAsync(down))
      {
        if (_extract_zip_file)
        {
          var ext_dir = this.Location.ExtractDir;
          if (this.extract(down.DestFile, ext_dir))
          {
            return ext_dir;
          }
        }
        else
        {
          return down.DestFile;
        }
      }
      return string.Empty;
    }

    void background_update(DownloadFile file, CancellationToken token)
    {
      LOG.wc("try client background update");

      if (Interlock.Set())
      {
        Task.Run(async () =>
        {
          try
          {
            while (!token.IsCancellationRequested)
            {
              var dest = await this.client_patch_download(file);
              if (!string.IsNullOrEmpty(dest))
              {
                this.BackgoundClientUpdated?.Invoke(this, dest);
                return;
              }

              // 5분마다 재 시도
              var ts = TimeSpan.FromMinutes(5);
              LOG.wc($"client update failed, retrying after: {ts}");
              await Task.Delay(ts, token);
            }
          }
          catch (Exception ex)
          {
            LOG.except(ex);
          }
          finally
          {
            Interlock.Reset();
          }
        });
      }
    }

    /// <summary>
    /// 압축파일을 지정된 위치에 해제하고,압축 파일을 삭제한다
    /// </summary>
    /// <param name="path">압축 파일</param>
    /// <param name="extract_dir">압축 해제 위치</param>
    /// <returns></returns>
    bool extract(string path, string extract_dir)
    {
      FileHelper.ClearDirectory(extract_dir);

      if (Path.GetExtension(path).ToLower() == ".zip")
      {
        ZipFile.ExtractToDirectory(path, extract_dir);
        File.Delete(path); // downloaded zip 파일 삭제
      }
      else
      {
        // 다운로드 파일이 압축파일이 아니면 압축 해제 폴더로 이동한다
        File.Move(path, Path.Combine(extract_dir, path));
      }
      return true;
    }
  }
}