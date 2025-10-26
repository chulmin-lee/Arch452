using System;

namespace UIControls
{
  public class DeferredAction
  {
    Action _action;
    public DeferredAction(Action action)
    {
      this._action = action;
    }
    public void Execute()
    {
      this._action.Invoke();
    }
  }
}
