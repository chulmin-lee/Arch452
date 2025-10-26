using Common;
using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices
{
  internal class PackageViewConfigFactory
  {
    public static IPackageViewConfig Create(PlaylistSchedule s)
    {
      var p = create_package_info(s);
      switch (p.Package)
      {
        case PACKAGE.ER_PATIENT: return new ErPatientViewConfig(p, s);
        case PACKAGE.ICU: return new IcuViewConfig(p, s);
        case PACKAGE.OPERATION: return new OperationViewConfig(p, s);
        case PACKAGE.DELIVERY_ROOM: return new DeliveryRoomViewConfig(p, s);
        case PACKAGE.WARD_ROOMS: return new WardRoomViewConfig(p, s);
        case PACKAGE.OFFICE_SINGLE: return new OfficeSingleViewConfig(p, s);
        case PACKAGE.EXAM_SINGLE: return new ExamSingleViewConfig(p, s);
        case PACKAGE.OFFICE_MULTI: return new OfficeMultiViewConfig(p, s);
        case PACKAGE.EXAM_MULTI: return new ExamMultiViewConfig(p, s);
        case PACKAGE.ENDO: return new EndoViewConfig(p, s);
        case PACKAGE.DRUG: return new DrugViewConfig(p, s);
        case PACKAGE.PROMOTION: return new PromotionViewConfig(p, s);
        // internal
        case PACKAGE.NO_SCHEDULE: return new NoScheduleViewConfig();
        case PACKAGE.ERROR_PACKAGE: return new ErrorViewConfig(p, s);
      }
      LOG.ec($"{p.Package} not supported");
      return new ErrorViewConfig(s.PackageName);
    }

    static PackageInfo create_package_info(PlaylistSchedule s)
    {
      if (s.Success)
      {
        // 내부 오류가 있는 경우 ErrorPackage PackageInfo 리턴
        switch (s.PackageName)
        {
          #region Emergency
          case PackageNames.ER_PATIENT_ADULT: return PackageInfoFactory.ER_PATIENT(s, false);
          case PackageNames.ER_PATIENT_CHILD: return PackageInfoFactory.ER_PATIENT(s, true);
          #endregion Emergency

          #region IPD
          case PackageNames.ICU_STAFF: return PackageInfoFactory.ICU(s);
          case PackageNames.OPERATION: return PackageInfoFactory.OPERATION(s);
          case PackageNames.DELIVERY_ROOM: return PackageInfoFactory.DELIVERY_ROOM(s);
          case PackageNames.WARD_ROOMS: return PackageInfoFactory.WARD_ROOMS(s);
          #endregion IPD

          #region OPD
          case PackageNames.OFFICE_SINGLE: return PackageInfoFactory.OFFICE_SINGLE(s);
          case PackageNames.EXAM_SINGLE: return PackageInfoFactory.EXAM_SINGLE(s);
          case PackageNames.OFFICE_MULTI: return PackageInfoFactory.OFFICE_MULTI(s);
          case PackageNames.EXAM_MULTI: return PackageInfoFactory.EXAM_MULTI(s);
          case PackageNames.ENDO: return PackageInfoFactory.ENDO(s);
          #endregion OPD

          case PackageNames.DRUG: return PackageInfoFactory.DRUG(s);
          case PackageNames.PROMOTION: return PackageInfoFactory.PROMOTION(s);
          case PackageNames.PROMOTION_V: return PackageInfoFactory.PROMOTION(s);
        }

        LOG.ec($"{s.PackageName} not supported");
        return PackageInfoFactory.UnknownPackage(s);
      }
      else
      {
        // playlist.xml 자체에 오류가 있는 경우 (이경우 packageName은 "er"이 된다)
        return new PackageInfo(PackageNames.ERROR, s.PackageError, s.PackageError.ToString());
      }
    }
  }
}