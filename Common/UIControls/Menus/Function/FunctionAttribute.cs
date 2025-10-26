using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  [AttributeUsage(AttributeTargets.All)]
  public class FunctionAttribute : Attribute
  {
    public FUNCTION_TYPE FunctionType { get; set; }
    public object GroupId { get; set; }

    public FunctionAttribute() { }
    public FunctionAttribute(FUNCTION_TYPE type , object groupId = null)
    {
      this.FunctionType = type;
      this.GroupId = groupId;
    }
  }

  public static class FunctionExtension
  {
    public static FunctionAttribute GetFunctionAttribute<T>(this T value)
      //where T : Enum
      where T : struct, IConvertible
    {
      return value.GetType().GetField(value.ToString())?.GetCustomAttributes(typeof(FunctionAttribute), false).FirstOrDefault() as FunctionAttribute;
    }
  }
}
