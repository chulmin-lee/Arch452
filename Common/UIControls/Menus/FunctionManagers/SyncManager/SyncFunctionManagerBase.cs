using System;
using System.Collections.Generic;

namespace UIControls
{
  public abstract class SyncFunctionManagerBase<G, F> : ISyncFunctionObserver<G, F>
    //where G : Enum where F : Enum
    where G : struct, IConvertible
    where F : struct, IConvertible
  {
    List<ISyncFunctionSubscriber<G,F>> _subscribers = new List<ISyncFunctionSubscriber<G,F>>();

    public int Count => _subscribers.Count;

    public void Register(ISyncFunctionSubscriber<G, F> sub)
    {
      _subscribers.Add(sub);
    }

    /// <summary>
    /// 외부에서 기능을 실행한 경우 (예. 단축키로 실행된 경우) 모든 subsriber에게 통지
    /// </summary>
    /// <param name="functId"></param>
    public void ExternalAction(F functId)
    {
      foreach (var s in _subscribers)
      {
        s.ExternalAction(functId);
      }
    }
    /// <summary>
    /// 통지한 subscriber를 제외한 나머지에게 통지
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="groupId"></param>
    /// <param name="functionId"></param>
    /// <param name="select"></param>
    public void Broadcast(ISyncFunctionSubscriber<G, F> sender, G groupId, F functionId, bool select = true)
    {
      foreach (var s in _subscribers)
      {
        if (object.ReferenceEquals(s, sender))
        {
          continue;
        }

        s.Broadcast(groupId, functionId, select);
      }
    }
  }
}
