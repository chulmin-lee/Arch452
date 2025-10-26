using Common;
using ServiceCommon;
using EUMC.ClientServices;
using System.Collections.ObjectModel;
using System.IO;
using UIControls;
using System.Linq;

namespace EUMC.Client
{
  internal class PromotionInformation : ViewModelBase
  {
    public PromotionInformation(PromotionViewConfig o)
    {
      this.IsPlaying = true;
      if (o.UseTV)
      {
        this.UseTV = true;
        this.TvChannel = o.ChannelNo;
      }
      else
      {
        this.UseMedia = true;
        this.Duration = o.Duration;
        this.MediaVolumn = o.Config.MediaVolumn; // 0~1.0
        this.Contents.Clear();

        foreach (var p in o.Config.ContentFiles.Where(x => x.IsMedia))
        {
          if (File.Exists(p.ContentPath))
          {
            this.Contents.Add(new MEDIA_FILE()
            {
              ContentPath = p.ContentPath,
              IsImage = p.IsImage,
              MediaId = p.Id,
            });
          }
        }
      }
    }
    internal void ScreenOnOff(bool screen)
    {
      this.IsPlaying = screen;
    }
    internal void Close()
    {
      this.IsPlaying = false;
    }

    #region Property
    public bool IsPlaying { get => _isPlaying; set => Set(ref _isPlaying, value); }
    public int MediaId
    {
      get => _media_id;
      set
      {
        if (Set(ref _media_id, value))
        {
          LOG.w($"Id = {value}");
          if (value > 0)
          {
            CLIENT_SERVICE.Send(new CLIENT_MEDIA_NOTI(CLIENT_SERVICE.ClientId, value));
          }
        }
      }
    }

    public ObservableCollection<MEDIA_FILE> Contents { get; set; } = new ObservableCollection<MEDIA_FILE>();
    public bool UseTV { get; set; }
    public int TvChannel { get; set; }
    public bool UseMedia { get; set; }
    public double MediaVolumn { get; set; }
    public int Duration { get; set; }
    bool _isPlaying = false;   // Run
    int _media_id = 0;
    #endregion Property
  }
}