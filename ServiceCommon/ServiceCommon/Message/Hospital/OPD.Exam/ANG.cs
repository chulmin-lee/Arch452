using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCommon
{
  public class ANG_REQ : ServiceMessage
  {
    public ANG_TYPE Type { get; set; }
    public ANG_REQ() : base(SERVICE_ID.ANG) { }
    public ANG_REQ(ANG_TYPE type) : this()
    {
      this.Type = type;
    }
  }
  public class ANG_RESP : ServiceMessage
  {
    public List<ANG_PT_INFO> Patients { get; set; } = new List<ANG_PT_INFO>();
    public ANG_RESP() : base(SERVICE_ID.ANG) { }
    public ANG_RESP(ANG_PT_INFO p) : this()
    {
      this.Patients.Add(p);
    }
    public ANG_RESP(List<ANG_PT_INFO> p) : this()
    {
      this.Patients = p;
    }
  }

  /// <summary>
  /// 조영실(Angiography)
  /// </summary>
  public class ANG_PT_INFO : IGroupKeyData<ANG_TYPE>
  {
    // 1: 혈관조영실
    // 2: 혈관조영실 3층
    // 3: 심뇌혈관 조영실
    public ANG_TYPE Type { get; set; }
    public string DeptCode { get; set; }
    public string DeptName { get; set; }
    public string PatientNo { get; set; }
    public string PatientName { get; set; }
    public string OperationName { get; set; }     // 시술명
    public string OperationRoom { get; set; }     // 장소
    public ANG_STATE StateCode { get; set; }
    public ANG_TYPE GroupKey => this.Type;
  }
  public enum ANG_TYPE
  {
    None,
    Angiography = 1,
    Angiography_3F = 2,
    IMC = 3, // 심뇌혈관 조영술
  }
  public enum ANG_STATE
  {
    Init            = 0, // 0 - 초기
    Preparing       = 1, // 1 - 준비중
    Prepared        = 2, // 2 - 준비완료
    CallPatient     = 3, // 3 - 환자호출
    Scheduled       = 4, // 4 - 시술예정
    During          = 5, // 5 - 시술중
    Completed       = 6, // 6 - 시술종료
    Cancelled       = 7, // 7 - 시술취소
    WardPreparation = 8, // 8 - 병동준비
    ScheduledToEnd  = 9, // 9 - 종료예정
  }

}
