using Common;
using ServiceCommon.ServerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ServiceCommon.TicketService
{
  internal class Division
  {
    public int DivId => DivInfo.DivisionID;
    DivisionInformation DivInfo;

    List<IServerSession> _generator = new List<IServerSession>();
    List<IServerSession> _display = new List<IServerSession>();
    List<IServerSession> _caller = new List<IServerSession>();
    List<IServerSession> _large = new List<IServerSession>();

    long _count = 0;
    object LOCK = new object();
    public int WaitCount
    {
      get => (int)Interlocked.Read(ref _count);
    }

    Queue<Ticket> Tickets = new Queue<Ticket>();
    public Division(DivisionInformation info)
    {
      this.DivInfo = info;
#if DEBUG
      for (int i = 0; i < 5; i++)
        this.CreateTicket();
#endif
    }
    public TICKET_GEN_RESP Ticketing(TICKET_GEN_REQ req)
    {
      lock (LOCK)
      {
        int waitno = this.CreateTicket();
        var resp = new TICKET_GEN_RESP(this.DivId, waitno);
        // 호출측에 응답

        // 관련 div에 공유
        var share = new TICKET_DIV_INFO_RESP();

        foreach (var session in _generator)
        {
        }

        //_caller.ForEach(session => session.Send(share));
        //_large.ForEach(session => session.Send(share));
        //_display.ForEach(session => session.Send(share));
        return resp;
      }
    }
    public TICKET_GEN_PT_RESP TicketingPatient(TICKET_GEN_PT_REQ req, PatientInfo info)
    {
      lock (LOCK)
      {
        int waitno = this.CreateTicket();
        var resp = new TICKET_GEN_PT_RESP()
        {
          DivId = this.DivId,
          WaitNo = waitno,
          SSN = info.SSN,
          PatientName = info.PatientName,
          PatientNo = info.PatientNo,
        };

        var share = new TICKET_DIV_INFO_RESP();

        foreach (var session in _generator)
        {
          // 호출측에는 공유하지 않음
        }

        //_caller.ForEach(session => session.Send(msg));
        //_large.ForEach(session => session.Send(msg));
        //_display.ForEach(session => session.Send(msg));
        return resp;
      }
    }

    internal ServiceMessage Call(TICKET_CALL_REQ req)
    {
      lock (LOCK)
      {
        int wndno = req.WndNo;
        int waitno = this.GetTicket();
        var msg = new TICKET_CALL_RESP(this.DivId, wndno, waitno);

        _generator.ForEach(session => session.Send(msg));

        foreach (var session in _caller)
        {
          // 호출측에는 공유하지 않음
        }
        _caller.ForEach(session => session.Send(msg));
        _display.ForEach(session => session.Send(msg));
        _large.ForEach(session => session.Send(msg));
        return msg;
      }
    }
    internal ServiceMessage Recall(TICKET_RECALL_REQ p)
    {
      lock (LOCK)
      {
        var msg = new TICKET_CALL_RESP(this.DivId, p.WndNo, p.WaitNo);
        _display.ForEach(session => session.Send(msg));
        _large.ForEach(session => session.Send(msg));
        return msg;
      }
    }
    internal TICKET_DIV_INFO GetDivisionInfo()
    {
      lock (LOCK)
      {
        return new TICKET_DIV_INFO
        {
          DivId = this.DivId,
          WaitNos = this.Tickets.Select(x => x.WaitNo).ToList()
        };
      }
    }
    public void AddSession(IServerSession o)
    {
      lock (LOCK)
      {
        switch (o.Package)
        {
          case PACKAGE.TICKET_GEN:
            {
              _generator.Add(o);
              o.Send(new TICKET_DIV_INFO_RESP(this.GetDivisionInfo()));
            }
            break;
          case PACKAGE.TICKET_DISPLAY:
            {
              _display.Add(o);
              o.Send(new TICKET_DIV_INFO_RESP(this.GetDivisionInfo()));
            }
            break;
          case PACKAGE.TICKET_CALLER:
            {
              _caller.Add(o);
              o.Send(new TICKET_DIV_INFO_RESP(this.GetDivisionInfo()));
            }
            break;
          case PACKAGE.TICKET_LARGE:
            {
              _large.Add(o);
              o.Send(new TICKET_DIV_INFO_RESP(this.GetDivisionInfo()));
            }
            break;
          default:
            LOG.wc($"{this.DivId} {o.Package}, ClientId: {o.SessionId} not supported");
            break;
        }
      }
    }
    public void RemoveSession(IServerSession o)
    {
      lock (LOCK)
      {
        switch (o.Package)
        {
          case PACKAGE.TICKET_GEN: _generator.Remove(o); break;
          case PACKAGE.TICKET_DISPLAY: _display.Remove(o); break;
          case PACKAGE.TICKET_CALLER: _caller.Remove(o); break;
          case PACKAGE.TICKET_LARGE: _large.Remove(o); break;
          default:
            LOG.wc($"{this.DivId} {o.Package}, ClientId: {o.SessionId} not supported");
            break;
        }
      }
    }
    int CreateTicket()
    {
      try
      {
        int waitno = this.DivInfo.GetNextNo();
        var ticket = new Ticket(this.DivId, waitno);
        this.Tickets.Enqueue(ticket);
        return waitno;
      }
      finally
      {
        Interlocked.Increment(ref _count);
      }
    }
    int GetTicket()
    {
      int waitno = 0;
      if (this.Tickets.Any())
      {
        waitno = this.Tickets.Dequeue().WaitNo;
        Interlocked.Decrement(ref _count);
      }
      return waitno;
    }
  }

  internal class Ticket
  {
    public int DivId;
    public int WaitNo;
    public DateTime Time = DateTime.Now;

    public Ticket(int div, int no)
    {
      this.DivId = div;
      this.WaitNo = no;
    }
  }
}