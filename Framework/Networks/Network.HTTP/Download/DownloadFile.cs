using System.IO;
using System.Threading;

namespace Framework.Network.HTTP
{
  public class DownloadFile
  {
    static long _index = 0;
    /// <summary>
    /// 다운로드 파일마다 고유의 id 를 가진다
    /// </summary>
    public long Index { get; private set; }
    public string SourceUrl { get; private set; } = string.Empty;
    public string DestFile { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public long Length { get; private set; }
    public bool UseProgress { get; private set; }
    public DownloadFile(string url, string local_path, bool useProgress = false)
    {
      this.Index = Interlocked.Increment(ref _index);
      this.SourceUrl = url;
      this.DestFile = local_path;
      this.FileName = Path.GetFileName(local_path);
      this.UseProgress = useProgress;
    }
    public void SetContentLength(long size)
    {
      this.Length = size;
    }
  }
}