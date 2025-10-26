using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UIControls
{
  public static partial class ImageLoader
  {
    /// <summary>
    /// 의사 사진을 base64 string으로 변환
    /// - 지정된 해상도 이하로 조절한다.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="save"></param>
    /// <returns></returns>
    public static string GetDoctorPhoto(string path, int width = 0, int height = 0, int maxlength = 0, bool save = false)
    {
      if (!File.Exists(path))
      {
        return string.Empty;
      }

      var fi = new FileInfo(path);
      if (width == 0 || height == 0 || fi.Length < maxlength)
      {
        var arr = File.ReadAllBytes(path);
        return Convert.ToBase64String(arr);
      }

      // image resize
      var image = BitmapFromUri(path);
      if (image == null)
        return string.Empty;

      if (image.PixelWidth <= width && image.PixelHeight <= height)
      {
        var arr = File.ReadAllBytes(path);
        return Convert.ToBase64String(arr);
      }

      var percent = Math.Min(width / (float)image.PixelWidth, height / (float)image.PixelHeight);
      int destWidth = (int)(image.PixelWidth * percent);
      int destHeight = (int)(image.PixelHeight * percent);

      var resize = ResizedImage(image, destWidth, destHeight, 0);

      var bitmapSource = resize as BitmapSource;
      var encoder = new JpegBitmapEncoder();
      if (bitmapSource != null)
      {
        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

        using (var stream = new MemoryStream())
        {
          encoder.Save(stream);
          var bytes = stream.ToArray();
          if (save)
          {
            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);
            var ext = Path.GetExtension(path);
            var temp = Path.Combine(dir, $"temp-{name}.{ext}");
            if (File.Exists(temp))
            {
              File.Delete(temp);
            }
            File.WriteAllBytes(temp, bytes);
          }
          return Convert.ToBase64String(bytes);
        }
      }
      return string.Empty;
    }

    public static ImageSource LoadDoctorImage(string path)
    {
      var image = BitmapFromUri(path);
      if (image != null)
      {
        int width = image.PixelWidth;
        int skip = 0; // (int)(image.PixelHeight * 0.1);
        int height = (int)(image.PixelHeight * 0.9);

        return new CroppedBitmap(image, new Int32Rect(0, skip, width, height));
      }
      return null;
    }
    public static BitmapImage BitmapFromBase64(string photo)
    {
      if (string.IsNullOrEmpty(photo))
        return null;

      byte[] arr = Convert.FromBase64String(photo);
      BitmapImage image = new BitmapImage();
      MemoryStream ms = new MemoryStream(arr);
      image.BeginInit();
      image.StreamSource = ms;
      image.EndInit();
      return image;
    }
    public static List<ImageSource> Splite(string uri, int count)
    {
      var list = new List<ImageSource>();
      var bitmap = new BitmapImage(new Uri(uri));
      int w = bitmap.PixelWidth;
      int h = bitmap.PixelHeight;

      int width = w / count;

      for (int i = 0; i < count; i++)
      {
        list.Add(new CroppedBitmap(bitmap, new Int32Rect(width * i, 0, width, h)));
      }
      return list;
    }
    public static void SaveImageSource(BitmapSource icon, string path)
    {
      var encoder = new JpegBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(icon));
      using (var stream = new MemoryStream())
      {
        encoder.Save(stream);
        var bytes = stream.ToArray();
        if (File.Exists(path))
        {
          File.Delete(path);
        }
        File.WriteAllBytes(path, bytes);
      }
    }
  }
}