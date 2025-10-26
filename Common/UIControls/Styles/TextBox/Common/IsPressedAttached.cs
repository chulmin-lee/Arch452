using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UIControls
{

  // readonly dp binding

  /*
  <Button
    ui:IsPressedAttached.IsEnabled="True"
    ui:IsPressedAttached.ButtonPressed="{Binding IsSelected}"
  */
  public class IsPressedAttached : DependencyObject
  {
    #region IsEnabled
    public static readonly DependencyProperty IsEnabledProperty
         = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool),
              typeof(IsPressedAttached),
              new PropertyMetadata(false, OnIsEnabledChanged));
    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
      var element = d as FrameworkElement;
      if (element == null)
      {
        return;
      }

      if (args.NewValue != null && (bool)args.NewValue == true)
      {
        Attach(element);
      }
      else
      {
        Detach(element);
      }
    }
    public static bool GetIsEnabled(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsEnabledProperty);
    }
    public static void SetIsEnabled(DependencyObject obj, bool value)
    {
      obj.SetValue(IsEnabledProperty, value);
    }
    #endregion


    #region ButtonPressed
    public static readonly DependencyProperty ButtonPressedProperty
         = DependencyProperty.RegisterAttached("ButtonPressed", typeof(bool),
             typeof(IsPressedAttached),
             new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
             );
    public static bool GetButtonPressed(DependencyObject obj)
    {
      return (bool)obj.GetValue(ButtonPressedProperty);
    }
    public static void SetButtonPressed(DependencyObject obj, bool value)
    {
      obj.SetValue(ButtonPressedProperty, value);
    }
    #endregion


    private static void Attach(FrameworkElement element)
    {
      element.PreviewMouseDown += Element_MouseDown;
    }
    private static void Detach(FrameworkElement element)
    {
      element.PreviewMouseDown -= Element_MouseDown;
    }
    private static void Element_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      var element = sender as FrameworkElement;
      if (element == null)
      {
        return;
      }
      SetButtonPressed(element, true);
    }
  }
}
