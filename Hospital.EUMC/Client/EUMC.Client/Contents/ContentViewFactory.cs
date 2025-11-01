using Common;
using EUMC.ClientServices;
using ServiceCommon;
using ServiceCommon.ClientServices;
using System;

namespace EUMC.Client
{
  internal class ContentViewFactory
  {
    public static IContentViewModel Create(IPackageViewConfig config)
    {
      LOG.ic($"{config.Package}");

      switch (config.Package)
      {
        case PACKAGE.OFFICE_SINGLE:
          {
            var o = config.CastTo<OfficeSingleViewConfig>();
            return new OfficeSingleContentVM(o);
          }
        //case PACKAGE.OFFICE_MULTI:
        //  {
        //    var o = config.CastTo<OfficeMultiViewConfig>();
        //    return new OfficeMultiContentVM(o);
        //  }
        //case PACKAGE.EXAM_SINGLE:
        //  {
        //    var o = config.CastTo<ExamSingleViewConfig>();
        //    return new ExamSingleContentVM(o);
        //  }

          /*
        case PACKAGE.ER_PATIENT:
          return new ErPatientContentVM(config.CastTo<ErPatientViewConfig>());
        case PACKAGE.ICU:
          {
            var o = config.CastTo<IcuViewConfig>();
            if (o.Config.IsStaff)
            {
              return new IcuStaffContentVM(o);
            }
            else
            {
              if (o.Config.IsBaby) return new IcuBabyContentVM(o);
              else return new IcuGuardContentVM(o);
            }
          }
        case PACKAGE.DELIVERY_ROOM:
          {
            var o = config.CastTo<DeliveryRoomViewConfig>();
            return new DeliveryContentVM(o);
          }


        case PACKAGE.EXAM_MULTI:
          {
            var o = config.CastTo<ExamMultiViewConfig>();
            return new ExamMultiContentVM(o);
          }
        case PACKAGE.DRUG:
          return new DrugContentVM(config.CastTo<DrugViewConfig>());
        case PACKAGE.OPERATION: return new OperationContentVM(config.CastTo<OperationViewConfig>());

        case PACKAGE.ENDO:
          return new EndoContentVM(config.CastTo<EndoViewConfig>());
        case PACKAGE.WARD_ROOMS:
          return new WardRoomContentVM(config.CastTo<WardRoomViewConfig>());
        //-------------------------------
        // ETC
        //-------------------------------
        case PACKAGE.PROMOTION:
          return new PromotionContentVM(config.CastTo<PromotionViewConfig>());
          */
        //-------------------------------
        // 내부용
        //-------------------------------
        case PACKAGE.UPDATER:
          return new UpdaterContentVM(config.CastTo<UpdaterViewConfig>());
        case PACKAGE.NO_SCHEDULE:
          return new NoScheduleContentVM(config.CastTo<NoScheduleViewConfig>());
        case PACKAGE.ERROR_PACKAGE:
          return new ErrorContentVM(config.CastTo<ErrorViewConfig>());
        //case PACKAGE.INFORMATION:
        //  return new InfomationMainVM(config);
          
      }

      LOG.ec($"{config.Package} not supported");
      throw new NotImplementedException();
    }
  }
}