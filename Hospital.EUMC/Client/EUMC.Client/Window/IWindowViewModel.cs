using ServiceCommon;
using ServiceCommon.ClientServices;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace EUMC.Client
{
  public interface IWindowViewModel
  {
    event EventHandler ContentChanged;
    IContentViewModel Contents { get; }
    bool IsCustomWindow { get; }
    void ConfigChanged(IPackageViewConfig config);
    void ReceiveMessage(ServiceMessage m);
    void Close();
    List<KeyBinding> KeyBindings();
  }
}