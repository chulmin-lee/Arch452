using ServiceCommon;
using ServiceCommon.ServerServices;
using System.IO;

namespace EUMC.HospitalService
{
  internal interface IHospitalMemberOwner : IGeneratorMemberOwner
  {
    IHospitalRepository Repository { get; }
  }

  public class HospitalMessageGenerator : MessageGeneratorBase<DATA_ID>, IHospitalMemberOwner
  {
    public IHospitalRepository Repository { get; private set; }
    HospitalServiceConfigurations _config;

    public HospitalMessageGenerator() : base("EUMC")
    {
      this.Repository = new HospitalRepositorySim(this.SimDataPath);
      var config_path = Path.Combine(this.ServiceDir, $"{this.ServiceName}_HospitalService.Config");
      _config = HospitalServiceConfigurations.Load(config_path);
      this.ScheduleInterval = _config.DataExtractor.ScheduleInterval;
      this.IsBackup = _config.DataExtractor.IsBackup;
    }

    protected override IDataExtractor<DATA_ID> CreateExtractor(DATA_ID id)
    {
      return DataExtractorFactory.Create(this, id, _config.DataExtractor);
    }
    /// <summary>
    /// Transformer를 가져온다. 만약 존재하지 않는 경우 생성한다.
    /// </summary>
    /// <param name="id">가져올 transformer ID</param>
    /// <param name="created">transformer를 새로 생성한 경우</param>
    /// <returns></returns>
    protected override IMessageTransformer<DATA_ID> GetTransformer(SERVICE_ID id, out bool created)
    {
      created = false;
      if (!this.Transformers.ContainsKey(id))
      {
        created = true;
        return MessageTransformerFactory.Create(this, id, _config.MessageTransformer);
      }
      return this.Transformers[id];
    }
  }
}