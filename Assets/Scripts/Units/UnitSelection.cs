using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class UnitSelection : NetworkBehaviour
{
    // A player script that manages selecting and manipulating units.
    public Player Player;

    public bool CanSelect = true;
    public Color SelectionColour = Color.white;

    public bool ServerCanViewAll = false;

    private Vector2 start;
    private SpriteRenderer sel;
    private bool selecting;

    private const int MAX_RAY_HITS = 20;
    private RaycastHit2D[] hits = new RaycastHit2D[MAX_RAY_HITS];

    public void Update()
    {
        if (isLocalPlayer)
        {
            UpdateSelection();
            UpdateUnitOverview();
        }
    }

    private void UpdateUnitOverview()
    {
        if (Unit.CurrentlySelected != null || UI_ShipOverview.Instance != null)
        {
            if (Unit.CurrentlySelected.Count == 0)
            {
                UI_ShipOverview.Instance.Ship = null;
            }
            else
            {
                Unit u = Unit.CurrentlySelected[0];
                if (u != null)
                {
                    // Ship...
                    var ship = u.GetComponent<Ship>();
                    if (ship != null)
                    {
                        UI_ShipOverview.Instance.Ship = ship;
                        bool show = false;
                        if (isServer)
                        {
                            if (ServerCanViewAll)
                            {
                                show = true;
                            }
                        }
                        if (Player.Faction == u.Faction)
                        {
                            show = true;
                        }

                        UI_ShipOverview.Instance.IsEnemy = !show;
                        UI_ShipOverview.Instance.TotalSelected = Unit.CurrentlySelected.Count;
                    }
                }
            }
        }
    }

    [Client]
    private void UpdateSelection()
    {
        if (InputManager.IsDown("Select") && !selecting)
        {
            // If not in options UI part of screen...
            bool mouseInUI = InputManager.MouseOverAnyUI;

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

            // Select whatever was just clicked on.
            Unit underMouse = GetUnitUnder(InputManager.MousePos);
            if(underMouse != null)
            {
                underMouse.Select();
            }
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

    private Unit GetUnitUnder(Vector2 pos)
    {
        Ray r = new Ray(new Vector3(0f, 0f, -100f) + (Vector3)pos, Vector3.forward);
        int total = Physics2D.GetRayIntersectionNonAlloc(r, hits);
        if(total > MAX_RAY_HITS)
        {
            Debug.LogWarning("Number of raycast hits under the unit selection exceeds the max capacity.");
            total = MAX_RAY_HITS;
        }

        for (int i = 0; i < total; i++)
        {
            var hit = hits[i];

            var unit = hit.transform.GetComponentInParent<Unit>();
            if(unit != null)
            {
                return unit;
            }
        }
        return null;
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