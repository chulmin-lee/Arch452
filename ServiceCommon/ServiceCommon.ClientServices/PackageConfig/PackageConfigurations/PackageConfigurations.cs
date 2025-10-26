using Common;
using Framework.Network.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.ClientServices
{
  /// <summary>
  /// 다양한 playlist.xml 포멧은 모두 PackageConfigurations으로 변환되어 처리된다
  /// </summary>
  public class PackageConfigurations
  {
    PackageScheduler _scheduler = new PackageScheduler();
    List<string> DownloadFiles = new List<string>();
    public bool HasDownloadFiles => this.DownloadFiles.Any();
    ILocation Location { get; set; }
    public PACKAGE_ERROR PackageError { get; set; } = PACKAGE_ERROR.Success;
    public bool Success => this.PackageError == PACKAGE_ERROR.Success;
    public PackageConfigurations(PackageScheduler scheduler, ILocation location)
    {
      _scheduler = scheduler;
      this.Location = location;
      DownloadFiles = this.GetContentList(_scheduler.GetDownloadContents(), this.Location.ContentDir);
      if (!this.HasDownloadFiles)
      {
        _scheduler.ConetentUpdated(this.Location.ContentDir);
      }
    }
    public PackageConfigurations(PACKAGE_ERROR error)
    {
      this.PackageError = error;
    }
    /// <summary>
    /// 다운로드해야할 파일 목록을 리턴한다.
    /// 사용하지 않는 contents는 삭제한다
    /// </summary>
    List<string> GetContentList(List<REMOTE_FILE> list, string content_dir)
    {
      // 사용하지 않는 local 파일 삭제
      foreach (var fi in new DirectoryInfo(content_dir).GetFiles())
      {
        var find = list.Where(x => x.FileName == fi.Name).FirstOrDefault();

        if (find != null)
        {
          int local_size = (int)fi.Length;
          if (find.Size != local_size)
          {
            LOG.dc($"update size: {fi.FullName} -> {find.Size} != {local_size}");
            fi.Delete(); // 사이즈가 다르면 삭제
          }
        }
        else
        {
          LOG.dc($"delete not used: {fi.FullName}");
          fi.Delete(); // 사용하지 않는 파일 삭제
        }
      }

      // 다운로드할 목록을 만든다
      var download_list = list.Where(x => !File.Exists(Path.Combine(content_dir, x.FileName))).ToList();

      return download_list.Select(x => x.Path).ToList(); // 33/photo.jpg 형식
    }
    public async Task ContentDownloadAsync()
    {
      if (this.Location == null || _scheduler == null)
      {
        throw new Exception("error");
      }

      if (this.DownloadFiles.Any())
      {
        foreach (var remote_file in this.DownloadFiles)
        {
          var url = this.Location.ContentUrl(remote_file);
          var local = this.Location.ContentPath(Path.GetFileName(remote_file));

          var download = new DownloadFile(url, local, true);
          await HttpDownloader.FileDownloadAsync(download);
        }

        _scheduler.ConetentUpdated(this.Location.ContentDir);
      }
    }
    /// <summary>
    /// 현재 시간에 맞는 스케쥴을 반환한다.
    /// </summary>
    public PlaylistSchedule GetCurrentSchedule()
    {
      return this.GetCurrentSchedule(DateTime.Now);
    }
    public PlaylistSchedule GetCurrentSchedule(DateTime date)
    {
      return this.Success ? _scheduler.GetCurrentSchedule(date)
                          : new PlaylistSchedule(this.PackageError);
    }
  }
}