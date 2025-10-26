using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Common
{
  public class NetworkHelper
  {
    public static string ParseIP(string ip)
    {
      Regex reg = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
      var result = reg.Matches(ip);

      if (result.Count > 0)
      {
        return result[0].Value;
      }
      else
      {
        throw new Exception($"IP Parse error : {ip}");
      }
    }

    public static string GetLocalIPAddress()
    {
      var host = Dns.GetHostEntry(Dns.GetHostName());
      foreach (var ip in host.AddressList)
      {
        if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
        {
          return ip.ToString();
        }
      }
      return string.Empty;
    }
    static Dictionary<int, NetworkInterface> _nics = new Dictionary<int, NetworkInterface>();
    public static string GetMacAddress()
    {
      string mac = string.Empty;
      if (ActiveNIC(out var nic))
      {
        var values = nic?.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")) ?? new List<string>();
        mac = string.Join(":", values);
      }
      return mac;
    }
    /// <summary>
    /// 활성화된 모든 NIC 가져오기
    /// </summary>
    /// <returns></returns>
    static IEnumerable<NetworkInterface> GetAllActiveNIC()
       => NetworkInterface.GetAllNetworkInterfaces()
              .Where(x => x.OperationalStatus == OperationalStatus.Up);

    static bool ActiveNIC(out NetworkInterface nic)
    {
      nic = GetAllActiveNIC().FirstOrDefault();
      return nic != null;
    }
  }
}