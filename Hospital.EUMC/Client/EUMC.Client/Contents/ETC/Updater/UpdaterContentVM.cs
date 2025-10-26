using Common;
using EUMC.ClientServices;
using ServiceCommon;
using ServiceCommon.ClientServices;

namespace EUMC.Client
{
  internal class UpdaterContentVM : ContentViewModelBase
  {
    public UpdaterInformation ITEM { get; set; }
    public UpdaterContentVM(UpdaterViewConfig config) : base(config)
    {
      this.ITEM = new UpdaterInformation(config);
      this.ContentWidth = ITEM.CONFIG.ContentWidth;
      this.ContentHeight = ITEM.CONFIG.ContentHeight;
    }

    public override bool MessageReceived(ServiceMessage m)
    {
      switch (m.ServiceId)
      {
        case SERVICE_ID.USER_MESSAGE:
          {
            var user = m.CastTo<UserMessage>();
            if (user != null)
            {
              switch (user.Type)
              {
                case USER_MESSAGE_TYPE.DOWNLOAD_PROGRESS:
                  {
                    return this.ITEM.Update(user.CastTo<DownloadProgressMessage>());
                  }
              }
            }
          }
          break;
      }
      return false;
    }

    protected override void ContentClose()
    {
      this.ITEM.Close();
    }
  }
}