using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  public class FunctionGroupManager<G, F>
    //where G : Enum where F : Enum
  where G : struct, IConvertible where F : struct, IConvertible
  {
    public G GroupId { get; private set; }

    Dictionary<F, FunctionItemBase<G,F>> Members = new Dictionary<F, FunctionItemBase<G, F>>();

    public FunctionGroupManager(G id)
    {
      this.GroupId = id;
    }

    public void Add(FunctionItemBase<G,F> o)
    {
      if(!this.GroupId.AreEquals(o.GroupId))
        throw new Exception($"wrong groupId = {o.GroupId}");

      //if (!o.IsSyncMember)
      //  throw new Exception("menuId == 0");

      this.Members.Add(o.FunctionId, o);
    }
    /// <summary>
    /// 모든 멤버들의 선택을 해제한다.
    /// </summary>
    public void UnselectAll()
    {
      foreach (var p in Members.Values)
      {
        // GroupMember는 True일때는 trigger 동작을 한다.
        //p.IsChecked = false; // r혹시 몰라서..
        p.CheckedUIUpdate(false);
        //if (p.IsChecked)
        //{
        //  // binding에 의해서 다른 동작이 수행되는것을 막기 위해서
        //  // 단순 UI 업데이트만 한다.
        //  p.CheckedUIUpdate(false);
        //  return;
        //}
      }
    }

    /// <summary>
    /// local FM 에서 처리
    /// 이미 선택된 상태에서 부모에게 통지가 온것이므로 id는 checked 상태
    /// 이전에 선택된 메뉴가 있으면 해제한다.
    /// </summary>
    /// <param name="id">새로 선택된 function id. 이미 true 상태임</param>
    public void LocalMemberChecked(F id)
    {
      var old = Members.Where(x => !x.Key.AreEquals(id) && x.Value.IsChecked).Select(d => d.Value).FirstOrDefault();

      if(old != null)
      {
        old.CheckedUIUpdate(false);
      }
    }
    /// <summary>
    /// 외부 그룹 멤버가 선택되었을때 호출된다.
    /// </summary>
    /// <param name="id">외부 FM에서 선택된 function id</param>
    /// <param name="select"></param>
    public void RemoteMemberChecked(F id)
    {
      this.UnselectAll();
      Find(id)?.CheckedUIUpdate(true);
    }
    FunctionItemBase<G,F> Find(F id)
    {
      Members.TryGetValue(id, out FunctionItemBase<G,F> o);
      return o;
    }
    /// <summary>
    /// 지정된 id 이외의 목록
    /// </summary>
    /// <param name="except_id"></param>
    /// <returns></returns>
    List<FunctionItemBase<G,F>> FindExcept(F except_id)
    {
      return Members.Where(x => !x.Key.AreEquals(except_id)).Select(d => d.Value).ToList();
    }
    /// <summary>
    /// 지정된 id 외에 선택된 메뉴를 찾는다.
    /// </summary>
    /// <param name="except_id"></param>
    /// <returns></returns>
    FunctionItemBase<G,F> FindPrevSelectedMenu(F except_id)
    {
      return Members.Where(x => !x.Key.AreEquals(except_id) && x.Value.IsChecked).Select(d => d.Value).FirstOrDefault();
    }
  }
}
