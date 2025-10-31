using Common;
using Framework.DataSource;
using Framework.Network.HTTP;
using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EUMC.HospitalService
{
  internal class DR_PHOTO_Extractor : DataExtractor<DR_PHOTO_DTO>
  {
    Config CONFIG;
    public DR_PHOTO_Extractor(IHospitalMemberOwner owner, Config config) : base(owner, DATA_ID.DR_PHOTO)
    {
      this.CONFIG = config;
      this.Interval = config.Interval;
      Directory.CreateDirectory(this.CONFIG.PhotoDir);
    }

    protected override List<DR_PHOTO_DTO> query() => this.Repository.DR_PHOTO();

    protected override INotifyData<DATA_ID> data_mapping(UpdateData<DR_PHOTO_DTO> updated)
    {
      // 삭제
      foreach (var p in updated.Deleted)
      {
        //var path = this.photo_path(p.GetFileName());
        var path = this.CONFIG.GetFilePath(p.GetFileName());
        if (File.Exists(path))
        {
          File.Delete(path);
        }
      }

      // download
      var add_tasks = new List<Task>();
      foreach (var p in updated.AddedAndUpdated)
      {
        var path = this.CONFIG.GetFilePath(p.GetFileName());
        if (!File.Exists(path))
        {
          add_tasks.Add(Photo_download_async(p));
        }
        else
        {
          p.Success = true;
        }
      }
      Task.WhenAll(add_tasks).Wait();

      // 파일 정리
      var local_files = Directory.GetFiles(this.CONFIG.PhotoDir).Select(x => Path.GetFileName(x)).ToList();
      var all_files = updated.All.Select(x => x.GetFileName()).ToList();

      var remain = local_files.Except(all_files).ToList();
      // 파일정리
      foreach (var p in remain)
      {
        File.Delete(this.CONFIG.GetFilePath(p));
      }

      var o = new UpdateData<DR_PHOTO_POCO>()
      {
        Constant = Mapper.Map<DR_PHOTO_DTO[], List<DR_PHOTO_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<DR_PHOTO_DTO[], List<DR_PHOTO_POCO>>(updated.Updated.Where(x => x.Success).ToArray()),
        Deleted  = Mapper.Map<DR_PHOTO_DTO[], List<DR_PHOTO_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<DR_PHOTO_DTO[], List<DR_PHOTO_POCO>>(updated.Added.Where(x => x.Success).ToArray()),
      }.Compose();

      o.All.ForEach(x =>
      {
        x.PhotoUrl = this.CONFIG.GetFileUrl(x.PhotoUrl); // 파일명만 온다
      });
      return new DataEventData<DR_PHOTO_POCO>(this.ID, o);
    }

    async Task Photo_download_async(DR_PHOTO_DTO p)
    {
      if (!string.IsNullOrEmpty(p.GetFileName()))
      {
        var dest = this.CONFIG.GetFilePath(p.GetFileName()); //   Path.Combine(this.photoStorage.PhotoDir, p.GetFilename());
        LOG.d($"{this.ID} down: {p.IMAGE_PATH}, save : {dest}");
        var content = new DownloadFile(p.IMAGE_PATH, dest);
        p.Success = await HttpDownloader.FileDownloadAsync(content).ConfigureAwait(false);
      }
    }

    internal class Config : DataConfig
    {
      public string PhotoDir { get; set; }
      public string PhotoUrl { get; set; }
      public int Interval { get; set; } = 60;
      public Config() : base(DATA_ID.DR_PHOTO)
      {
      }
      public string GetFilePath(string name)
      {
        return Path.Combine(this.PhotoDir, name);
      }
      public string GetFileUrl(string name)
      {
        return $"{this.PhotoUrl}/{name}";
      }

    }
  }
}