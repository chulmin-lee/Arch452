using Common;
using ServiceCommon.ServerServices;
//using UIControls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  public class DR_PHOTO_Loader : MessageLoaderBase
  {
    Dictionary<int, PhotoSession> Sessions = new Dictionary<int, PhotoSession>();
    // dr_no, photo_path
    Dictionary<string, PhotoUpdateInfo> Items = new Dictionary<string, PhotoUpdateInfo>();

    public DR_PHOTO_Loader() : base(SERVICE_ID.DR_PHOTO)
    {
    }

    protected override void message_notified(INotifyMessage m)
    {
      var data = m as NotifyMessage<PhotoUpdateInfo>;
      if (data == null) throw new Exception($"{m.ID} not supported");

      foreach (var p in data.Updated)
      {
        this.Items[p.DoctorNo] = p;
        var session = this.Sessions.Values.Where(x => x.RoomKey == p.RoomKey).FirstOrDefault();

        if (session != null)
        {
          //session.Send(new DR_PHOTO_RESP(p.DoctorNo, ImageLoader.GetDoctorPhoto(p.FilePath, 500, 1000, 1000000, false)));
        }
      }
    }
    protected override ServiceMessage request_service(ServiceMessage m)
    {
      var req = m.CastTo<DR_PHOTO_REQ>();

      string photo = string.Empty;
      if (this.Items.TryGetValue(req.DoctorNo, out var value))
      {
        //photo = ImageLoader.GetDoctorPhoto(value.FilePath, 500, 1000, 1000000, false);
      }
      return new DR_PHOTO_RESP(req.DoctorNo, photo);
    }
    protected override void subscribe_session(IServerSession s)
    {
      var room = s.PackageInfo?.OpdRoom?.DeptRooms.First() ?? throw new Exception("no single");
      var session = new PhotoSession(s, room.DeptCode, room.RoomCode);
      this.Sessions.Add(session.ID, session);
    }

    protected override void unsubscribe_session(IServerSession s)
    {
      this.Sessions.Remove(s.Key);
    }
  }

  public class PhotoSession : SessionMember
  {
    public string DeptCode { get; set; }
    public string RoomCode { get; set; }
    public string RoomKey { get; set; }
    public PhotoSession(IServerSession s, string deptCode, string roomCode) : base(s)
    {
      this.DeptCode = deptCode;
      this.RoomCode = roomCode;
      this.RoomKey = $"{deptCode}:{roomCode}";
    }
  }

  public class PhotoUpdateInfo
  {
    public string DoctorNo { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string RoomKey { get; set; } = string.Empty;
  }
}