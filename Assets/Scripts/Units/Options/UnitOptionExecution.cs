using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class UnitOptionExecution : NetworkBehaviour
{
    // A player script that manages client based option execution requests.
    public Player Player;

    [Client]
	public void RequestOptionExecutionUniqueParams(Unit[] units, UnitOption option, UnitOptionParams[] param)
    {
        if (isServer)
        {
            RequestOption_Server(units, option, param);
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

            string[] jsons = new string[param.Length];
            for (int i = 0; i < param.Length; i++)
            {
                var p = param[i];
                if(p != null)
                {
                    jsons[i] = p.ToJson();
                }
            }

            // Send command request to the server.
            CmdRequestExecUniqueParams(gos, option, jsons);
        }
    }

    [Client]
    public void RequestOptionExecution(Unit[] units, UnitOption option, UnitOptionParams param)
    {
        if (isServer)
        {
            RequestOption_Server(units, option, param);
        }
        else
        {
            // Make game object array from unit array.
            // Validate units at the same time.
            GameObject[] gos = new GameObject[units.Length];
            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
                if (unit != null)
                {
                    if (unit.Faction == Player.Faction)
                    {
                        gos[i] = unit.gameObject;
                    }
                }
            }

            string json = param == null ? null : param.ToJson();

            // Send command request to the server.
            CmdRequestExec(gos, option, json);
        }
    }

    [Command]
    private void CmdRequestExecUniqueParams(GameObject[] gos, UnitOption option, string[] param)
    {
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

        // Make the params object from the json. Works even if the json is null or blank.
        // May return null, which is fine.
        UnitOptionParams[] p = null;
        if(param != null)
        {
            p = new UnitOptionParams[param.Length];
            for (int i = 0; i < param.Length; i++)
            {
                Debug.Log("Received {0}".Form(param[i]));
                p[i] = UnitOptionParams.TryDeserialize(param[i]);
            }

        }

        // Run the server version.
        RequestOption_Server(units, option, p);
    }

    [Command]
    private void CmdRequestExec(GameObject[] gos, UnitOption option, string param)
    {
        if (gos == null || gos.Length == 0)
            return;

        // Turn gameobjects into units.
        // Validate units too.
        Unit[] units = new Unit[gos.Length];
        for (int i = 0; i < gos.Length; i++)
        {
            var go = gos[i];

            if (go != null)
            {
                var unit = go.GetComponent<Unit>();
                if (unit != null)
                {
                    if (unit.Faction == Player.Faction)
                    {
                        units[i] = unit;
                    }
                }
            }
        }

        // Make the params object from the json. Works even if the json is null or blank.
        // May return null, which is fine.
        UnitOptionParams p = null;
        if (param != null)
        {
            p = UnitOptionParams.TryDeserialize(param);
        }

        // Run the server version.
        RequestOption_Server(units, option, p);
    }

    [Server]
    private void RequestOption_Server(Unit[] units, UnitOption option, UnitOptionParams[] param)
    {
        if (units == null || units.Length == 0)
            return;

        // No need for validation, server side and already done.
        Unit.ExecuteOptions(units, option, param);
    }

    [Server]
    private void RequestOption_Server(Unit[] units, UnitOption option, UnitOptionParams param)
    {
        if (units == null || units.Length == 0)
            return;

        // No need for validation, server side and already done.
        Unit.ExecuteOptions(units, option, param);
    }
}