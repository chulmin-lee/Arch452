using System;
using System.IO;

namespace ServiceCommon.ClientServices
{
  public class Location : ILocation
  {
    public Location(ServerConfig o, string updater) : this(o.HTTP_HOME, o.ProductName, o.ClientID, updater)
    {
    }
    public Location(string http, string product, int clientId, string updater)
    {
      this.StartupDir = AppDomain.CurrentDomain.BaseDirectory;

      this.VersionJsonURL = $"{http}/update/{product}/Version.json";
      this.VersionJsonLocal = get_path("Version.json");

      this.PlaylistXmlURL = $"{http}/playlist/{product}/{clientId}/Playlist.xml";
      this.PlaylistXmlPath = get_path("Playlist.xml");

      _update_url = $"{http}/update/{product}";
      _content_url = $"{http}/contents/{product}";

      this.DownloadDir = get_path("Download");
      this.ExtractDir = get_path("Temp");
      this.ContentDir = get_path("Contents");
      this.UpdaterPath = get_path(updater);

      Directory.CreateDirectory(this.DownloadDir);
      Directory.CreateDirectory(this.ContentDir);
      Directory.CreateDirectory(this.ExtractDir);
    }

    public string VersionJsonURL { get; private set; }

    public string VersionJsonLocal { get; private set; }

    public string PlaylistXmlURL { get; private set; }

    public string PlaylistXmlPath { get; private set; }

    public string ExtractDir { get; private set; }

    public string DownloadDir { get; private set; }

    public string UpdaterPath { get; private set; }

    public string ContentDir { get; private set; }

    //public string HttpHome {get; private set; }

    public string ContentPath(string filename)
    {
      return Path.Combine(this.ContentDir, filename);
    }

    public string ContentUrl(string filename)
    {
      return $"{_content_url}/{filename}";
    }
    public string DownloadFilePath(string filename)
    {
      return Path.Combine(DownloadDir, filename);
    }

    public string UpdateFileUrl(string filename)
    {
      return $"{_update_url}/{filename}";
    }
    string get_path(string name) => Path.Combine(StartupDir, name);
    string StartupDir;
    string _update_url;
    string _content_url;
  }
}