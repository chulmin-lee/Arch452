using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UIControls
{
  public enum FUNCTION_TYPE
  {
    ACTION,       // 단순 실행. 상태 유지 없다.
    SET,       // 선택할때마다 상태가 반전된다. 다른 메뉴와 무관하게 동작 (independent), INDEPENDENT_TOGGLE
    GROUP, // 선택할때마다 상태가 반전된다. 같은 그룹에서는 하나만 선택 가능하다 (mutually exclusive), EXCLUSIVE_TOGGLE
  }

  /// <summary>
  /// 개별 기능에 대한 정의
  /// </summary>
  public class FunctionDataModel<G, F>
    //where G : Enum where F : Enum
    where G : struct, IConvertible where F : struct, IConvertible
  {
    public FUNCTION_TYPE FunctionType { get; set; }
    public G GroupId { get; set; } = default(G);   // domain, band,
    public F FunctionId { get; set; }

    protected FunctionDataModel(FUNCTION_TYPE type, F functionId)
    {
      this.FunctionType = type;
      this.FunctionId = functionId;
    }
    protected FunctionDataModel(FUNCTION_TYPE type, G groupId, F functionId)
    {
      this.FunctionType = type;
      this.GroupId = groupId;
      this.FunctionId = functionId;
    }
  }

  public class SingleFunction<G, F> : FunctionDataModel<G, F>
    //where G : Enum where F : Enum
    where G : struct, IConvertible where F : struct, IConvertible
  {
    public SingleFunction(F funcId) : base(FUNCTION_TYPE.ACTION, funcId) { }
  }
  public class ToggleFunction<G, F> : FunctionDataModel<G, F>
    //where G : Enum where F : Enum
    where G : struct, IConvertible where F : struct, IConvertible
  {
    public ToggleFunction(F funcId) : base(FUNCTION_TYPE.SET, funcId) { }
  }
  public class GroupToggleFunction<G, F> : FunctionDataModel<G, F>
    //where G : Enum where F : Enum
    where G : struct, IConvertible where F : struct, IConvertible
  {
    public GroupToggleFunction(G groupId, F funcId) : base(FUNCTION_TYPE.GROUP, groupId, funcId) { }
  }
}
