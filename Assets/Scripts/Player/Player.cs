using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public static List<Player> All = new List<Player>();

    [SyncVar]
    public string Name;

    [SyncVar]
    public Faction Faction;

    public override void OnStartClient()
    {
        if (isServer)
            return;

        if (!All.Contains(this))
        {
            All.Add(this);
        }
    }

    public override void OnStartServer()
    {
        if (!All.Contains(this))
        {
            All.Add(this);
        }
    }

    public void OnDestroy()
    {
        if (All.Contains(this))
        {
            All.Remove(this);
        }
    }
}