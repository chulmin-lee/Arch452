using ServiceCommon;
using ServiceCommon.ServerServices;
using EUMC.HospitalService;

namespace EUMC.ServerServices
{
  public partial class ServerService : IServerService
  {
    void init_message_handler()
    {
      this.MessageHandlers.Add(this.init_hospital_service());

    }
    IMessageServiceHandler init_hospital_service()
    {
      var hospital = new HospitalMessageServiceHandler(new HospitalMessageGenerator());
      foreach (var message in hospital.SupportMessages)
      {
        if (this.MessageMap.ContainsKey(message))
        {
          throw new ServiceException($"{message} already registered");
        }
        this.MessageMap.Add(message, hospital);
      }

      foreach (var package in hospital.SupportPackages)
      {
        if (this.PackageMap.ContainsKey(package))
        {
          throw new ServiceException($"{package} already registered");
        }
        this.PackageMap.Add(package, hospital);
      }

      return hospital;
    }

    UserServiceHandler init_user_service()
    {
      var user = new UserServiceHandler(SessionHandler);

      foreach (var message in user.SupportMessages)
      {
        if (this.MessageMap.ContainsKey(message))
        {
          throw new ServiceException($"{message} already registered");
        }
        this.MessageMap.Add(message, user);
      }
      return user;
    }
  }
}