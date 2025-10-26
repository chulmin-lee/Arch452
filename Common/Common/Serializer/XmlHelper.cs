using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml.Serialization;

namespace Common
{
  public static class XmlHelper
  {
    static readonly ConcurrentDictionary<Type, XmlSerializer> _cache = new ConcurrentDictionary<Type, XmlSerializer>();
    static XmlSerializer Get<T>() where T : class
    {
      var type =  typeof(T);
      if (!_cache.TryGetValue(type, out var value))
      {
        value = XmlSerializer.FromTypes(new[] { type })[0];
        if (value != null)
        {
          _cache.TryAdd(type, value);
        }
      }
      return value;
      //return _cache.GetOrAdd(typeof(T), _ => XmlSerializer.FromTypes(new[] { typeof(T) })[0]);
    }

    public static void Save<T>(this T t, string filename) where T : class
    {
      var serializer = Get<T>();
      if (serializer != null)
      {
        using (var stream = File.Create(filename))
          serializer.Serialize(stream, t);
      }
      else
      {
        throw new Exception("serailizer is null");
      }
    }
    public static T Load<T>(string path) where T : class
    {
      try
      {
        if (!File.Exists(path))
        {
          throw new Exception($"{path} not found");
        }

        var serializer = Get<T>();
        if (serializer != null)
        {
          using (var reader = new FileStream(path, FileMode.Open))
          {
            return (T)serializer.Deserialize(reader);
          }
        }
        else
        {
          throw new Exception("serailizer is null");
        }
      }
      catch (Exception ex)
      {
        LOG.except(ex);
      }
      return null;
    }

    public static T Deserialize<T>(string xml) where T : class
    {
      var serializer = Get<T>();
      if (serializer != null)
      {
        using (var reader = new StringReader(xml))
        {
          return (T)serializer.Deserialize(reader);
        }
      }
      else
      {
        throw new Exception("serailizer is null");
      }
    }
  }
}