using System.Collections.Generic;

namespace ServiceCommon
{
  public class TICKET_DIVISION
  {
    public int DivId;
    public string DivName= string.Empty;
    public string DivDesc = string.Empty;
    public List<TICKET_WINDOW> Windows = new List<TICKET_WINDOW>();
    public TICKET_DIVISION(int div, string divnm, string divDesc)
    {
      this.DivId = div;
      this.DivName = divnm;
      this.DivDesc = divDesc;
    }
    public void AddWindow(TICKET_WINDOW wnd)
    {
      this.Windows.Add(wnd);
    }
  }
  public class TICKET_WINDOW
  {
    public int DivId;
    public int WndNo;
    public string WndName= string.Empty;
    public string Position = string.Empty;
    public TICKET_WINDOW(int div, int wndno, string wndnm, string pos)
    {
      this.DivId = div;
      this.WndNo = wndno;
      this.WndName = wndnm;
      this.Position = pos;
    }
  }
}