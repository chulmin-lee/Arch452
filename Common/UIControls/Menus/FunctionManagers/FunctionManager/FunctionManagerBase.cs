using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace UIControls
{
  /// <summary>
  /// 메뉴매니저
  /// </summary>
  /// <typeparam name="B">band id</typeparam>
  public abstract class FunctionManagerBase<B, G, F> : IFunctionManager<G, F>
    //where B : Enum where G : Enum where F : Enum
    where B : struct, IConvertible where G : struct, IConvertible where F : struct, IConvertible
  {
    public string ManagerName { get; private set; }
    public Dictionary<G, FunctionGroupManager<G,F>> Groups;   // group 목록
    public Dictionary<B, FunctionItemBase<G,F>> BandHeaders;        // band 목록
    public Dictionary<F, FunctionItemBase<G,F>> AllFunctions;   // 전체 메뉴 목록. Header/seperator는 제외
    public ISyncFunctionObserver<G, F> _observer;
    public FunctionCollection<G, F> Functions;

    public List<FunctionItemBase<G, F>> Bands => BandHeaders.Values.ToList();

    public FunctionManagerBase(FunctionCollection<G, F> functions, MenuLayoutBase<B, G, F> layout, string name)
    {
      if (functions == null)
      {
        throw new ArgumentNullException("functions");
      }
      if (layout == null)
      {
        throw new ArgumentNullException("layout");
      }

      Functions = functions;
      ManagerName = name;
      Groups = new Dictionary<G, FunctionGroupManager<G, F>>();
      BandHeaders = new Dictionary<B, FunctionItemBase<G, F>>();
      AllFunctions = new Dictionary<F, FunctionItemBase<G, F>>();

      ComposeMenuLayout(layout);
    }
    public void SetObserver(ISyncFunctionObserver<G, F> observer)
    {
      _observer = observer;
      _observer.Register(this);
    }

    /// <summary>
    /// 자신의 토글 메뉴의 상태가 변경된 경우 호출된다.
    /// 1. 상태가 변경된 메뉴가 group member인 경우 해당 그룹의 이전 선택 항목을 선택 취소 한다.
    /// 2. SyncManager에게 변경 사항을 통지한다.
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="functionId"></param>
    /// <param name="select"></param>
    public void LocalMemberChecked(G groupId, F functionId, bool select)
    {
      if (functionId.IsDefaultValue())
      {
        throw new Exception("FunctionId == 0");
      }

      // groupId는 0 일 수 있다.
      if (groupId.IsNotDefaultValue())
      {
        // select는 항상 true 이다.
        FindGroup(groupId).LocalMemberChecked(functionId);
      }
      Debug.WriteLine($"Clicked [{ManagerName}] groupId: {groupId}, FunctionId: {functionId}");
      _observer.Broadcast(this, groupId, functionId, select);
    }


    /// <summary>
    /// 다른 MenuManager의 변경사항을 SyncManager가 통지해준다
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="functionId">checkable function id</param>
    /// <param name="selected">toggle 일때만 의미가 있다.</param>
    public void Broadcast(G groupId, F functionId, bool selected)
    {
      Debug.WriteLine($"Notifed()[{ManagerName}] groudId: {groupId}, menuId:{functionId}, select={selected}");

      var member = FindMember(functionId);
      var group = FindGroup(groupId);

      if (member == null)
      {
        // group 멤버인 경우 선택될때만(true) broadcast 하므로
        // member는 없지만, group이 존재한다면 해당 그룹의 모든 멤버들은 선택해제 한다.
        group?.UnselectAll();
      }
      else
      {
        // checkable인 경우에만 통지 한다.
        if (member.IsCheckable)
        {
          if (member.IsGroupMember)
          {
            // 그룹메뉴는 선택시에만 통지
            if (selected)
            {
              group.RemoteMemberChecked(functionId);
            }
            else
            {
              throw new Exception("group member sync : false");
            }
          }
          else
          {
            // 단순 토글
            member.CheckedUIUpdate(selected);
          }
        }
        else
        {
          throw new Exception("don't sync action function");
        }
      }
    }
    public void ExternalAction(F functionId)
    {
      Debug.WriteLine($"ExternalAction()[{ManagerName}] functionId:{functionId}");

      var info = this.Functions.GetFunction(functionId);
      var member = FindMember(functionId);

      switch (info.FunctionType)
      {
        case FUNCTION_TYPE.ACTION:
          {
            if(member != null)
            {
              StartAction(member);
            }
          } break;
        case FUNCTION_TYPE.SET:
          {
            if (member != null)
            {
              this.Broadcast(info.GroupId, info.FunctionId, true);
            }
          } break;
        case FUNCTION_TYPE.GROUP:
          {
            this.Broadcast(info.GroupId, info.FunctionId, true);
          }
          break;
      }
    }

    #region Menu Structure
    bool ComposeMenuLayout(MenuLayoutBase<B, G, F> composer)
    {
      foreach (var band in composer.Bands)
      {
        if (band.Type != MENU_MEMBER_TYPE.BAND)
        {
          throw new Exception("not HEADER");
        }

        if (band.BandId.IsDefaultValue())
        {
          throw new Exception("bandId == 0");
        }

        var o = create_band(band);

        BandHeaders.Add(band.BandId, o);
      }
      return true;
    }

    FunctionItemBase<G, F> create_band(MenuLayoutItem<B, G, F> o)
    {
      var header = new FunctionItemBase<G,F>(this, o.Header, o.Icon);
      {
        foreach (var item in o.Children)
        {
          switch (item.Type)
          {
            case MENU_MEMBER_TYPE.BAND:
              {
                throw new Exception("HEADER");
              }
            case MENU_MEMBER_TYPE.SUBMENU:
              {
                create_submenu(header, item);
                break;
              }
            case MENU_MEMBER_TYPE.MENU:
              {
                create_function_item(header, item);
                break;
              }
            case MENU_MEMBER_TYPE.SEPERATOR:
              {
                header.InsertSeperator();
                break;
              }
          }
        }
      }
      return header;
    }
    void create_submenu(FunctionItemBase<G, F> parent, MenuLayoutItem<B, G, F> o)
    {
      var sub = parent.InsertSubmenu(o.Header, o.Icon);
      foreach (var item in o.Children)
      {
        switch (item.Type)
        {
          case MENU_MEMBER_TYPE.BAND:
            {
              throw new Exception("HEADER");
            }
          case MENU_MEMBER_TYPE.SUBMENU:
            {
              create_submenu(sub, item);
              break;
            }
          case MENU_MEMBER_TYPE.MENU:
            {
              create_function_item(sub, item);
              break;
            }
          case MENU_MEMBER_TYPE.SEPERATOR:
            {
              sub.InsertSeperator();
              break;
            }
          default:
            throw new Exception("default");
        }
      }
    }


    void create_function_item(FunctionItemBase<G, F> parent, MenuLayoutItem<B, G, F> o)
    {
      var info = this.Functions.GetFunction(o.FunctionId);

      if (info == null || !o.FunctionId.AreEquals(info.FunctionId))
      {
        throw new Exception($"not found: {o.FunctionId}");
      }

      FunctionItemBase <G, F> m = null;

      switch (info.FunctionType)
      {
        case FUNCTION_TYPE.ACTION:
          {
            m = parent.InsertActionItem(o.Command, o.Icon);
            break;
          }
        case FUNCTION_TYPE.SET:
          {
            m = parent.InsertToggleItem(o.Command, o.Icon, o.IsChecked);
            break;
          }
        case FUNCTION_TYPE.GROUP:
          {
            m = parent.InsertGroupToggleItem(o.Command, info.GroupId, o.Icon, o.IsChecked);
            break;
          }
        default:
          throw new Exception("default");
      }

      // 같은 메뉴를 여러군데 등록할 수 있다.

      if (!AllFunctions.ContainsKey(m.FunctionId))
      {
        AllFunctions.Add(m.FunctionId, m);
        if (m.IsGroupMember)
        {
          FindGroupOrCreate(m.GroupId).Add(m);
        }
      }
    }
    #endregion

    #region BAND
    public FunctionItemBase<G, F> FindBand(B bandId)
    {
      BandHeaders.TryGetValue(bandId, out FunctionItemBase<G, F> band);
      return band;
    }
    public virtual void BandEnable(B id, bool enable)
    {
      FindBand(id)?.SetEnable(enable);
    }
    public virtual void BandShow(B id, bool show)
    {
      FindBand(id)?.SetVisible(show);
    }
    #endregion

    #region Group
    FunctionGroupManager<G, F> FindGroupOrCreate(G groupId)
    {
      var o = FindGroup(groupId);
      if (o == null)
      {
        o = new FunctionGroupManager<G, F>(groupId);
        Groups.Add(o.GroupId, o);
      }
      return o;
    }
    FunctionGroupManager<G, F> FindGroup(G groupId)
    {
      Groups.TryGetValue(groupId, out FunctionGroupManager<G, F> o);
      return o;
    }
    #endregion

    public FunctionItemBase<G, F> FindMember(F funcId)
    {
      AllFunctions.TryGetValue(funcId, out FunctionItemBase<G, F> o);
      return o;
    }
    #region virtual
    protected virtual void StartAction(FunctionItemBase<G, F> o) { }
    #endregion
  }
}
