using Common;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UIControls
{
  public static partial class ImageLoader
  {
    static string BaseLocation;

    static ImageLoader()
    {
      var asm = Assembly.GetExecutingAssembly().GetName().Name;
      BaseLocation = $"pack://application:,,,/{asm};component/Resources";
    }

    public static string GetResourceURL(string path)
    {
      var asm = Assembly.GetExecutingAssembly().GetName().Name;
      return $"pack://application:,,,/{asm};component/{path}";
    }

    public static ImageSource GetImage(string name)
    {
      var pack = $"{BaseLocation}/{name}";
      var uri = new Uri(pack);

      return GetIconPack(uri);
    }
    public static ImageSource GetGrayImage(string name)
    {
      var pack = $"{BaseLocation}/{name}";
      var uri = new Uri(pack);

      return GetGrayScale(uri);
    }
    public static ImageSource LoadImage(string path)
    {
      try
      {
        return BitmapFromUri(path);
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        return null;
      }
    }
    static BitmapImage BitmapFromUri(string path)
    {
      try
      {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(path, UriKind.Absolute);
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        return bitmap;
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        return null;
      }
    }
    public static async Task<ImageSource> GetImageFromUrlAsync(string url)
    {
      try
      {
        using (HttpClient HTTP = new HttpClient())
        {
          using (var response = await HTTP.GetAsync(url))
          {
            if (response.StatusCode == HttpStatusCode.OK)
            {
              using (var ms = new MemoryStream())
              {
                using (var source = await response.Content.ReadAsStreamAsync())
                {
                  await source.CopyToAsync(ms).ConfigureAwait(false);

                  ms.Position = 0;

                  var bitmap = new BitmapImage();
                  bitmap.BeginInit();
                  bitmap.CacheOption = BitmapCacheOption.OnLoad; // 이미지 로딩 속도를 향상시킵니다.
                  bitmap.StreamSource = ms;
                  bitmap.EndInit();
                  return bitmap;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        LOG.except(ex);
      }
      return null;
    }

    public static ImageSource GetImageFromStream(MemoryStream ms)
    {
      try
      {
        var bitmap = new BitmapImage();
        ms.Position = 0;
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad; // 이미지 로딩 속도를 향상시킵니다.
        bitmap.StreamSource = ms;
        bitmap.EndInit();
        return bitmap;
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        return null;
      }
    }

    static ImageSource ResizedImage(ImageSource source, int width, int height, int margin)
    {
      var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

      var group = new DrawingGroup();
      RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
      group.Children.Add(new ImageDrawing(source, rect));

      var drawingVisual = new DrawingVisual();
      using (var drawingContext = drawingVisual.RenderOpen())
        drawingContext.DrawDrawing(group);

      var resizedImage = new RenderTargetBitmap(
        width, height,         // Resized dimensions
        96, 96,                // Default DPI values
        PixelFormats.Default); // Default pixel format

      resizedImage.Render(drawingVisual);

      //return BitmapFrame.Create(resizedImage);
      return resizedImage;
    }

    static ImageSource GetIconPack(Uri uri) => new BitmapImage(uri);
    static ImageSource GetGrayScale(Uri uri)
    {
      try
      {
        var map = new BitmapImage(uri);
        BitmapImage grayBitmap = new BitmapImage();
        grayBitmap.BeginInit();
        grayBitmap.UriSource = uri;

        // 메모리 절약을 위해서크기르 조절할 수 있다.
        grayBitmap.DecodePixelWidth = map.PixelWidth;
        grayBitmap.EndInit();

        var src = new FormatConvertedBitmap();
        src.BeginInit();
        src.Source = grayBitmap;
        src.DestinationFormat = PixelFormats.Gray32Float;
        src.EndInit();
        return src;
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        return null;
      }
    }
  }
}