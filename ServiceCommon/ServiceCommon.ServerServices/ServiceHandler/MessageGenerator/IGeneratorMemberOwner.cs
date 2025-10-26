namespace ServiceCommon.ServerServices
{
  /// <summary>
  /// TransformerHandler가 transformer, extractor를 관리하기 위해서 가져야할 인터페이스
  /// 이것을 상속받은 클래스에 Repository가 있다
  /// 아니면 클래스 생성자로 필요한 정보를 전달하면 된다.
  /// 즉 owner는 받되 직접 사용하지 않으면 된다.
  /// </summary>
  /// <typeparam name="D"></typeparam>

  public interface IGeneratorMemberOwner
  {
    string ServiceName { get; }
    string ServiceDir { get; }
    int ScheduleInterval { get; }
    bool IsBackup { get; }
    string BackupDataPath { get; }
    string SimDataPath { get; }
  }
}