using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UIControls
{
  public class MediaManager
  {
    public bool IsEmpty => Contents.Count == 0;
    public int Count => Contents.Count;
    //public int ImageCount => Contents.Count(c => c.IsImage);
    public int ImageCount => Contents.Where(x => x.IsImage).Count();
    List<MEDIA_FILE> Contents = new List<MEDIA_FILE>();
    int Index = -1;

    public void SetContents(List<MEDIA_FILE> list)
    {
      Index = -1;
      Contents = list;
    }
    public MEDIA_FILE GetNextContent()
    {
      if (!IsEmpty)
      {
        this.Index++;
        if (this.Index >= this.Contents.Count)
        {
          this.Index = 0;
        }
        return Contents[Index];
      }
      return null;
    }
    /// <summary>
    /// 헌재 재생중인 컨텐츠 삭제
    /// </summary>
    public void RemoveCurrent()
    {
      if (0 <= Index && Index < Contents.Count)
      {
        Contents.RemoveAt(Index);
        Index = Math.Min(Index, Contents.Count - 1);
      }
    }
  }

  public class MEDIA_FILE
  {
    public bool IsImage;
    public bool IsVideo => !this.IsImage;
    public string ContentPath = string.Empty;
    public int MediaId;
    public Uri GetUri()
    {
      return new Uri(ContentPath);
    }
    public bool FileExist()
    {
      return File.Exists(ContentPath);
    }
  }
}