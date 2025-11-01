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
    public string ClientID { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;  // 30000

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
        ServerPort = key.Port.ToInt(),
        HTTP_HOME = key.UploadURL,
        ProductName = key.ProductName,
      };
    }
  }
}