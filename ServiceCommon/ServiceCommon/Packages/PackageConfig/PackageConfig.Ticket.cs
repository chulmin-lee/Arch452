using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon
{
  public class TicketPackage
  {
    public TicketPackage() { }
    public TicketPackage(List<TICKET_DIVISION> divs)
    {
      this.Divisions.AddRange(divs);
      this.DivIds = divs.Select(x => x.DivId).ToList();
      this.Windows.AddRange(divs.SelectMany(x => x.Windows));
      this.WndNos = this.Windows.Select(x => x.WndNo).ToList();
    }
    public TicketPackage(List<int> divids)
    {
      this.DivIds.AddRange(divids);
    }
    public TicketPackage(List<int> divids, List<int> wndnos)
    {
      this.DivIds.AddRange(divids);
      this.WndNos.AddRange(wndnos);
    }
    public List<int> DivIds { get; set; } = new List<int>();
    public List<int> WndNos { get; set; } = new List<int>();
    public List<TICKET_DIVISION> Divisions { get; set; } = new List<TICKET_DIVISION>();
    public List<TICKET_WINDOW> Windows { get; set; } = new List<TICKET_WINDOW>();
  }
}