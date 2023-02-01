using Newtonsoft.Json.Linq;

namespace ModularApp.Core.Helpers;

public static class JsonSerializerHelper
{
    #region Serialize
    public static string SerializeObject<T>(T obj)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
    }
    
    public static string TrySerializeObject<T>(T obj)
    {
        try
        {
            return SerializeObject(obj);

        }
        catch (Exception)
        {
            return "";
        }
    }
    
    public static string SerializeDynamicObject<T>(T obj)
    {
        return System.Text.Json.JsonSerializer.Serialize(obj);
    }
    
    public static string TrySerializeDynamicObject<T>(T obj)
    {
        try
        {
            return SerializeDynamicObject(obj);

        }
        catch (Exception)
        {
            return "";
        }
    }
    #endregion

    #region Deserialize
    public static T? DeserializeObject<T>(string serializedObject)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(serializedObject);
    }
    
    public static T? TryDeserializeObject<T>(string serializedObject)
    {
        try
        {
            return DeserializeObject<T>(serializedObject);
        }
        catch (Exception)
        {
            return default;
        }
    }
    
    public static T? DeserializeDynamicObject<T>(string serializedObject)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(serializedObject);
    }
    
    public static T? TryDeserializeDynamicObject<T>(string serializedObject)
    {
        try
        {
            return DeserializeDynamicObject<T>(serializedObject);
        }
        catch (Exception)
        {
            return default;
        }
    }
    #endregion

    #region Json Linq
    public static T? GetJsonValueByKey<T>(string json, string key)
    {
        if (string.IsNullOrEmpty(json)) return default;
        
        // json object begin
        if (json.StartsWith("{"))
        {
            var jo = JObject.Parse(json);

            var jToken = jo[key]?.ToString();
            if (jToken is null) return default;

            return DeserializeObject<T>(jToken);
        }

        if (json.StartsWith("["))
        {
            var ja = JArray.Parse(json);
            foreach (var jToken in ja)
            {
                if (jToken is not JObject jo) continue;
                  
                var token = jo[key]?.ToString();
                if (token is null) return default;

                return DeserializeObject<T>(token);
            }

            return default;
        }
        // json object end

        return default;
    }
    #endregion
}