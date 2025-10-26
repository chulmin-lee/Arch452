using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Network.HTTP
{
  public static class HttpDownloader
  {
    public static event EventHandler<DownloadProgressReport> DownloadProgressReported;
    static DownloadProgress _progress;
    static HttpClient HTTP;

    static HttpDownloader()
    {
      HTTP = new HttpClient();
      _progress = new DownloadProgress();
      _progress.Reported += (s, e) => DownloadProgressReported?.Invoke(null, e);
    }

    public static async Task<bool> FileDownloadAsync(DownloadFile content, CancellationToken token = default(CancellationToken))
    {
      if (File.Exists(content.DestFile))
      {
        File.Delete(content.DestFile); // 기존 파일 삭제
      }
      else if (!content.DestFile.CreateFileDirectory())
      {
        LOG.ec($"{content.DestFile} create directory failed");
        return false;
      }

      LOG.dc($"download: {content.SourceUrl} start, file: {content.FileName}");

      try
      {
        var response = await HTTP.GetAsync(content.SourceUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        if (response.StatusCode != HttpStatusCode.OK)
        {
          LOG.ec($"error : {response.StatusCode}");
          return false;
        }

        var content_length = response.Content.Headers.ContentLength ?? 0L;
        if (content_length == 0)
        {
          LOG.ec($"error : Length is 0");
          return false;
        }
        content.SetContentLength(content_length);

        using (var destination = new FileStream(content.DestFile, FileMode.Create, FileAccess.Write, FileShare.None))
        {
          using (var source = await response.Content.ReadAsStreamAsync())
          {
            if (content.UseProgress)
            {
              await download_progress_async(source, destination, 81920, content, token);
            }
            else
            {
              await source.CopyToAsync(destination).ConfigureAwait(false);
            }
            return true;
          }
        }
      }
      catch (HttpRequestException ex)
      {
#if NET8_0
        // NET 8 부터는 상태코드를 얻을 수 있다.
        LOG.ec($"download failed: code = {ex.StatusCode}, error = {ex.HttpRequestError}");
#else
      LOG.except(ex);
#endif
      }
      catch (Exception ex)
      {
        LOG.except(ex);
      }
      return false;
    }

    static async Task download_progress_async(Stream source, Stream destination, int buffer_size, DownloadFile content, CancellationToken token = default(CancellationToken))
    {
      #region exception
      if (buffer_size < 0) throw new ArgumentOutOfRangeException(nameof(buffer_size));
      if (source is null) throw new ArgumentNullException(nameof(source));
      if (!source.CanRead) throw new InvalidOperationException($"'{nameof(source)}' is not readable.");
      if (destination == null) throw new ArgumentNullException(nameof(destination));
      if (!destination.CanWrite) throw new InvalidOperationException($"'{nameof(destination)}' is not writable.");
      #endregion exception

      var buffer = new byte[buffer_size];
      long total_read = 0;
      int read;

      // 다운로드 진행률 보고
      Queue<double> history = new Queue<double>();
      DateTime lastTime = DateTime.Now;
      long received_size = 0;

      while ((read = await source.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false)) != 0)
      {
        await destination.WriteAsync(buffer, 0, read, token).ConfigureAwait(false);
        total_read += read;

        //await Task.Delay(600); // Test로 600ms 대기 (progress 보고를 위해)

        #region progress
        {
          var now = DateTime.Now;
          TimeSpan ts = now - lastTime;
          lastTime = now;

          //---------------------------
          // download speed
          //---------------------------
          double speed = (total_read - received_size) / ts.TotalSeconds;

          if (speed > 0)
          {
            history.Enqueue(speed);
            if (history.Count > 5)
            {
              history.Dequeue();
            }
          }

          received_size = total_read;

          if (ts.TotalMilliseconds > 500)
          {
            speed = history.Average();

            string time_left = string.Empty;
            if (total_read > 0 && total_read < content.Length)
            {
              double left_second = (content.Length - total_read) / speed;
              if (left_second > 0)
              {
                time_left = TimeSpan.FromSeconds(left_second).ToString(@"hh\:mm\:ss");
              }
            }
            _progress.Report(new DownloadProgressReport
            {
              Index = content.Index,
              FileName = content.FileName,
              Percent = (int)(total_read * 100 / content.Length), // int로 계산
              DownloadSpeed = $"{speed.ToByteSize()}/s",
              DownloadSize = $"{total_read.ToByteSize()} / {content.Length.ToByteSize()}",
              TimeLeft = time_left
            });
          }
          else if (received_size >= content.Length)
          {
            // 다운로드가 완료되었을 때도 보고
            _progress.Report(new DownloadProgressReport
            {
              Index = content.Index,
              FileName = content.FileName,
              Percent = 100, // 완료시 100%
              TimeLeft = "0"
            });
          }
        }
        #endregion progress
      }
    }
    /// <summary>
    /// 이미지를 MemoryStream으로 다운로드 한다 (다른 assembly를 참조하지 않기 위함)
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<MemoryStream> GetMemoryStreamAsync(string url)
    {
      try
      {
        using (var response = await HTTP.GetAsync(url))
        {
          if (response.StatusCode == HttpStatusCode.OK)
          {
            var destination = new MemoryStream();
            using (var source = await response.Content.ReadAsStreamAsync())
            {
              await source.CopyToAsync(destination).ConfigureAwait(false);
            }
            return destination;
          }
        }
      }
      catch (Exception ex)
      {
        LOG.except(ex);
      }
      return null;
    }

    public static async Task<string> GetStringAsync(string url)
    {
      try
      {
        return await HTTP.GetStringAsync(url);
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        return string.Empty; // 실패시 빈 문자열 반환
      }
    }

    public static async Task<long> GetContentLengthAsync(string url, CancellationToken token = default(CancellationToken))
    {
      if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
      try
      {
        var response = await HTTP.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        switch (response.StatusCode)
        {
          case HttpStatusCode.OK:
            return response.Content.Headers.ContentLength ?? 0L;
          default:
            LOG.ec($"error: {url}, status code: {response.StatusCode}");
            return -1L; // 실패시 -1 반환
        }
      }
      catch (HttpRequestException ex)
      {
        LOG.ec($"GetContentLengthAsync failed: {url}, message: {ex.Message}");
      }
      return -1L; // 실패시 -1 반환
    }
  }
}