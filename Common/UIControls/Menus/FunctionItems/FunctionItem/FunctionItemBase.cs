using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace UIControls
{
  /// <summary>
  /// UI에 바인딩될 메뉴 아이템
  /// 주의: T consraint에 Enum 대신 struct, IConvertible 을 사용할것. Designer에서 에러 표시뜸
  /// </summary>
  /// <typeparam name="G"></typeparam>
  /// <typeparam name="F"></typeparam>

  public class FunctionItemBase<G, F> : FunctionItemCore
    //where G : Enum where F : Enum
    where G : struct, IConvertible where F : struct, IConvertible
  {
    public IFunctionManager<G, F> Manager { get; private set; }
    public FunctionItemBase<G, F> Parent { get; set; }
    public G GroupId { get; protected set; } = default(G); // 논리적 집합
    public F FunctionId { get; protected set; } = default(F);


    public FunctionItemBase()
    {
    }

    public FunctionItemBase(IFunctionManager<G, F> manager, string header, MenuIconPack icon = null)
    {
      Manager = manager;
      Role = FUNCTION_ITEM_ROLE.TOP_HEADER;
      _header = header;
      Icons = icon;
    }
    public FunctionItemBase(string header, MenuIconPack icon = null)
    {
      Role = FUNCTION_ITEM_ROLE.SUB_MENU;
      _header = header;
      Icons = icon;
    }

    protected FunctionItemBase(FunctionCommand<G, F> command, MenuIconPack iconpack,
                               bool checkable = false, bool ischecked = false, G groupId = default(G))
    {
      if (string.IsNullOrEmpty(command.Header))
      {
        throw new ArgumentNullException("header");
      }
      if (command == null)
      {
        throw new ArgumentNullException("command");
      }

      this.Role = FUNCTION_ITEM_ROLE.FUNCTION;
      this.FunctionId = command.FunctionId;
      this.GroupId = groupId;
      this.IsCheckable = checkable;

      if (this.IsCheckable)
      {
        this._isChecked = ischecked;
        this.IsGroupMember = this.GroupId.IsNotDefaultValue();
      }

      this._shortcuts = command.Shortcut;
      this.Header = command.Header;
      this.Command = command;
      this.Icons = iconpack;
    }

    #region Check
    public override bool IsChecked
    {
      get { return _isChecked; }
      set
      {
        if (Set(ref _isChecked, value))
        {
          if (this.IsGroupMember)
          {
            // group 메뉴인 경우에는 선택만 통지. (나머지는 자동 off)
            if (value)
              SelectionChanged(GroupId, FunctionId, value);
          }
          else
          {
            // 단일 toggle이면 on/off 모두 통지
            SelectionChanged(GroupId, FunctionId, value);
          }
        }
      }
    }
    /// <summary>
    /// 메뉴가 눌린것을 감지하기 위한 속성으로 다음을 감지하기위해서 사용한다.
    ///   1) MenuItem press
    ///   2) Button click
    /// Button/MenuItem.IsPressed는 readonly DP라서 바인딩이 안되므로, 전용 Attached를  사용하여 바인딩한다.
    /// 현재 선택중이 아니면 부모에게 통지한다.
    /// 두번 클릭시 동기화는 안되지만, command는 실행된다.
    /// 수정:
    ///   Menu의 경우 IsChecked가 또 발생하므로 Menu에는 적용하지 말자.
    /// </summary>
    public override bool IsMenuPressed
    {
      get { return _isMenuPressed; }
      set
      {
        if (Set(ref _isMenuPressed, value))
        {
          if (IsCheckable)
          {
            // toggle 메뉴
            if (this.IsGroupMember)
            {
              if (value && !IsChecked)
              {
                // 선택되었을때만 동기화 한다.
                // false라는 얘기는 다른 group 메뉴가 true라는 얘기임
                IsChecked = true;
              }
            }
            else
            {
              // 상태를 토글한다.
              IsChecked = !IsChecked;
            }
          }
          else
          {
            // toggle 메뉴가 아님.
            // 이 경우는 main menu/context menu에서 눌린것으로 동기화할 필요 없다.
          }
        }
      }
    }
    #endregion

    public override string Header
    {
      get { return _header; }
      set
      {
        if (Set(ref _header, value))
        {
          if (this.Role == FUNCTION_ITEM_ROLE.FUNCTION)
          {
            ToolTip = string.IsNullOrEmpty(Shortcuts) ? value : $"{value} ({Shortcuts})";
          }
        }
      }
    }

    public override string Shortcuts
    {
      get { return _shortcuts; }
      set
      {
        if (Set(ref _shortcuts, value))
        {
          if (this.Role == FUNCTION_ITEM_ROLE.FUNCTION)
          {
            ToolTip = string.IsNullOrEmpty(value) ? Header : $"{Header} ({value})";
          }
        }
      }
    }

    /// <summary>
    /// 외부 상태변화에 동기화 하기 위해서 사용.
    /// </summary>

    public void SelectionChanged(G groupId, F functionId, bool ischecked)
    {
      if (Manager != null)
      {
        Manager.LocalMemberChecked(groupId, functionId, ischecked);
      }
      else if (Parent != null)
      {
        Parent.SelectionChanged(groupId, functionId, ischecked);
      }
    }

    #region Children
    //public ObservableCollection<FunctionItemBase<G, F>> Children { get; set; } = new ObservableCollection<FunctionItemBase<G, F>>();

    public virtual void InsertSeperator()
    {
      var o = new FunctionItemBase<G,F>() {IsSeparator = true};
      Children.Add(o);
    }
    public virtual FunctionItemBase<G, F> InsertSubmenu(string header, MenuIconPack icon = null)
    {
      var o = new FunctionItemBase<G,F>(header, icon);
      o.Parent = this;
      Children.Add(o);
      return o;
    }
    public virtual FunctionItemBase<G, F> InsertActionItem(FunctionCommand<G, F> command, MenuIconPack icon = null)
    {
      var o = new FunctionItemBase<G,F>(command, icon);
      o.Parent = this;
      Children.Add(o);
      return o;
    }
    public virtual FunctionItemBase<G, F> InsertToggleItem(FunctionCommand<G, F> command, bool ischecked)
    {
      return this.InsertToggleItem(command, null, ischecked);
    }
    public virtual FunctionItemBase<G, F> InsertToggleItem(FunctionCommand<G, F> command, MenuIconPack icon = null, bool ischecked = false)
    {
      var o = new FunctionItemBase<G,F>(command, icon, checkable:true, ischecked);
      o.Parent = this;
      Children.Add(o);
      return o;
    }
    public virtual FunctionItemBase<G, F> InsertGroupToggleItem(FunctionCommand<G, F> command, G groupId, MenuIconPack icon = null, bool ischecked = false)
    {
      var o = new FunctionItemBase<G,F>(command, icon, checkable:true, ischecked, groupId);
      o.Parent = this;
      Children.Add(o);
      return o;
    }


    //public virtual void Add(FunctionItemBase<G,F> o)
    //{
    //  if (o != null)
    //  {
    //    o.Parent = this;
    //    Children.Add(o);

    //    //if (o.IsSyncMember)
    //    {
    //      this.ChildAdded(o);
    //    }
    //  }
    //}

    //public virtual void Add(List<FunctionItemBase<G,F>> items)
    //{
    //  foreach (var o in items)
    //  {
    //    this.Add(o);
    //  }
    //}
    //public FunctionItemBase<G, F> Find(string name)
    //{
    //  if (Header == name)
    //  {
    //    return this;
    //  }

    //  foreach (var item in Children)
    //  {
    //    var o = item.Find(name);
    //    if (o != null)
    //    {
    //      return o;
    //    }
    //  }
    //  return null;
    //}
    public FunctionItemBase<G, F> Find(F functionId)
    {
      if (this.FunctionId.AreEquals(functionId))
      {
        return this;
      }

      foreach (FunctionItemBase<G, F> item in Children)
      {
        var o = item.Find(functionId);
        if (o != null)
        {
          return o;
        }
      }
      return null;
    }
    #endregion
  }
}
