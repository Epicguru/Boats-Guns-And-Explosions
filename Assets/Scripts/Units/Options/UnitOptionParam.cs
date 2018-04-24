
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class UnitOptionParams
{
    private Dictionary<byte, object> data = new Dictionary<byte, object>();

    public bool ContainsKey(byte key)
    {
        return data.ContainsKey(key);
    }

    public void Update(byte key, object value)
    {
        if (ContainsKey(key))
        {
            data[key] = value;
        }
        else
        {
            data.Add(key, value);
        }
    }

    public T Get<T>(byte key)
    {
        return Get<T>(key, default(T));
    }

    public T Get<T>(byte key, T defaultValue)
    {
        if (!ContainsKey(key))
        {
            Debug.LogWarning("Params does not contain the key {0}, returning default value.".Form(key));
            return defaultValue;
        }

        object obj = data[key];

        if(obj is T)
        {
            return (T)obj;
        }
        else
        {
            Debug.LogError("Wrong data type for key {0} in params, returning the default value. Requested type is '{1}' but real type is '{2}'!".Form(key, typeof(T).FullName, obj == null ? "<null>" : obj.GetType().FullName));
            return defaultValue;
        }
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.None);
    }

    public static UnitOptionParams TryDeserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var deserialized = JsonConvert.DeserializeObject<UnitOptionParams>(json);
            return deserialized;
        }
        catch
        {
            return null;
        }
    }
}