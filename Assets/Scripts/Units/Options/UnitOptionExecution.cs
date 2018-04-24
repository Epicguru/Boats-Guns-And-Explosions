using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class UnitOptionExecution : NetworkBehaviour
{
    // A player script that manages client based option execution requests.
    public Player Player;

    [Client]
	public void RequestOptionExecution(Unit[] units, UnitOption option, UnitOptionParams param = null)
    {
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

            string json = null;
            if(param != null)
            {
                json = param.ToJson();
            }

            // Send command request to the server.
            CmdRequestExec(gos, option, json);
        }
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
        UnitOptionParams p = UnitOptionParams.TryDeserialize(param);

        // Run the server version.
        RequestOption_Server(units, option, p);
    }

    [Server]
    private void RequestOption_Server(Unit[] units, UnitOption option, UnitOptionParams param = null)
    {
        if (units == null || units.Length == 0)
            return;

        // No need for validation, server side.
        Unit.ExecuteOption(units, option, param);

    }
}