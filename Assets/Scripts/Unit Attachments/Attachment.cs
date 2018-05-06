﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetParenting))]
[RequireComponent(typeof(NetPosSync))]
public class Attachment : NetworkBehaviour
{
    public NetParenting Parenting
    {
        get
        {
            if(_parenting == null)
            {
                _parenting = GetComponent<NetParenting>();
            }
            return _parenting;
        }
    }
    private NetParenting _parenting;

    public NetPosSync PosSync
    {
        get
        {
            if(_posSync == null)
            {
                _posSync = GetComponent<NetPosSync>();
            }
            return _posSync;
        }
    }
    private NetPosSync _posSync;

    public AttachmentType ID
    {
        get
        {
            return _id;
        }
    }
    [SerializeField]
    private AttachmentType _id;

    public AttachmentSize Size
    {
        get
        {
            return _size;
        }
    }
    [SerializeField]
    private AttachmentSize _size;

    public string Name = "Big Gun";

    private static Dictionary<byte, Attachment> Loaded;

    public static void Load()
    {
        if (Loaded != null)
            return;

        Loaded = new Dictionary<byte, Attachment>();
        var found = Resources.LoadAll<Attachment>("Unit Attachments");

        foreach (var a in found)
        {
            if (!Loaded.ContainsKey((byte)a.ID))
            {
                Loaded.Add((byte)a.ID, a);
            }
            else
            {
                Debug.LogError("'{0}' has a duplicate unit attachment ID! ID: {1}".Form(a.Name, (byte)a.ID));
            }
        }

        Debug.Log("Loaded {0} unit attachments.".Form(Loaded.Count));
    }

    public static void NetRegister()
    {
        if (Loaded == null)
            return;

        foreach (var pair in Loaded)
        {
            NetworkPrefabs.Add(pair.Value.gameObject);
        }
    }

    public static void Unload()
    {
        if (Loaded == null)
            return;

        Loaded.Clear();
        Loaded = null;
    }
}

public enum AttachmentType : byte
{
    AUTO_CANNON
}

public enum AttachmentSize : byte
{
    SMALL,
    MEDIUM,
    LARGE,
    HUGE
}