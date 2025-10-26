namespace ServiceCommon.ClientServices
{
  public interface ILocation
  {
    string VersionJsonURL { get; }
    string VersionJsonLocal { get; }
    string PlaylistXmlURL { get; }
    string PlaylistXmlPath { get; }

    // update
    string ExtractDir { get; }
    string DownloadDir { get; }
    string UpdaterPath { get; }
    string ContentDir { get; }

    string ContentPath(string filename);
    string ContentUrl(string filename);
    string UpdateFileUrl(string name);
    // client update 파일 다운로드 위치
    string DownloadFilePath(string name);
  }
}