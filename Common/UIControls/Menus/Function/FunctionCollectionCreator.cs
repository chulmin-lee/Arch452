using System;
using System.Diagnostics;
using System.Linq;

namespace UIControls
{
  public static class FunctionCollectionCreator
  {
    public static FunctionCollection<G, F> CreateFunctionCollection<G, F>()
      //where G:Enum where F:Enum
      where G : struct, IConvertible
      where F : struct, IConvertible
    {
      G defaultGroupId = default(G);

      var collection = new FunctionCollection<G, F>();

      foreach (F funcId in Enum.GetValues(typeof(F)))
      {
        var attr = funcId.GetFunctionAttribute();

        if (attr == null)
          continue;

        G groupId = defaultGroupId;

        var o = attr.GroupId;
        if (o != null)
        {
          if (o is F)
          {
            groupId = (G)o;
          }
          else
          {
            try
            {
              groupId = (G)Convert.ChangeType(o, typeof(G));
            }
            catch (InvalidCastException ex)
            {
              // nothing
              Debug.WriteLine(ex.Message);
            }
          }
        }

        switch (attr.FunctionType)
        {
          case FUNCTION_TYPE.ACTION:
            {
              collection.Add(new SingleFunction<G, F>(funcId));
              break;
            }
          case FUNCTION_TYPE.SET:
            {
              collection.Add(new ToggleFunction<G, F>(funcId));
              break;
            }
          case FUNCTION_TYPE.GROUP:
            {
              if (groupId.IsDefaultValue())
              {
                throw new Exception("GroupId == NONE");
              }
              collection.Add(new GroupToggleFunction<G, F>(groupId, funcId));
              break;
            }
          default:
            throw new Exception($"unknown type: {attr.FunctionType}");
        }
      } // foreach
      return collection;
    }
  }
}
