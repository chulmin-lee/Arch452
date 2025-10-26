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
    public string DoctorNo { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public DR_PHOTO_RESP() : base(SERVICE_ID.DR_PHOTO) { }
    public DR_PHOTO_RESP(string dr_no, string photo) : this()
    {
      this.DoctorNo = dr_no;
      this.Photo = photo;
    }
  }

  public class DR_PHOTO_UPDATED : ServiceMessage
  {
    public List<string> DeletedDoctors { get; set; } = new List<string>();
    public List<string> UpdatedDoctors { get; set; } = new List<string>(); // add + update

    public DR_PHOTO_UPDATED() : base(SERVICE_ID.DR_PHOTO_NOTI) { }
    public DR_PHOTO_UPDATED(List<string> deleted, List<string> updated) : this()
    {
      this.DeletedDoctors = deleted;
      this.UpdatedDoctors = updated;
    }
  }
}