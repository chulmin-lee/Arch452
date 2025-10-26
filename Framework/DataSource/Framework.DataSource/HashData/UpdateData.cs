using System.Collections.Generic;

namespace Framework.DataSource
{
  /// <summary>
  /// DTO 변경 여부 데이타
  /// </summary>
  /// <typeparam name="T"></typeparam>

  public class UpdateData<T>
  {
    public List<T> Constant = new List<T>();   // 변경되지 않은 데이터
    public List<T> Updated = new List<T>();     // 변경된 데이터
    public List<T> Deleted = new List<T>();     // 삭제된 데이터
    public List<T> Added = new List<T>();       // 새로 추가된 데이터

    //
    public List<T> All = new List<T>();         // 현재 데이타 (NotChanged + Changed + Added)
    public List<T> ChangedAll = new List<T>();  // 변경된 모든 데이타 (updated/added/deleted)
    public List<T> AddedAndUpdated = new List<T>();  // 변경된 데이타 (updated/added)
    public int Count => All.Count;
    /// <summary>
    /// 이전 데이터와 비교해서 변경된 갯수
    /// </summary>
    public int ChangeCount => Updated.Count + Deleted.Count + Added.Count;
    public bool IsChanged => this.ChangeCount > 0;
    public override string ToString()
    {
      return $"[{typeof(T).Name}] Added:{Added.Count}, Deleted:{Deleted.Count}, Updated:{Updated.Count}";
    }
    public bool IsValid()
    {
      return All.Count == Constant.Count + Updated.Count + Added.Count;
    }
    /// <summary>
    /// All 을 만든다
    /// </summary>
    public UpdateData<T> Compose()
    {
      this.All.Clear();
      this.All.AddRange(this.Constant);
      this.All.AddRange(this.Updated);
      this.All.AddRange(this.Added);

      this.ChangedAll.Clear();
      this.ChangedAll.AddRange(this.Deleted);
      this.ChangedAll.AddRange(this.Updated);
      this.ChangedAll.AddRange(this.Added);

      this.AddedAndUpdated.Clear();
      this.AddedAndUpdated.AddRange(this.Updated);
      this.AddedAndUpdated.AddRange(this.Added);
      return this;
    }
  }
}