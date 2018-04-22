
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Unit : NetworkBehaviour
{
    public static List<Unit> CurrentlySelected = new List<Unit>();
    private static List<Unit> selected;
    private static List<Unit> all = new List<Unit>();

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

    public Faction Faction { get; private set; }

    public SpriteRenderer Bounds;

    private bool renderBounds;
    private SpriteRenderer selBounds;

    public void Awake()
    {
        all.Add(this);
    }

    public void OnDestroy()
    {
        all.Remove(this);
    }

    public void Select()
    {
        if (!CurrentlySelected.Contains(this))
        {
            CurrentlySelected.Add(this);
        }
    }

    public void Update()
    {
        renderBounds = CurrentlySelected.Contains(this);        

        // Completely local and client sided. Just visual.
        if (renderBounds)
        {
            if(Bounds == null)
            {
                Bounds = GetComponentInChildren<SpriteRenderer>();
                if(Bounds == null)
                {
                    Debug.LogWarning("No Bounds sprite renderer assigned in editor, and no SpriteRenderers were found at runtime on children. The bounds are not rendererd. ({0})".Form(name));
                    return;
                }
            }
            if(selBounds == null)
            {
                // Create selection bounds...
                var sel = SelectionBoundsPool.Instance.GetFromPool();
                selBounds = sel.Renderer;
            }
            
            if(selBounds != null)
            {
                // Set position of selection bounds.
                selBounds.color = Color.green;
                selBounds.transform.position = Bounds.bounds.min - Bounds.bounds.size * 0.1f;
                selBounds.size = Bounds.bounds.size * 1.2f;
            }
        }
        else
        {
            if(selBounds != null)
            {
                SelectionBoundsPool.Instance.ReturnToPool(selBounds.GetComponent<SelectionBounds>());
                selBounds = null;
            }
        }
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
        CurrentlySelected.Clear();
        var found = Select(r);
        foreach (var unit in found)
        {
            CurrentlySelected.Add(unit);
        }
    }

    public static void DeselectPermanent()
    {
        // De-select all...
        CurrentlySelected.Clear();
    }

    public static void MoveUnitsTo(Unit[] units, Vector2 target)
    {
        if (units == null || units.Length == 0)
            return;

        foreach (var unit in units)
        {
            if (unit == null)
                continue;

            if (unit.isServer)
            {
                unit.SetMovementTarget(target);
            }
            else
            {
                Debug.LogError("Called MoveUnitsTo and passed a non-server unit! This method can only be called on server!");
            }
        }
    }
}