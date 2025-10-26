using System;
using System.Collections.Generic;

namespace ServiceCommon
{
  public class CAFETERIA_REQ : ServiceMessage
  {
    public CAFETERIA_REQ() : base(SERVICE_ID.CAFETERIA) { }
  }

  public class CAFETERIA_RESP : ServiceMessage
  {
    public List<MENU> Menus { get; set; } = new List<MENU>();
    public string FoodOrigin { get; set; } = string.Empty;
    public CAFETERIA_RESP() : base(SERVICE_ID.CAFETERIA) { }
    public class MENU
    {
      public DayOfWeek DayOfWeek { get; set; }
      public string Breakfast { get; set; } = string.Empty;
      public string Lunch { get; set; } = string.Empty;
      public string Dinner { get; set; } = string.Empty;
    }
  }
}