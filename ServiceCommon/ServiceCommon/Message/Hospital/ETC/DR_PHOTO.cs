using System.Collections.Generic;

namespace ServiceCommon
{
  public class DR_PHOTO_REQ : ServiceMessage
  {
    public string DoctorNo { get; set; } = string.Empty;
    public DR_PHOTO_REQ() : base(SERVICE_ID.DR_PHOTO) { }
    public DR_PHOTO_REQ(string d) : this()
    {
      this.DoctorNo = d;
    }
  }

  public class DR_PHOTO_RESP : ServiceMessage
  {
    public DR_PHOTO_INFO Photo { get; set; } = new DR_PHOTO_INFO();
    public DR_PHOTO_RESP() : base(SERVICE_ID.DR_PHOTO) { }
    public DR_PHOTO_RESP(DR_PHOTO_INFO p) : this()
    {
      this.Photo = p;
    }
  }

  public class DR_PHOTO_INFO
  {
    public string DoctorNo { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
  }
}