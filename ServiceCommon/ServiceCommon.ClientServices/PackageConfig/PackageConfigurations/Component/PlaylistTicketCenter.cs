using System.Collections.Generic;

namespace ServiceCommon.ClientServices
{
  public class PlaylistTicketCenter
  {
    public TicketClientConfig Generator { get; private set; }
    public TicketClientConfig Display { get; private set; }
    public TicketClientConfig Caller { get; private set; }
    public TicketClientConfig Large { get; private set; }
    public PlaylistTicketCenter() { }
    //public PlaylistTicketCenter(xml_ticket_center o)
    //{
    //  this.Generator = o.generator != null ? new TicketClientConfig(o.generator) : null;
    //  this.Display = o.dislay != null ? new TicketClientConfig(o.dislay) : null;
    //  this.Caller = o.caller != null ? new TicketClientConfig(o.caller) : null;
    //  this.Large = o.large != null ? new TicketClientConfig(o.large) : null;
    //}

    public class TicketClientConfig
    {
      public List<TicketDivision> Divisions { get; set; } = new List<TicketDivision>();
      /*
      public TicketClientConfig(xml_ticket_center.ticket_client config)
      {
        foreach (var div in config.divisions)
        {
          if (int.TryParse(div.div_id, out int div_id))
          {
            if (div_id == 0)
            {
              LOG.e($"divid == 0");
              continue;
            }
          }
          else
          {
            LOG.e($"divid: {div.div_id}");
            continue;
          }

          var division = new TicketDivision(div_id, div.div_nm, div.div_desc);

          foreach (var p in div.windows)
          {
            if (int.TryParse(p.wnd_no, out int wndno))
            {
              if (wndno > 0)
              {
                division.AddWindow(div_id, wndno, p.wnd_nm, p.position);
              }
              else
              {
                LOG.e($"wndno == 0");
              }
            }
            else
            {
              LOG.e($"wnd: wnd_no error : {p.wnd_no}");
            }
          }

          this.Divisions.Add(division);
        }
      } */
    }
    public class TicketDivision
    {
      public int DivId { get; set; }
      public string DivName { get; set; } = string.Empty;
      public string Desc { get; set; } = string.Empty;

      public List<TicketWindow> Windows { get; set; } = new List<TicketWindow>();
      public TicketDivision(int id, string name, string desc)
      {
        this.DivId = id;
        this.DivName = name;
        this.Desc = desc;
      }
      public void AddWindow(int div, int wndno, string name, string pos)
      {
        this.Windows.Add(new TicketWindow(div, wndno, name, pos));
      }
    }
    public class TicketWindow
    {
      public int DivId { get; set; }
      public int WndNo { get; set; }
      public string WndName { get; set; } = string.Empty;
      public string Position { get; set; } = string.Empty;
      public TicketWindow(int div, int wndno, string name, string pos)
      {
        this.DivId = div;
        this.WndNo = wndno;
        this.WndName = name;
        this.Position = pos;
      }
      public bool IsLeft => this.Position.ToUpper() == "L";
      public bool IsRight => this.Position.ToUpper() == "R";
    }
  }
}