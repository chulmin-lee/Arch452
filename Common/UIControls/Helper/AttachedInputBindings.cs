using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace UIControls
{
  public static class AttachedInputBindings
  {
    public static readonly DependencyProperty InputBindingsSourceProperty
      = DependencyProperty.RegisterAttached("InputBindingsSource",
                typeof(IEnumerable), typeof(AttachedInputBindings),
                new UIPropertyMetadata(null, InputBindingsSourceChanged));
    public static IEnumerable GetInputBindingsSource(DependencyObject dp)
    {
      return (IEnumerable)dp.GetValue(InputBindingsSourceProperty);
    }
    public static void SetInputBindingsSource(DependencyObject dp, IEnumerable value)
    {
      dp.SetValue(InputBindingsSourceProperty, value);
    }
    static void InputBindingsSourceChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      var uiElement = dp as UIElement;
      if (uiElement == null)
        throw new Exception(String.Format("Object of type '{0}' does not support InputBindings", dp.GetType()));

      uiElement.InputBindings.Clear();
      if (e.NewValue != null)
      {
        var bindings = (IEnumerable)e.NewValue;
        foreach (var binding in bindings.Cast<InputBinding>())
          uiElement.InputBindings.Add(binding);
      }
    }
  }
}
