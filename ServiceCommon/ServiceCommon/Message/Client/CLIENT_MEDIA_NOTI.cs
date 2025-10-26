namespace ServiceCommon
{
  public class CLIENT_MEDIA_NOTI : ServiceMessage
  {
    public int ClientId { get; set; }
    public int MediaId { get; set; }
    public CLIENT_MEDIA_NOTI() : base(SERVICE_ID.CLIENT_MEDIA_NOTI)
    {
    }
    public CLIENT_MEDIA_NOTI(int client_id, int mediaId) : this()
    {
      ClientId = client_id;
      MediaId = mediaId;
    }
  }
}