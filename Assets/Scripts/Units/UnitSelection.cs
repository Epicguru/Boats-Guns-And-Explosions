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
    private bool selecting;

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
        if (InputManager.IsDown("Select") && !selecting)
        {
            // If not in options UI part of screen...
            bool mouseInUI = UI_UnitOptions.Instance.OptionsBounds.rect.Contains(InputManager.ScreenMousePos);

            if (!mouseInUI)
            {
                start = InputManager.MousePos;
                selecting = true;
            }
        }

        if (selecting)
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

        if (InputManager.IsUp("Select") && selecting)
        {
            selecting = false;
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
        }

        // Get options for the selected units (if any)
        if (Unit.CurrentlySelected.Count > 0)
        {
            RequestUnitOptions();
        }
        else
        {
            HideUnitOptions();
        }
    }

    [Client]
    public void RequestUnitOptions()
    {
        if (!isLocalPlayer)
        {
            Debug.LogError("Cannot request unit options on a player that is not yours!");
            return;
        }
        if(Unit.CurrentlySelected.Count == 0)
        {
            return;
        }

        Unit[] units = Unit.CurrentlySelected.ToArray();

        // Validate units...
        for (int i = 0; i < units.Length; i++)
        {
            var unit = units[i];
            if(unit != null)
            {
                // Remove units that are not controlled by us (this faction)
                if(unit.Faction != Player.Faction)
                {
                    units[i] = null;
                }
            }
        }

        var options = Unit.GetAllOptions(units);

        // Display options...
        UI_UnitOptions.Instance.Active = true;
        UI_UnitOptions.Instance.SpawnOptions(options);
    }

    [Client]
    public void HideUnitOptions()
    {
        // Hide options.
        UI_UnitOptions.Instance.DestroySpawned();
        UI_UnitOptions.Instance.Active = false;
    }
}