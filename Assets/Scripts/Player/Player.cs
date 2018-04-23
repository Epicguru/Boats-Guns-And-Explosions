using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public static List<Player> All = new List<Player>();
    public static Player Local;

    public UnitSelection UnitSelection;
    public UnitMovement UnitMovement;
    public UnitOptionExecution UnitOptionExecution;

    [SyncVar]
    public string Name;

    [SyncVar]
    public Faction Faction;

    public override void OnStartLocalPlayer()
    {
        Local = this;
    }

    public static Player GetPlayer(uint id)
    {
        if (All == null || All.Count == 0)
            return null;

        foreach (var player in All)
        {
            if(player.netId.Value == id)
            {
                return player;
            }
        }

        return null;
    }

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

        if (isLocalPlayer)
        {
            Local = null;
        }
    }
}