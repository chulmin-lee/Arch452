using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  public class PlaylistUpdater : PlaylistUpdaterBase
  {
    public PlaylistUpdater(ILocation location) : base(location) { }
    protected override PackageConfigurations CreatePackageConfigurationsImpl(string xml)
    {
      return PackageConfigurationsFactory.Create(xml, this.Location);
    }
  }
}