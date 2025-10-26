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
  internal class DR_PHOTO_Extractor : DataExtractor<PHOTO_DTO>
  {
    string _photo_dir;
    public DR_PHOTO_Extractor(IHospitalMemberOwner owner, Config config) : base(owner, DATA_ID.DR_PHOTO)
    {
      this.Interval = config.Interval;
      _photo_dir = Path.Combine(owner.ServiceDir, config.PhotoDirName);
      Directory.CreateDirectory(_photo_dir);
    }

    protected override List<PHOTO_DTO> query() => this.Repository.PHOTO_DATA();

    protected override INotifyData<DATA_ID> data_mapping(UpdateData<PHOTO_DTO> updated)
    {
      // 삭제
      foreach (var p in updated.Deleted)
      {
        var path = this.photo_path(p.GetFilename());
        if (File.Exists(path))
        {
          File.Delete(path);
        }
      }

      // download
      var add_tasks = new List<Task>();
      foreach (var p in updated.AddedAndUpdated)
      {
        var path = this.photo_path(p.GetFilename());
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
      var local_files = Directory.GetFiles(_photo_dir).Select(x => Path.GetFileName(x)).ToList();
      var all_files = updated.All.Select(x => x.GetFilename()).ToList();

      var remain = local_files.Except(all_files).ToList();
      // 파일정리
      foreach (var p in remain)
      {
        File.Delete(this.photo_path(p));
      }

      var o = new UpdateData<PHOTO_POCO>()
      {
        Constant = Mapper.Map<PHOTO_DTO[], List<PHOTO_POCO>>(updated.Constant.ToArray()),
        Updated  = Mapper.Map<PHOTO_DTO[], List<PHOTO_POCO>>(updated.Updated.Where(x => x.Success).ToArray()),
        Deleted  = Mapper.Map<PHOTO_DTO[], List<PHOTO_POCO>>(updated.Deleted.ToArray()),
        Added    = Mapper.Map<PHOTO_DTO[], List<PHOTO_POCO>>(updated.Added.Where(x => x.Success).ToArray()),
      }.Compose();

      // 사진 경로 (HTTP 인 경우 URL로 변경 필요)
      o.All.ForEach(x => x.FilePath = this.photo_path(x.Filename));
      return new DataEventData<PHOTO_POCO>(this.ID, o);
    }

    string photo_path(string filename) => Path.Combine(_photo_dir, filename);
    async Task Photo_download_async(PHOTO_DTO p)
    {
      if (!string.IsNullOrEmpty(p.GetFilename()))
      {
        var dest = this.photo_path(p.GetFilename()); //   Path.Combine(this.photoStorage.PhotoDir, p.GetFilename());
        LOG.d($"{this.ID} down: {p.IMAGE_PATH}, save : {dest}");
        var content = new DownloadFile(p.IMAGE_PATH, dest);
        p.Success = await HttpDownloader.FileDownloadAsync(content).ConfigureAwait(false);
      }
    }

    internal class Config : DataConfig
    {
      public string PhotoDirName { get; set; } = string.Empty;
      public bool UseHttp { get; set; } = false;
      public string PhotoUrl { get; set; } = string.Empty;
      public int Interval { get; set; } = 60;
      public Config() : base(DATA_ID.DR_PHOTO)
      {
      }
    }
  }
}