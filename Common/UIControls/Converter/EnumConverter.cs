using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace UIControls
{
  /*
  - Usages
    <ComboBox ItemsSource="{Binding EnumValue, Converter={local:EnumToListConverter}, Mode=OneTime}"
              SelectedValuePath="Value"
              DisplayMemberPath="Description"
              SelectedValue="{Binding EnumValue}"/>
  */

  public class EnumDescription
  {
    public object Value { get; set; }
    public string Description { get; set; }
    public EnumDescription(object value, string desc)
    {
      this.Value = value;
      this.Description = desc;
    }
  }

  public class EnumToListConverter : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Type t = value.GetType();
      if (!t.IsEnum)
        throw new ArgumentException($"{nameof(t)} must be an enum type");
      return ToList(t);
    }
    static List<EnumDescription> ToList(Type t)
    {
      return Enum.GetValues(t).Cast<Enum>().Select((e) => GetValueDescription(e)).ToList();
    }
    static EnumDescription GetValueDescription(Enum value)
    {
      var attr = value.GetType().GetField(value.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault()
                 as DescriptionAttribute;

      var desc = attr?.Description;
      if (string.IsNullOrEmpty(desc))
      {
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        desc = ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
      }
      return new EnumDescription(value, desc);
    }
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new NotImplementedException();
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }
}