using System;
using System.Collections.Generic;
using System.Linq;

namespace UIControls
{
  /// <summary>
  /// application에 맞는 function 목록을 생성하는 추상 클래스
  /// </summary>
  /// <typeparam name="G">group id</typeparam>
  /// <typeparam name="F">function id</typeparam>
  public abstract class FunctionCollectionComposer<G, F>
    //where F : Enum where G : Enum
    where G : struct, IConvertible where F : struct, IConvertible
  {
    public FunctionCollection<G, F> Functions { get; private set; }
    Dictionary<F, FunctionDataModel<G, F>> dic = new Dictionary<F, FunctionDataModel<G, F>>();

    public FunctionCollectionComposer()
    {
      ComposeCustomFunctions();
      this.Functions = new FunctionCollection<G, F>(dic.Values.ToList());
    }
    protected abstract void ComposeCustomFunctions();

    /// <summary>
    /// 상태 유지없이 실행하는 기능 정의
    /// </summary>
    protected void SingleFunction(F funcId)
    {
      if (dic.ContainsKey(funcId))
        throw new Exception($"{funcId} already exist");
      dic.Add(funcId, new SingleFunction<G, F>(funcId));
    }
    /// <summary>
    /// 단일로 on/off 를 반복하는 기능 (예. 로그 사용 켜기/끄기)
    /// </summary>
    protected void ToggleFunction(F funcId)
    {
      if (dic.ContainsKey(funcId))
        throw new Exception($"{funcId} already exist");
      dic.Add(funcId, new ToggleFunction<G, F>(funcId));
    }
    /// <summary>
    /// 그룹으로 묶에서 해당 그룹에서 유일하게 하나만 선택되는 기능
    /// 예) 도형 타입 선택 - 사각형/선/점/..
    /// </summary>
    protected void GroupToggleFunction(G groupId, F funcId)
    {
      if (dic.ContainsKey(funcId))
        throw new Exception($"{funcId} already exist");
      dic.Add(funcId, new GroupToggleFunction<G, F>(groupId, funcId));
    }
  }
}
