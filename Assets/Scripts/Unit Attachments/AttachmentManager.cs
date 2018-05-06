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
        if (!Sockets.ContainsKey(socketID))
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

    [Server]
    public void Attach(byte socketID, byte ID)
    {

    }
}