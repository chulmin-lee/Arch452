using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Common
{
  public static class CollectionHelper
  {
    public static void AddRange<T>(this ObservableCollection<T> o, List<T> items)
    {
      items.ForEach(item => o.Add(item));
    }
    public static void Replace<T>(this ObservableCollection<T> o, List<T> items)
    {
      o.Clear();
      items.ForEach(item => o.Add(item));
    }
  }
}