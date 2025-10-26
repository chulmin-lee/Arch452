using Common;
using ServiceCommon.ClientServices;
using System;
using System.IO;
using System.Xml.Serialization;

namespace EUMC.ClientServices
{

  [XmlRoot(ElementName = "Root")]
  public class KeyXml
  {
    public string UploadURL { get; set; } = string.Empty;  // http://192.168.0.30
    public string ApiURL { get; set; } = string.Empty;  // http://127.0.0.1<
    public string ClientID { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ApiPort { get; set; } = string.Empty;  // 30000

    public static ServerConfig Load(string path)
    {
      if (!File.Exists(path))
      {
        throw new FileNotFoundException($"{path}");
      }

      var key = XmlHelper.Load<KeyXml>(path);

      if (key == null) throw new Exception("key parsing failed");

      return new ServerConfig
      {
        ClientID = key.ClientID.ToInt(),
        ServerIP = NetworkHelper.ParseIP(key.UploadURL),
        ServerPort = key.ApiPort.ToInt(),
        ApiServerIP = NetworkHelper.ParseIP(key.ApiURL),
        ApiServerPort = key.ApiPort.ToInt(),
        HTTP_HOME = key.UploadURL,
        ProductName = key.ProductName,
      };
    }
  }
}