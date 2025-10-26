using AutoMapper;
using ServiceCommon;
using ServiceCommon.ServerServices;

namespace EUMC.HospitalService
{
  internal abstract class MessageTransformer : MessageTransformerBase<DATA_ID>
  {
    protected IMapper Mapper { get; private set; }
    protected IHospitalMemberOwner Owner { get; private set; }

    public MessageTransformer(IHospitalMemberOwner owner, SERVICE_ID id) : base(id)
    {
      this.Owner = owner;
      this.Mapper = DataMapper.Mapper;
    }
  }
}