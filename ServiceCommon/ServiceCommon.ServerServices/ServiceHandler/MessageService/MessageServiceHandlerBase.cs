using Common;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.ServerServices
{
  public abstract class MessageServiceHandlerBase : IMessageServiceHandler
  {
    public List<SERVICE_ID> SupportMessages { get; private set; } = new List<SERVICE_ID>();
    public List<PACKAGE> SupportPackages => this.PackageMaps.Keys.ToList();
    protected Dictionary<SERVICE_ID, IMessageLoader> MessageMaps = new Dictionary<SERVICE_ID, IMessageLoader>();
    protected Dictionary<PACKAGE, List<SERVICE_ID>> PackageMaps = new Dictionary<PACKAGE, List<SERVICE_ID>>();
    protected IMessageGenerator Broker;

    public MessageServiceHandlerBase(IMessageGenerator impl)
    {
      this.Broker = impl;
      this.Initialize();
    }
    protected abstract Dictionary<PACKAGE, List<SERVICE_ID>> InitalizePackageMapImpl();
    protected abstract IMessageLoader CreateMessageLoaderImpl(SERVICE_ID id);
    void Initialize()
    {
      this.PackageMaps = this.InitalizePackageMapImpl();

      foreach (var ids in this.PackageMaps.Values)
      {
        foreach (var id in ids)
        {
          if (!SupportMessages.Contains(id))
          {
            this.SupportMessages.Add(id);

            var loader = this.CreateMessageLoaderImpl(id);
            if (loader != null)
            {
              LOG.wc($"LoaderCreate: {loader.ID}");
              if (this.Broker.MessageSubscribe(loader))
              {
                this.MessageMaps.Add(loader.ID, loader);
              }
              else
              {
                throw new ServiceException($"subscribe loader {id} failed");
              }
            }
            else
            {
              throw new ServiceException($"create loader [{id}] failed");
            }
          }
        }
      }
    }
    public void Start() => this.Broker.Start();
    public void Stop() => this.Broker.Stop();
    public bool Subscribe(IServerSession session)
    {
      if (session.PackageInfo != null)
      {
        var package = session.PackageInfo.Package;

        if (this.PackageMaps.ContainsKey(package))
        {
          bool registered = false;
          var ids = this.PackageMaps[package];
          foreach (var id in ids)
          {
            if (this.MessageMaps.TryGetValue(id, out var loader))
            {
              loader.Subscribe(session);
              registered = true;
            }
            else
            {
              LOG.ec($"{id} not supported");
              return false;
            }
          }
          return registered;
        }
      }
      return false;
    }

    public bool Unsubscribe(IServerSession session)
    {
      if (session.PackageInfo != null)
      {
        var package = session.PackageInfo.Package;
        if (this.PackageMaps.ContainsKey(package))
        {
          var ids = this.PackageMaps[package];
          foreach (var id in ids)
          {
            if (this.MessageMaps.TryGetValue(id, out var loader))
            {
              loader.Unsubscribe(session);
              return true;
            }
            else
            {
              LOG.ec($"{id} not supported");
              return false;
            }
          }
        }
      }
      return false;
    }

    public ServiceMessage RequestService(ServiceMessage m)
    {
      if (this.MessageMaps.TryGetValue(m.ServiceId, out var loader))
      {
        return loader.RequestService(m);
      }
      return ServiceMessage.None;
    }

    public void RequestService(IServerSession session, ServiceMessage m)
    {
      var msg = this.RequestService(m);
      if (msg != null)
      {
        if (msg.ServiceId != SERVICE_ID.NONE)
          session.Send(msg);
      }
    }
  }
}