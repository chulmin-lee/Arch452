using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
  public static class NewtonJson
  {
    static JsonSerializerSettings _settings =  new JsonSerializerSettings()
    {
      Formatting = Formatting.Indented,
      NullValueHandling = NullValueHandling.Ignore,
      DefaultValueHandling = DefaultValueHandling.Ignore,
      MissingMemberHandling = MissingMemberHandling.Ignore,
    };

    public static string Serialize<T>(T o)
    {
      return JsonConvert.SerializeObject(o, _settings);
    }
    public static byte[] SerializeToByteArray<T>(T o)
    {
      var json = JsonConvert.SerializeObject(o);
      return Encoding.UTF8.GetBytes(json);
    }

    public static bool Serialize<T>(T o, string path)
    {
      try
      {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
        {
          Directory.CreateDirectory(dir);
        }

        var json = Serialize(o);
        File.WriteAllText(path, json);
        return true;
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        return false;
      }
    }
    public static bool Serialize<T>(IEnumerable<T> list, string path)
    {
      try
      {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
        {
          Directory.CreateDirectory(dir);
          JsonSerializerSettings settings = new JsonSerializerSettings()
          {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
          };

          var json = JsonConvert.SerializeObject(list, settings);
          File.WriteAllText(path, json);
          return true;
        }
      }
      catch (Exception ex)
      {
        LOG.except(ex);
      }
      return false;
    }
    public static T Load<T>(string path) where T : class, new()
    {
      try
      {
        if (File.Exists(path))
        {
          var json = File.ReadAllText(path);
          return Deserialize<T>(json);
        }
      }
      catch (Exception ex)
      {
        LOG.except(ex);
      }
      return null;
    }
    public static T Deserialize<T>(string json) where T : class
    {
      try
      {
        return JsonConvert.DeserializeObject<T>(json);
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        LOG.e(json);
        return null;
      }
    }

    static object _lock = new object();
    static MethodInfo DeserializeObject;
    static Dictionary<Type, MethodInfo> GenericDeserializeObjects = new Dictionary<Type, MethodInfo>();
    public static T Deserialize<T>(Type type, string json) where T : class
    {
      try
      {
        lock (_lock)
        {
          // find method
          if (DeserializeObject == null)
          {
            var methods = typeof(JsonConvert).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                           .Where(x => x.Name.Equals("DeserializeObject", StringComparison.OrdinalIgnoreCase));
            foreach (MethodInfo method in methods)
            {
              if (method.GetParameters().Length == 1 && method.IsGenericMethod)
              {
                DeserializeObject = method;
                break;
              }
            }
          }

          // generic method
          if (DeserializeObject != null)
          {
            if (!GenericDeserializeObjects.TryGetValue(type, out var method))
            {
              method = DeserializeObject.MakeGenericMethod(type);
              GenericDeserializeObjects.Add(type, method);
            }
            return (T)method.Invoke(null, new object[] { json });
          }
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        Debug.WriteLine(json);
      }
      return null;
    }
  }
}