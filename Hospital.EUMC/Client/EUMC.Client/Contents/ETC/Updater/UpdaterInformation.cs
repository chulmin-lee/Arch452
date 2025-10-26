using EUMC.ClientServices;
using ServiceCommon.ClientServices;
using System.Collections.ObjectModel;
using System.Linq;
using UIControls;

namespace EUMC.Client
{
  internal class UpdaterInformation : ContentInformation
  {
    public UpdaterViewConfig.ContentConfig CONFIG { get; set; }
    public string Title { get => _title; set => Set(ref _title, value); }
    public string Version { get; set; } = string.Empty;
    int _no = 1;

    public ObservableCollection<DownloadProgress> Items { get; } = new ObservableCollection<DownloadProgress>();
    public UpdaterInformation(UpdaterViewConfig config) : base(1)
    {
      this.Version = config.Config.Version;
      this.CONFIG = config.Config;
      this.Title = "업데이트 확인 중";
    }

    internal bool Update(DownloadProgressMessage m)
    {
      //_isUpdate = true;
      this.Title = $"업데이트 진행중";

      var find = this.Items.Where(x => x.Index == m.Index).FirstOrDefault();
      if (find != null)
      {
        find.Update(m);
      }
      else
      {
        this.Items.Add(new DownloadProgress(m, _no++));
      }
      if (this.Items.Count > this.CONFIG.ItemRows)
      {
        this.Items.RemoveAt(0);
      }
      return true;
    }
    string _title = string.Empty;
  }

  internal class DownloadProgress : ViewModelBase
  {
    public long Index { get; set; }
    public string FileName { get; set; }
    public long Length { get; set; }
    public int Percent { get => _percent; set => Set(ref _percent, value); }
    public string DownloadSpeed { get => _downloadSpeed; set => Set(ref _downloadSpeed, value); }
    public string DownloadSize { get => _downloadSize; set => Set(ref _downloadSize, value); }
    public string TimeLeft { get => _timeLeft; set => Set(ref _timeLeft, value); }
    public int No { get; set; }

    public DownloadProgress(DownloadProgressMessage m, int no)
    {
      this.Index = m.Index;
      this.FileName = m.FileName;
      this.Percent = m.Percent;
      this.DownloadSpeed = m.DownloadSpeed;
      this.DownloadSize = m.DownloadSize;
      this.TimeLeft = m.TimeLeft;
      this.No = no;
    }

    public void Update(DownloadProgressMessage m)
    {
      this.Percent = m.Percent;

      if (this.Percent >= 100)
      {
        this.DownloadSpeed = string.Empty;
        this.DownloadSize = string.Empty;
        this.TimeLeft = string.Empty;
      }
      else
      {
        this.DownloadSpeed = m.DownloadSpeed;
        this.DownloadSize = m.DownloadSize;
        this.TimeLeft = m.TimeLeft;
      }
    }
    int _percent = 0;
    string _downloadSpeed = string.Empty;
    string _downloadSize  = string.Empty;
    string _timeLeft = string.Empty;
  }
}