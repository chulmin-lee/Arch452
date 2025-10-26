/*
using Common;
using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.ClientServices;
internal class SHD_GrpcSSClientSessionHandler : GrpcSSClientSessionHandler
{
  public SHD_GrpcSSClientSessionHandler(IClientService owner, string address) : base(owner, address)
  {
  }

  protected override void RequestInitData()
  {
    if (this.PackageInfo == null)
    {
      LOG.ec("PackageInfo is null");
      return;
    }

    var d = this.PackageInfo;

    switch (d.Package)
    {
      //========================================
      // Emergency
      //========================================
      case PACKAGE.ER_PATIENT:
        {
          GuardAgainst.Null(d.Emergency);
          request_data(new ER_PATIENT_REQ(d.Emergency.IsChild));
          request_data(new ER_STATISTICS_REQ(d.Emergency.IsChild));
          request_data(new ER_CONGESTION_REQ(d.Emergency.IsChild));
        }
        break;
      //========================================
      // IPD
      //========================================
      case PACKAGE.DELIVERY_ROOM: request_data(new DELIVERY_ROOM_REQ()); break;
      case PACKAGE.ICU:
        {
          GuardAgainst.Null(d.Icu);
          request_data(new ICU_REQ(d.Icu.WardCodes));
        }
        break;
      case PACKAGE.OPERATION: request_data(new OPERATION_REQ()); break;
      case PACKAGE.WARD_ROOMS:
        {
          GuardAgainst.Null(d.WardRoom);
          request_data(new WARD_ROOM_REQ(d.WardRoom.Floor, d.WardRoom.AreaCode));
          break;
        }
      //========================================
      // OPD
      //========================================
      case PACKAGE.OFFICE_SINGLE:
        {
          GuardAgainst.Null(d.OpdRoom);
          var x = d.OpdRoom.DeptRooms.First();
          request_data(new OFFICE_REQ(x.DeptCode, x.RoomCode));
        }
        break;
      case PACKAGE.OFFICE_MULTI:
        {
          GuardAgainst.Null(d.OpdRoom);
          foreach (var x in d.OpdRoom.DeptRooms)
          {
            request_data(new OFFICE_REQ(x.DeptCode, x.RoomCodes));
          }
        }
        break;

      case PACKAGE.EXAM_SINGLE:
        {
          GuardAgainst.Null(d.OpdRoom);
          var x = d.OpdRoom.DeptRooms.First();
          request_data(new EXAM_REQ(x.DeptCode, x.RoomCode));
        }
        break;
      case PACKAGE.EXAM_MULTI:
      case PACKAGE.EXAM_MULTI_PET:
        {
          GuardAgainst.Null(d.OpdRoom);
          foreach (var x in d.OpdRoom.DeptRooms)
          {
            request_data(new EXAM_REQ(x.DeptCode, x.RoomCodes));
          }
        }
        break;

      case PACKAGE.EXAM_OFFICE_MIX:
        {
          GuardAgainst.Null(d.OpdRoom);

          var offices = d.OpdRoom.DeptRooms.Where(x => x.RoomType == "A");
          var exams = d.OpdRoom.DeptRooms.Where(x => x.RoomType == "A");

          foreach (var x in offices)
          {
            request_data(new OFFICE_REQ(x.DeptCode, x.RoomCodes));
          }
          foreach (var x in exams)
          {
            request_data(new EXAM_REQ(x.DeptCode, x.RoomCodes));
          }
        }
        break;

      case PACKAGE.INSPECTION:
        {
          GuardAgainst.Null(d.Inspection);
          request_data(new INSPECTION_REQ(d.Inspection.Type));
        }
        break;
      case PACKAGE.ENDO: request_data(new ENDO_REQ()); break;
      //========================================
      // ETC
      //========================================
      case PACKAGE.DRUG: request_data(new DRUG_REQ()); break;
      case PACKAGE.DR_SCH: request_data(new DR_SCH_REQ()); break;
      //========================================
      // Funeral
      //========================================
      case PACKAGE.FUNERAL_SINGLE:
        {
          GuardAgainst.Null(d.Funeral);
          request_data(new FUNERAL_REQ(d.Funeral.RoomCodes.First()));
        }
        break;
      case PACKAGE.FUNERAL_MULTI:
        {
          GuardAgainst.Null(d.Funeral);
          request_data(new FUNERAL_REQ(d.Funeral.RoomCodes));
        }
        break;
      //========================================
      // Ticket
      //========================================
      case PACKAGE.TICKET_GEN:
        {
          GuardAgainst.Null(d.Ticket);
          request_data(new TICKET_DIV_INFO_REQ(d.Ticket.DivIds));
        }
        break;
      case PACKAGE.TICKET_DISPLAY:
        {
          GuardAgainst.Null(d.Ticket);
          request_data(new TICKET_DIV_INFO_REQ(d.Ticket.DivIds));
        }
        break;

      case PACKAGE.TICKET_CALLER:
        {
          GuardAgainst.Null(d.Ticket);
          request_data(new TICKET_DIV_INFO_REQ(d.Ticket.DivIds));
        }
        break;
      case PACKAGE.TICKET_LARGE:
        {
          GuardAgainst.Null(d.Ticket);
          request_data(new TICKET_DIV_INFO_REQ(d.Ticket.DivIds));
        }
        break;

      default:
        LOG.wc($"{d.Package} not supported");
        break;
    }

    void request_data(ServiceMessage m)
    {
      LOG.dc($"send firt data request: {m.ServiceId}");
      this.Send(m);
    }
  }
}
*/