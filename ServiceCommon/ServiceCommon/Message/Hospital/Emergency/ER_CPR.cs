using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCommon
{
  public class ER_CPR_REQ : ServiceMessage
  {
    public ER_CPR_REQ():base(SERVICE_ID.ER_CPR) { }
  }
  public class ER_CPR_RESP : ServiceMessage
  {
    public bool CPR { get; set; }
    public ER_CPR_RESP() : base(SERVICE_ID.ER_CPR) { }
    public ER_CPR_RESP(bool on) : this()
    {
      this.CPR = on;
    }
  }
}
