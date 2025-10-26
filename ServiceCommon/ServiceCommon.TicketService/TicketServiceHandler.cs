using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace ServiceCommon.TicketService
{
  public class TicketServiceHandler : IMessageServiceHandler
  {
    Dictionary<int, Division> _divisions = new Dictionary<int, Division>();
    Dictionary<int, DivisionInformation> _infos = new Dictionary<int, DivisionInformation>();

    public List<SERVICE_ID> SupportMessages { get; private set; }
    public List<PACKAGE> SupportPackages => this.PackageMaps.Keys.ToList();
    protected Dictionary<SERVICE_ID, IMessageLoader> MessageMaps = new Dictionary<SERVICE_ID, IMessageLoader>();
    protected Dictionary<PACKAGE, List<SERVICE_ID>> PackageMaps = new Dictionary<PACKAGE, List<SERVICE_ID>>();

    public TicketServiceHandler()
    {
      this.SupportMessages = this.Initialize();

      for (int i = 0; i < 10; i++)
      {
        int divid = i+1;
        int start = i * 100 + 1;
        int end = start + 99;

        var info = new DivisionInformation(divid, start, end, "산부인과", "특수 검사실");
        _infos.Add(divid, info);
        _divisions.Add(divid, new Division(info));
      }
    }
    List<SERVICE_ID> Initialize()
    {
      this.PackageMaps.Add(PACKAGE.TICKET_CALLER, new List<SERVICE_ID> { SERVICE_ID.TICKET_DIV_INFO, SERVICE_ID.TICKET_CALL, SERVICE_ID.TICKET_RECALL });
      this.PackageMaps.Add(PACKAGE.TICKET_DISPLAY, new List<SERVICE_ID> { SERVICE_ID.TICKET_DIV_INFO });
      this.PackageMaps.Add(PACKAGE.TICKET_LARGE, new List<SERVICE_ID> { SERVICE_ID.TICKET_DIV_INFO });
      this.PackageMaps.Add(PACKAGE.TICKET_GEN, new List<SERVICE_ID> { SERVICE_ID.TICKET_DIV_INFO, SERVICE_ID.TICKET_GEN, SERVICE_ID.TICKET_GEN_PT });

      var list = new HashSet<SERVICE_ID>();

      foreach (var ids in this.PackageMaps.Values)
      {
        foreach (var id in ids)
        {
          list.Add(id);
        }
      }
      return list.ToList();
    }

    public bool Start()
    {
      // 초기화가 완료된 후 session을 연다
      return true;
    }

    List<int> get_division_id(IServerSession o)
    {
      //GuardAgainst.Null(o.PackageInfo?.Ticket);
      o.PackageInfo?.Ticket.IsNull();

      return o.PackageInfo.Ticket.DivIds;
    }

    public ServiceMessage RequestService(ServiceMessage m)
    {
      LOG.dc($"{m.ServiceId}");
      switch (m.ServiceId)
      {
        case SERVICE_ID.TICKET_GEN:
          {
            var req = m.CastTo<TICKET_GEN_REQ>();
            return this.GetDivision(req.DivId)?.Ticketing(req) ?? this.Error(m);
          }
        case SERVICE_ID.TICKET_GEN_PT:
          {
            var req = m.CastTo<TICKET_GEN_PT_REQ>();

            var division = this.GetDivision(req.DivId);
            if (division != null)
            {
              // blocking시간을 줄이기 위해서 여기서 조회를 수행한다.
              var info = GetPatientInfo(req.SSN, req.PatientNo);
              return division.TicketingPatient(req, info);
            }
            return this.Error(m);
          }
        case SERVICE_ID.TICKET_CALL:
          {
            var req = m.CastTo<TICKET_CALL_REQ>();
            return this.GetDivision(req.DivId)?.Call(req) ?? this.Error(m);
          }
        case SERVICE_ID.TICKET_RECALL:
          {
            var req = m.CastTo<TICKET_RECALL_REQ>();
            return this.GetDivision(req.DivId)?.Recall(req) ?? this.Error(m);
          }
        case SERVICE_ID.TICKET_DIV_INFO:
          {
            var req = m.CastTo<TICKET_DIV_INFO_REQ>();
            var list = new List<TICKET_DIV_INFO>();
            foreach (var div in req.DivIds)
            {
              var p = this.GetDivision(div)?.GetDivisionInfo();
              if (p != null) { list.Add(p); }
            }
            return new TICKET_DIV_INFO_RESP(list);
          }
      }
      return ServiceMessage.None;
    }
    Division GetDivision(int id)
    {
      _divisions.TryGetValue(id, out var division);
      return division;
    }
    ServiceMessage Error(ServiceMessage m)
    {
      return ServiceMessage.Error(m.ServiceId, MessageErrorCode.Unavailable);
    }
    PatientInfo GetPatientInfo(string ssn, string pt_no)
    {
      return new PatientInfo
      {
        SSN = ssn,
        PatientNo = pt_no,
        PatientName = "name"
      };
    }

    public void Stop()
    {
    }

    void IMessageServiceHandler.Start()
    {
      throw new NotImplementedException();
    }

    public void RequestService(IServerSession session, ServiceMessage request)
    {
      throw new NotImplementedException();
    }

    public bool Subscribe(IServerSession session)
    {
      throw new NotImplementedException();
    }

    public bool Unsubscribe(IServerSession session)
    {
      throw new NotImplementedException();
    }
  }
}