using UIControls;
using ServiceCommon;

namespace EUMC.Client
{
  public interface IInformationContent
  {
    INFO_TYPE ID { get; }
    void MessageReceived(ServiceMessage m);
  }

  internal class InfoContentBaseVM : ViewModelBase, IInformationContent
  {
    public INFO_TYPE ID { get; private set; }
    public InfoContentBaseVM(INFO_TYPE type)
    {
      this.ID = type;
    }
    public virtual void MessageReceived(ServiceMessage m)
    {
    }
  }
}