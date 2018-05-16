using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Unit))]
public class AttachmentManager : NetworkBehaviour
{
    public Unit Unit
    {
        get
        {
            if(_unit == null)
            {
                _unit = GetComponent<Unit>();
            }
            return _unit;
        }
    }
    private Unit _unit;

    public Dictionary<byte, AttachmentSocket> Sockets = new Dictionary<byte, AttachmentSocket>();

    public void Awake()
    {
        // Find sockets...
        RefreshSockets();
    }

    public void Start()
    {
        if (isServer)
        {
            for (byte i = 0; i < Sockets.Count; i++)
            {
                Attach(i, (byte)AttachmentType.AUTO_CANNON);
            }
        }
    }

    public void RefreshSockets()
    {
        Sockets.Clear();
        var found = GetComponentsInChildren<AttachmentSocket>();

        foreach (var item in found)
        {
            if (!Sockets.ContainsKey(item.NetParent.GetID()))
            {
                Sockets.Add(item.NetParent.GetID(), item);
            }
            else
            {
                Debug.LogError("Error in attachment socket setup, duplicate socket NetParent ID values: {0}, socket '{1}'. Make sure all NetParents have unique IDs.".Form(item.NetParent.GetID(), item.Name));
            }
        }
    }

    public Attachment GetAttached(byte socketID)
    {
        if (!HasSocket(socketID))
        {
            return null;
        }

        var socket = Sockets[socketID];

        if(socket == null)
        {
            Debug.LogError("Null socket! Why was it destroyed?");
            return null;
        }
        else
        {
            return socket.GetCurrentlyAttached();
        }
    }

    public bool IsAnyAttachment(byte socketID)
    {
        return GetAttached(socketID) != null;
    }

    public bool HasSocket(byte socketID)
    {
        return Sockets.ContainsKey(socketID);
    }

    [Server]
    public void Attach(byte socketID, byte ID)
    {
        if (!HasSocket(socketID))
            return;

        var socket = Sockets[socketID];
        if (socket == null)
        {
            Debug.LogError("Null socket, cannot attach, why is it destroyed?!?");
            return;
        }

        var prefab = Attachment.Get(ID);

        if(prefab == null)
        {
            Debug.LogWarning("Attachment prefab for ID: {0} could not be found, no action taken.".Form(ID));
            return;
        }

        if (!socket.IsValid(prefab))
        {
            Debug.LogWarning("Invalid attachment for the slot, no action taken. Socket: ID({0}), Name({1}), Attachment: ID({2}), Name({3})".Form(socketID, socket.Name, ID, prefab.Name));
            return;
        }

        var a = GetAttached(socketID);

        if(a != null)
        {
            // Remove old attachment, the add new.
            RemoveAttachment(socketID);
        }

        // Now spawn new attachment...
        var spawned = Instantiate(prefab);

        // Do whatever needs doing to attachment here...
        // Parent...
        var parenting = spawned.GetComponent<NetParenting>();
        if(parenting != null)
        {
            parenting.SetParent(socket.NetParent, true);
        }
        else
        {
            Debug.LogError("The attachment {0} does not have a net parent component!".Form(spawned.Name));
        }
        spawned.transform.localPosition = Vector3.zero;
        spawned.transform.localEulerAngles = Vector3.zero;

        // Now spawn on network for everyone to see...
        NetworkServer.Spawn(spawned.gameObject);
    }

    [Server]
    public void RemoveAttachment(byte socketID)
    {
        // Server side removal of attachment. Just destroy it.
        var a = GetAttached(socketID);

        if(a != null)
        {
            Destroy(a);
        }
    }
}