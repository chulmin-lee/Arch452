using ServiceCommon.ServerServices;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCommon.HospitalService
{
  public class GroupingDataSession<T, K> : SessionMember
    where T : class, IGroupKeyData<K>
  {
    public List<K> Keys { get; private set; } = new List<K>();
    public GroupingDataSession(IServerSession s, K key) : base(s)
    {
      this.Keys.Add(key);
    }
    public GroupingDataSession(IServerSession s, List<K> keys) : base(s)
    {
      this.Keys.AddRange(keys);
    }
    public List<T> SendOrNot(List<T> changed)
    {
      return changed.Where(x => this.Keys.Contains(x.GroupKey)).ToList();
    }
  }

  public class GroupingDataCollection<T, K>
    where T : class, IGroupKeyData<K>
    //where K : notnull
  {
    protected Dictionary<K, T> Items = new Dictionary<K, T>();

    public void Update(List<T> list)
    {
      list.ForEach(x => this.Items[x.GroupKey] = x);
    }
    public T Select(K key)
    {
      return this.Items.ContainsKey(key) ? this.Items[key] : null;
    }
    public List<T> Select(List<K> keys)
    {
      return this.Items.Where(x => keys.Contains(x.Key)).Select(x => x.Value).ToList();
    }
    public List<T> SelectAll() => this.Items.Values.ToList();
  }
}