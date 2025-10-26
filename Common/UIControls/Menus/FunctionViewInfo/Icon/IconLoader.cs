using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UIControls
{
  public static class IconLoader
  {
    /// <summary>
    /// icon image 파일을 주어진 크기의 이미지로 나눈다.
    /// </summary>
    /// <param name="uri">이미지 파일 경로</param>
    /// <param name="width">개별 이미지 폭</param>
    /// <param name="height">개별 이미지 높이</param>
    /// <param name="count">아이콘 갯수</param>
    /// <returns></returns>
    public static MenuIconPack GetIconPack(Uri uri, int width, int height, int count)
    {
      if (uri == null)
      {
        Debug.WriteLine("uri is null");
        return null;
      }

      if (width <= 0 || height <= 0 || count <= 0 || count > 4)
      {
        Debug.WriteLine($"invalid parameter : width={width}, height={height}, count = {count}");
        return null;
      }

      BitmapImage bitmap = null;
      try
      {
        bitmap = new BitmapImage(uri);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        return null;
      }

      int x = (bitmap.PixelWidth / count) - width;  // x 차이
      int y = (bitmap.PixelHeight) - height;        // y 차이

      if (x < 0 || y < 0)
      {
        Debug.WriteLine("image too small");
        return null;
      }

      int mx = x / 2;   // x margin
      int my = y / 2;   // y margin


      ImageSource[] source = new ImageSource[count];

      for (int i = 0; i < count; i++)
      {
        int sx = width * i + mx + i * x;
        source[i] = new CroppedBitmap(bitmap, new Int32Rect(sx, my, width, height));
      }

      var icons = new MenuIconPack();

      switch (count)
      {
        case 1:
          {
            icons.Normal = source[0];
          }
          break;
        case 2:
          {
            icons.Normal = source[0];
            icons.MouseOver = source[1];
          }
          break;
        case 3:
          {
            icons.Normal = source[0];
            icons.MouseOver = source[1];
            icons.Disabled = source[2];
          }
          break;
        case 4:
          {
            icons.Normal = source[0];
            icons.MouseOver = source[1];
            icons.Selected = source[2];
            icons.Disabled = source[3];
          }
          break;
      }
      return icons;
    }
    public static MenuIconPack GetIconPack(string path, int width, int height, int count)
    {
      if (File.Exists(path))
      {
        return GetIconPack(new Uri(path), width, height, count);
      }
      Debug.WriteLine($"file not found : {path}");
      return null;
    }
  }
}
