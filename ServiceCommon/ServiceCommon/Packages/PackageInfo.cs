using System.Collections.Generic;

namespace ServiceCommon
{
  public class PackageInfo
  {
    public PACKAGE Package { get; set; } = PACKAGE.NONE;
    public string PackageName { get; set; } = string.Empty;
    public bool UseService { get; set; }
    public PackageInfo() { }
    public PackageInfo(string packageName, PACKAGE package, bool use_service = true)
    {
      this.PackageName = packageName;
      this.Package = package;
      this.UseService = use_service;
    }
    public PackageInfo(string packageName, PACKAGE_ERROR err, string message)
    {
      this.PackageName = packageName;
      this.Package = PACKAGE.ERROR_PACKAGE;
      this.UseService = false;
      this.ErrorPackage = new ErrorPackage
      {
        ErrorCode = err,
        ErrorMessage = $"[{packageName}] {message}"
      };
    }

    public EmergencyPackage Emergency { get; set; }
    public EmergencyOfficePackage EmergencyOffice { get; set; }
    public EmergencyIsolationPackage EmergencyIsolation { get; set; }
    public IcuPackage Icu { get; set; }
    public WardRoomPackage WardRoom { get; set; }

    // OPD
    public OpdRoomPackage OpdRoom { get; set; }
    public InspectionPackage Inspection { get; set; }
    public AngPackage Angiography { get; set; }
    public EndoPackage Endoscope { get; set; }
    // funeral
    public FuneralPackage Funeral { get; set; }

    // ticket
    public TicketPackage Ticket { get; set; }

    // common
    public ErrorPackage ErrorPackage { get; set; }

    // internal
    public List<PackageRoomConfig> RoomConfigs { get; set; } = new List<PackageRoomConfig>();
  }

  public class PackageRoomConfig
  {
    public string DeptCode { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public string DurationTime { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Key => $"{this.DeptCode}:{this.RoomCode}";
  }
}