﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class UnitOptionExecution : NetworkBehaviour
{
    // A player script that manages client based option execution requests.
    public Player Player;

    [Client]
	public void RequestOptionExecution(Unit[] units, UnitOption option)
    {
        // TODO params

        if (isServer)
        {
            RequestOption_Server(units, option);
        }
        else
        {
            // Make game object array from unit array.
            // Validate units at the same time.
            GameObject[] gos = new GameObject[units.Length];
            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
                if(unit != null)
                {
                    if(unit.Faction == Player.Faction)
                    {
                        gos[i] = unit.gameObject;
                    }
                }
            }

            // Send command request to the server.
            CmdRequestExec(gos, option);
        }
    }

    [Command]
    private void CmdRequestExec(GameObject[] gos, UnitOption option)
    {
        // TODO params
        if (gos == null || gos.Length == 0)
            return;

        // Turn gameobjects into units.
        // Validate units too.
        Unit[] units = new Unit[gos.Length];
        for (int i = 0; i < gos.Length; i++)
        {
            var go = gos[i];

            if(go != null)
            {
                var unit = go.GetComponent<Unit>();
                if(unit != null)
                {
                    if(unit.Faction == Player.Faction)
                    {
                        units[i] = unit;
                    }
                }
            }
        }

        // Run the server version.
        RequestOption_Server(units, option);
    }

    [Server]
    private void RequestOption_Server(Unit[] units, UnitOption option)
    {
        if (units == null || units.Length == 0)
            return;

        // No need for validation, server side.
        Unit.ExecuteOption(units, option);
    }
}