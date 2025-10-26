using System;

namespace Framework.Network.HTTP
{
  public class DownloadProgressReport
  {
    public long Index;
    public string FileName = string.Empty;
    public int Percent;
    public string DownloadSpeed = string.Empty;
    public string DownloadSize = string.Empty;
    public string TimeLeft = string.Empty;

    public override string ToString()
    {
      return $"{FileName}, {Percent}%, speed: {DownloadSpeed}, size: {DownloadSize}, left: {TimeLeft}";
    }
  }

  /// <summary>
  /// 파일 다운로드 진행율 및 속도 계산
  /// - 500ms 이내에는 전송하지 않는다
  /// - 퍼센트는 int로 계산한다
  /// - 파일크기는 꼭 long을 사용하자
  /// </summary>
  public class DownloadProgress : IProgress<DownloadProgressReport>
  {
    public event EventHandler<DownloadProgressReport> Reported;
    public DownloadProgress()
    {
    }

    public void Report(DownloadProgressReport o)
    {
      this.Reported?.Invoke(this, o);
    }
  }
}