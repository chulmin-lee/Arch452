using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UIControls
{

  /// <summary>
  /// Function 집합. Group 정보도 포함
  /// Band 와는 무관하다.
  /// band - 물리적 집합
  /// group - 논리적 집합
  /// </summary>
  /// <typeparam name="F">function id</typeparam>
  /// <typeparam name="G">group id</typeparam>
  public class FunctionCollection<G, F>
    //where G: Enum where F: Enum
    where G : struct, IConvertible where F : struct, IConvertible
  {
    Dictionary<F, FunctionDataModel<G, F>> Collections { get; set; } = new Dictionary<F, FunctionDataModel<G, F>>();

    public int Count => Collections.Count;

    public FunctionCollection()
    {
      //this.Collections = GetCollection();
    }
    //protected abstract Dictionary<F, FunctionDataModel<G, F>> GetCollection();

    public FunctionCollection(List<FunctionDataModel<G, F>> functions)
    {
      this.Add(functions);
    }
    public void Add(FunctionDataModel<G, F> func)
    {
      if (this.Collections.ContainsKey(func.FunctionId))
        throw new Exception($"already has key : {func.FunctionId}");
      this.Collections.Add(func.FunctionId, func);
    }
    public void Add(List<FunctionDataModel<G, F>> functions)
    {
      foreach (var func in functions)
      {
        this.Add(func);
      }
    }
    public FunctionDataModel<G, F> GetFunction(F funcId)
    {
      Collections.TryGetValue(funcId, out FunctionDataModel<G, F> item);
      return item;
    }
    public List<FunctionDataModel<G, F>> GetAllActionMembers()
    {
      return Collections.Where(x => x.Value.FunctionType == FUNCTION_TYPE.ACTION).Select(d => d.Value).ToList();
    }
    public List<FunctionDataModel<G, F>> GetAllSetMembers()
    {
      return Collections.Where(x => x.Value.FunctionType == FUNCTION_TYPE.SET).Select(d => d.Value).ToList();
    }
    public List<FunctionDataModel<G, F>> GetAllGroupMembers()
    {
      return Collections.Where(x => x.Value.FunctionType == FUNCTION_TYPE.GROUP).Select(d => d.Value).ToList();
    }
    public List<FunctionDataModel<G, F>> GetGroupMembers(G groupId)
    {
      return Collections.Where(x => EqualityComparer<G>.Default.Equals(x.Value.GroupId, groupId)).Select(d => d.Value).ToList();
    }
    public void Clear()
    {
      Collections.Clear();
    }
  }
}
