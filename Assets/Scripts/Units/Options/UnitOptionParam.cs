
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[JsonObject(MemberSerialization.OptIn)]
public class UnitOptionParams
{
    [JsonProperty]
    private Dictionary<byte, object> data = new Dictionary<byte, object>();

    public bool ContainsKey(byte key)
    {
        return data.ContainsKey(key);
    }

    public bool IsOfType<T>(byte key)
    {
        if(typeof(T) == typeof(GameObject))
        {
            uint x;
            return ContainsKey(key) && (data[key] == null ? false : TryConvertTo<uint>(data[key], out x));
        }
        else
        {
            return ContainsKey(key) && data[key] is T;
        }
    }

    public void Update(byte key, object value)
    {
        if(value is Component)
        {
            Debug.LogError("Cannot use components in unit params: use the GameObject is belongs to, and the GetComponent later. Has not been added to params.");
            return;
        }
        if(value is GameObject)
        {
            // Game objects that have a network ID have network identities are serialized differently.
            if (value == null)
                return;
            var net = (value as GameObject).GetComponent<NetworkIdentity>();
            if(net == null)
            {
                Debug.LogError("Passed a game object to the unit options params, but the game object does not have a network identity component. Cannot serialize without NetworkIdentity. Will not be added or updated.");
                return;
            }
            var id = net.netId.Value;

            if (ContainsKey(key))
            {
                data[key] = id;
            }
            else
            {
                data.Add(key, id);
            }
        }
        else
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

        if(typeof(T) == typeof(GameObject))
        {
            uint id = Get<uint>(key);
            if (id == default(uint))
            {
                Debug.LogWarning("ID is invalid, or the object at key is not a GameObject, returning default.");
                return defaultValue;
            }

            var o = ClientScene.FindLocalObject(new NetworkInstanceId(id));
            T t;
            bool worked = TryConvertTo<T>(o, out t);
            if (worked)
            {
                return t;
            }
            else
            {
                Debug.LogError("Failed to cast game object to generic param (T). Why? T should be GameObject or a sublcass of it. Returning default.");
                return defaultValue;
            }
        }

        if (obj is T)
        {
            return (T)obj;
        }
        else
        {
            T res;
            if(TryConvertTo<T>(obj, out res))
            {
                return res;
            }
            else
            {
                Debug.LogError("Wrong data type for key {0} in params, returning the default value. Requested type is '{1}' but real type is '{2}'!".Form(key, typeof(T).FullName, obj == null ? "<null>" : obj.GetType().FullName));
                if (obj is uint)
                {
                    Debug.LogError("It looks like you might be trying to retrieve a game object, use GetGameObject(id) instead of Get<GameObject>(id).");
                }
                return defaultValue;
            }
        }
    }

    private static bool TryConvertTo<T>(object input, out T result)
    {
        try
        {
            result = (T)Convert.ChangeType(input, typeof(T));
            return true;
        }
        catch
        {
            result = default(T);
            return false;
        }
    }

    private static JsonSerializerSettings settings;
    public static JsonSerializerSettings Settings
    {
        get
        {
            if(settings == null)
            {
                settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.None;
                settings.ContractResolver = UnityContractResolver.Instance;
                settings.TypeNameHandling = TypeNameHandling.Auto;
                settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
                settings.StringEscapeHandling = StringEscapeHandling.Default;
                settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            }
            return settings;
        }
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Settings);
    }

    public static UnitOptionParams TryDeserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            try
            {
                var deserialized = JsonConvert.DeserializeObject<UnitOptionParams>(json, Settings);
                return deserialized;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }
        catch
        {
            return null;
        }
    }
}