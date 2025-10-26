using System;
using System.ComponentModel;
using System.Linq;

namespace Common
{
  public static class EnumHelper
  {
    public static T GetAttribute<T>(this Enum value) where T : Attribute
    {
      return (T)value.GetType().GetField(value.ToString())?.GetCustomAttributes(typeof(T), false)?.FirstOrDefault();
    }

    public static int EnumToInt<T>(this T value) where T : Enum
    {
      return (int)(object)value;
    }

    public static string GetDescription(this Enum value)
    {
      var attr = value.GetAttribute<DescriptionAttribute>();
      return attr?.Description ?? string.Empty;
    }
    //public static List<EnumDescription> GetEnumDescriptions(this Enum value)
    //{
    //  Type t = value.GetType();
    //  return Enum.GetValues(t).Cast<Enum>().Select((e) => new EnumDescription(e, e.GetDescription())).ToList();
    //}
  }

  //public class EnumDescription
  //{
  //  public object Value { get; set; }
  //  public string Description { get; set; }
  //  public EnumDescription(object value, string description)
  //  {
  //    Value = value;
  //    Description = description;
  //  }
  //}
}