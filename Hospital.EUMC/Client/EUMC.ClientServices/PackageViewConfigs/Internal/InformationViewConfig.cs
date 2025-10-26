using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class InformationViewConfig : PackageViewConfig
  {
    public InformationViewConfig() : base(PACKAGE.INFORMATION)
    {
    }
  }
}