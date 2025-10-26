using System;
using System.Runtime.InteropServices;

namespace UIControls
{
  internal static class NativeUtils
  {
    internal static uint TPM_LEFTALIGN;

    internal static uint TPM_RETURNCMD;

    static NativeUtils()
    {
      NativeUtils.TPM_LEFTALIGN = 0;
      NativeUtils.TPM_RETURNCMD = 256;
    }

    [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
    internal static extern IntPtr PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
    internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
    internal static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

    [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
    internal static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

    /*
      WM_SYSCOMMAND=274 item: Selects an item from the window menu.
      SC_MONITORPOWER=61808: Power the display on/low/off via lParam= -1/1/2.
      SC_KEYMENU=61696: Open the Alt-letter menu; code of letter in lParam.
      SC_TASKLIST=61744: Open the Start menu.
      SC_SCREENSAVE=61760: Executes the screen saver.
      SC_NEXTWINDOW=61504: Selects next window.
      SC_PREVWINDOW=61520: Selects previous window.
      SC_MOVE=61456: Enables window move.
      SC_SIZE=61440: Enables window resize.
      SC_MINIMIZE=61472: Minimizes the window.
      SC_MAXIMIZE=61488: Maximizes the window.
      SC_RESTORE=61728: Restores the window.
      SC_CLOSE=61536: Closes the window.
      SC_SCROLL=65523: Enables "Scroll mode" in cmd.exe window. (Note 1)
      MF_ENABLED=0: Enables the menu item. (Note 2)
      MF_GRAYED=1: Disables and gray the menu item. (Note 2)
      MF_DISABLED=2: Disables the menu item, but not gray it. (Note 2)
    */

    public static IntPtr GetSystemMenu(IntPtr hwnd, bool isMax, bool resizable)
    {
      var menu = GetSystemMenu(hwnd, false);

      if (isMax)
      {
        NativeUtils.EnableMenuItem(menu, 61488, 1);
      }
      else
      {
        int v = (resizable) ? 0 : 1;
        NativeUtils.EnableMenuItem(menu, 61488, (uint)v);   // 0: enable, 1: disalbe
      }
      return menu;
    }
  }


}
