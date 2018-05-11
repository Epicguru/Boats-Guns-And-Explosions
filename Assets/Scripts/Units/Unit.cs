
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Unit : NetworkBehaviour
{
    public static List<Unit> CurrentlySelected = new List<Unit>();
    private static List<Unit> selected;
    private static List<Unit> all = new List<Unit>();
    private static Dictionary<UnitOption, int> options = new Dictionary<UnitOption, int>();
    private static List<UnitOption> tempOps = new List<UnitOption>();

    [Tooltip("Display name.")]
    public string Name;

    public ushort ID
    {
        get
        {
            return _id;
        }
    }
    [SerializeField]
    [Tooltip("Internal ID")]
    private ushort _id;

    public Faction Faction
    {
        get
        {
            return _faction;
        }
        set
        {
            if (!isServer)
            {
                Debug.LogError("Cannot set faction outside of server!");
            }
            else
            {
                _faction = value;
            }
        }
    }
    [SerializeField]
    [SyncVar]
    private Faction _faction;

    public SpriteRenderer Bounds;

    public bool IsSelected
    {
        get
        {
            return CurrentlySelected.Contains(this);
        }
    }

    private bool renderBounds;
    private SpriteRenderer selBounds;

    public void Awake()
    {
        all.Add(this);
    }

    public void OnDestroy()
    {
        Deselect();
        all.Remove(this);
    }

    public void Select()
    {
        if (!CurrentlySelected.Contains(this))
        {
            CurrentlySelected.Add(this);
        }
    }

    public void Deselect()
    {
        if (CurrentlySelected.Contains(this))
        {
            CurrentlySelected.Remove(this);
        }
    }

    public void Update()
    {
        // Both client and server:
        UpdateSelectionBounds();
    }

    [Server]
    public void ExecOption(OptionAndParams option)
    {
        BroadcastMessage("ExecuteOption", option, SendMessageOptions.DontRequireReceiver);
    }

    private void UpdateSelectionBounds()
    {
        renderBounds = CurrentlySelected.Contains(this);

        // Completely local and client sided. Just visual.
        if (renderBounds)
        {
            if (Bounds == null)
            {
                Bounds = GetComponentInChildren<SpriteRenderer>();
                if (Bounds == null)
                {
                    Debug.LogWarning("No Bounds sprite renderer assigned in editor, and no SpriteRenderers were found at runtime on children. The bounds are not rendererd. ({0})".Form(name));
                    return;
                }
            }
            if (selBounds == null)
            {
                // Create selection bounds...
                var sel = SelectionBoundsPool.Instance.GetFromPool();
                selBounds = sel.Renderer;
            }

            if (selBounds != null)
            {
                // Set position of selection bounds.
                selBounds.color = Faction.GetColour();
                selBounds.transform.position = Bounds.bounds.min - Bounds.bounds.size * 0.1f;
                selBounds.size = Bounds.bounds.size * 1.2f;
            }
        }
        else
        {
            if (selBounds != null)
            {
                SelectionBoundsPool.Instance.ReturnToPool(selBounds.GetComponent<SelectionBounds>());
                selBounds = null;
            }
        }
    }

    public virtual void GetOptions(List<UnitOption> options)
    {
        BroadcastMessage("GetUnitOptions", options, SendMessageOptions.DontRequireReceiver);
    }

    [Server]
    public abstract void SetMovementTarget(Vector2 target);

    public static List<Unit> Select(Rect r)
    {
        // Looping through all units and querying bounds is faster than using a box raycast.
        // But with very large ships means that selecting the edge of this ship does not select the unit.
        if (selected == null)
        {
            selected = new List<Unit>();
        }
        selected.Clear();
        foreach (var unit in all)
        {
            if (unit == null)
                continue;

            if(r.Contains(unit.transform.position, true))
            {
                selected.Add(unit);
            }
        }

        return selected;
    }

    public static void SelectPermanent(Rect r)
    {
        // Same as Select but copies the result to the static CurrentlySelected list.
        var found = Select(r);
        foreach (var unit in found)
        {
            unit.Select();
        }
    }

    public static void DeselectPermanent()
    {
        // De-select all...
        CurrentlySelected.Clear();
    }

    [Server]
    public static void MoveUnitsTo(Unit[] units, Vector2 target)
    {
        if (units == null || units.Length == 0)
            return;

        foreach (var unit in units)
        {
            if (unit == null)
                continue;

            unit.SetMovementTarget(target);
        }
    }

    public static Dictionary<UnitOption, int> GetAllOptions(Unit[] units)
    {
        // Gets all options for an array of units.
        // Records what options are avaiable and how many units requested them.

        options.Clear();
        tempOps.Clear();
        if (units == null)
            return options;
        if (units.Length == 0)
            return options;

        for (int i = 0; i < units.Length; i++)
        {
            var unit = units[i];

            if (unit == null)
                continue;

            tempOps.Clear();
            unit.GetOptions(tempOps);

            for (int j = 0; j < tempOps.Count; j++)
            {
                var uo = tempOps[j];
                if (!options.ContainsKey(uo))
                {
                    options.Add(uo, 1);
                }
                else
                {
                    options[uo] += 1;                
                }
            }
        }
        tempOps.Clear();
        return options;
    }

    [Server]
    public static void ExecuteOption(Unit unit, UnitOption option, UnitOptionParams param)
    {
        if (unit == null)
            return;

        unit.ExecOption(new OptionAndParams() { Option = option, Params = param });
    }

    [Server]
    public static void ExecuteOption(Unit[] units, UnitOption option, UnitOptionParams param)
    {
        if (units == null || units.Length == 0)
            return;

        foreach (var unit in units)
        {
            if(unit != null)
            {                
                ExecuteOption(unit, option, param);
            }
        }
    }

    [Server]
    public static void ExecuteOption(Unit[] units, UnitOption option, UnitOptionParams[] param)
    {
        if (units == null || units.Length == 0)
            return;
        if(param != null && param.Length != units.Length)
        {
            Debug.LogError("Not enough parameters for the number of units supplied! (Total {0} units, {1} params). Pass a null array to not execute without options.".Form(units.Length, param.Length));
            return;
        }

        for (int i = 0; i < units.Length; i++)
        {
            var unit = units[i];
            if (units != null)
            {
                UnitOptionParams p = null;
                if(param != null)
                {
                    p = param[i];
                }
                ExecuteOption(unit, option, p);
            }
        }
    }

    public static void Dispose()
    {
        // Looks strange, but this is best way to dispose and reset.
        // TODO implement.

        CurrentlySelected = new List<Unit>();
        all = new List<Unit>();
        selected = null;
        options = new Dictionary<UnitOption, int>();
        tempOps = new List<UnitOption>();
    }
}