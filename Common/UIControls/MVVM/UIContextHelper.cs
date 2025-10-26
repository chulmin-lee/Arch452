using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UIControls
{
  public class UIContextHelper
  {
    public static Dispatcher UIDispatcher;
    public static string BaseDirectory = null;
    public static Window WindowHandle { get; private set; }

    public static void Initialize()
    {
      BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      UIDispatcher = Dispatcher.CurrentDispatcher;
    }
    public static void Initialize(Window w)
    {
      WindowHandle = w;
      BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      UIDispatcher = Dispatcher.CurrentDispatcher;
    }
    public static void SetWindowHandle(Window w)
    {
      WindowHandle = w;
    }
    /// <summary>
    /// ViewModel 에서 Dispatcher 접근
    /// 예) UIContextHelper.CheckBeginInvokeOnUI(() => { CommandManager.InvalidateRequerySuggested(); });
    /// </summary>
    /// <param name="action"></param>

    public static void CheckInvokeOnUI(Action action)
    {
      if (action == null)
      {
        return;
      }

      if (UIDispatcher.CheckAccess())
      {
        action();
      }
      else
      {
        UIDispatcher.Invoke(action);
      }
    }

    public static void CheckBeginInvokeOnUI(Action action)
    {
      if (action == null)
      {
        return;
      }

      if (UIDispatcher.CheckAccess())
      {
        action();
      }
      else
      {
        UIDispatcher.BeginInvoke(action);
      }
    }

    public static void CheckBeginInvokeOnRender(Action action)
    {
      if (action == null)
      {
        return;
      }

      if (UIDispatcher.CheckAccess())
      {
        action();
      }
      else
      {
        UIDispatcher.BeginInvoke(DispatcherPriority.Render, action);
      }
    }
  }
}
