using Common;
using System.Collections.Generic;

namespace EUMC.HospitalService
{
  public static class HospitalExtension
  {
    static MaskedNameHelper _instance;
    static MaskedNameHelper Instance => _instance ?? (_instance = new MaskedNameHelper(MaskPosition.Right, 0.5, new List<string> { "아기" }));

    public static string MaskedName(this string name)
    {
      return Instance.MaskedName(name);
    }

  }
}
