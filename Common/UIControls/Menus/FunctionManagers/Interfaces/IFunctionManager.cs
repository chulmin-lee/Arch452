using System;

namespace UIControls
{
  //---------------------------------------------------------------
  // constraint로 Enum 을 사용하면 binding에서 에러가 발생한다.
  // 그래서 예정 방식을 사용한다.
  // (X) where G : Enum
  // (O) where G : struct, IConvertible
  //---------------------------------------------------------------

  public interface ISyncFunctionObserver<G, F>
    where G : struct, IConvertible
    where F : struct, IConvertible
  {
    /// <summary>
    /// Menu Manager가 자신을 등록할때 호출
    /// </summary>
    /// <param name="sub"></param>
    void Register(ISyncFunctionSubscriber<G,F> sub);

    /// <summary>
    /// Menu Manager 내부에서 일어난 변경 사항을 다른 Menu Manaager에게 통지하기 위해서 호출
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="groupId"></param>
    /// <param name="functionId"></param>
    /// <param name="select"></param>
    void Broadcast(ISyncFunctionSubscriber<G, F> sender, G groupId, F functionId, bool select = true);

    /// <summary>
    /// Menu Manager가 아닌 외부에서 호출 (예. 단축키로 실행한 경우)
    /// </summary>
    /// <param name="functionId"></param>
    void ExternalAction(F functionId);
  }

  public interface ISyncFunctionSubscriber<G, F>
    where G : struct, IConvertible
    where F : struct, IConvertible
  {
    /// <summary>
    /// 다른 Menu Manager에서 설정이 변경된 경우 동기를 맞춘다.
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="functionId"></param>
    /// <param name="select"></param>
    /// <returns></returns>
    void Broadcast(G groupId, F functionId, bool select);

    /// <summary>
    /// 외부에서 기능이 실행된 경우 동기를 맞춘다.
    /// </summary>
    /// <param name="functionId"></param>
    /// <returns></returns>
    void ExternalAction(F functionId);
  }

  public interface IFunctionManager<G, F> : ISyncFunctionSubscriber<G,F>
    //where G : Enum where F : Enum
    where G : struct, IConvertible where F : struct, IConvertible
  {
    // 자식 FunctionItem에서 호출
    void LocalMemberChecked(G groupId, F functionId, bool select);
  }
}
