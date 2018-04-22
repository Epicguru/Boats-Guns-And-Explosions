using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class UnitSelection : NetworkBehaviour
{
    // A player script that manages selecting and manipulating units.
    public Player Player;

    public bool CanSelect = true;
    public Color SelectionColour = Color.white;

    private Vector2 start;
    private SpriteRenderer sel;

    public void Update()
    {
        if (isClient)
        {
            UpdateSelection();
        }
    }

    [Client]
    private void UpdateSelection()
    {
        if (InputManager.IsDown("Select"))
        {
            HideUnitOptions();
            start = InputManager.MousePos;
        }

        if (InputManager.IsPressed("Select"))
        {
            if (sel == null)
            {
                sel = SelectionBoundsPool.Instance.GetFromPool().Renderer;
            }

            if (sel != null)
            {
                sel.color = SelectionColour;
                sel.transform.position = start;
                sel.size = InputManager.MousePos - start;
            }
        }

        if (InputManager.IsUp("Select"))
        {
            if (sel != null)
            {
                SelectionBoundsPool.Instance.ReturnToPool(sel.GetComponent<SelectionBounds>());
                sel = null;
            }

            // Deselect if not pressing the 'select multiple' button.
            if (!InputManager.IsPressed("Select Multiple"))
            {
                Unit.DeselectPermanent();
            }
            // Select units in the bounds.
            Unit.SelectPermanent(new Rect(start, InputManager.MousePos - start));

            // Get options for the selected units (if any)
            if(Unit.CurrentlySelected.Count > 0)
            {
                RequestUnitOptions();
            }
        }
    }

    [Client]
    public void RequestUnitOptions()
    {
        // Request player options from server for selected objects.
        if (!isLocalPlayer)
            return;

        Unit[] units = Unit.CurrentlySelected.ToArray();
        // Filter out units that we do not own.
        for (int i = 0; i < units.Length; i++)
        {
            var unit = units[i];
            if (unit == null)
                continue;
            if (unit.Faction != Player.Faction)
                units[i] = null;
        }

        if (isServer)
        {
            RequestOptions_Server(units);
        }
        else
        {
            // Make game object array from the units array.
            GameObject[] gos = new GameObject[units.Length];
            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
                gos[i] = unit == null ? null : unit.gameObject;
            }
            CmdRequestOptions(gos, Player.netId.Value);
        }
    }

    [Client]
    public void HideUnitOptions()
    {
        if (!isLocalPlayer)
            return;

        UI_UnitOptions.Instance.Active = false;
        UI_UnitOptions.Instance.DestroySpawned();
    }

    [Server]
    private void RequestOptions_Server(Unit[] units)
    {
        // No need to validate units on the server...
        // Simply make a options dictionary using the unit array.

        var ops = Unit.GetAllOptions(units);

        // Final stage.
        FinalOptionsReceieved(ops);
    }

    [Command]
    private void CmdRequestOptions(GameObject[] gos, uint id)
    {
        if (id == 0) // Invalid id.
            return;

        if(gos == null || gos.Length == 0)
        {
            // Nothing to do...
            string json = JsonConvert.SerializeObject(new Dictionary<UnitOption, int>(), Formatting.None);
            RpcGetOptions(json, id);
        }
        else
        {
            Player player = Player.GetPlayer(id);
            if(player == null)
            {
                Debug.LogError("Player requested unit options, but player ID returned no player object!");
                return;
            }

            // Turn GameObjects into Units and validate units along the way.
            Unit[] units = new Unit[gos.Length];
            for (int i = 0; i < gos.Length; i++)
            {
                var go = gos[i];
                if(go != null)
                {
                    var u = go.GetComponent<Unit>();
                    if(u != null)
                    {
                        if(u.Faction != player.Faction)
                        {
                            Debug.LogWarning("Player requested options for a unit they do not own! Should not happen, keep an eye on player '{0}'".Form(player.Name));
                        }
                        else
                        {
                            units[i] = u;
                        }
                    }
                }
            }

            // Make options dictionary based on the validated units states.
            var dic = Unit.GetAllOptions(units);
            string json = JsonConvert.SerializeObject(dic);

            // Now give these options to the player...
            RpcGetOptions(json, id);
        }
    }

    [ClientRpc]
    private void RpcGetOptions(string options, uint id)
    {
        // The options param is a Json format serialization of a Dictionary.
        // UNet struggles with complex objects like generic dictionaries, so I just serialize it.

        if (Player.netId.Value != id)
            return;

         var ops = JsonConvert.DeserializeObject<Dictionary<UnitOption, int>>(options);

        if (ops == null)
            return;

        // Final!
        FinalOptionsReceieved(ops);
    }

    [Client]
    private void FinalOptionsReceieved(Dictionary<UnitOption, int> options)
    {
        if (options == null)
            return;

        // Spawn options in the UI.
        UI_UnitOptions.Instance.SpawnOptions(options);

        // Make sure the UI is active.
        UI_UnitOptions.Instance.Active = true;

        Debug.Log("Showing options...");
    }
}